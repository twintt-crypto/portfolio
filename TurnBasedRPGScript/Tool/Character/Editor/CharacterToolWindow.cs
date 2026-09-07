using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterToolWindow : EditorWindow
{
    private const string PREVIEW_SCENE_PATH = "Assets/Scenes/ScenePreview.unity";

    private GameObject prefab;
    private GameObject instance;
    private Animator animator;

    private string[] stateNames;
    private int selectedStateIndex;

    private float currentTime;
    private bool isPlaying;
    private double lastTime;

    private CharacterAnimationSet animationSet;

    private int selectedEventIndex = -1;
    private int lastSelectedEventIndex = -1;

    private string[] socketNames;
    private bool isDraggingEvent = false;

    private Vector2 scrollPos;

    private HashSet<AnimationEventData> executedEvents = new();
    private List<GameObject> spawnedEffects = new();

    [MenuItem("Tools/S7/Character Tool")]
    public static void Open()
    {
        GetWindow<CharacterToolWindow>("Character Tool");
    }

    private void OnEnable()
    {
        LoadPreviewScene();
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
        ClearInstance();
        ClearSpawnedEffects();
    }

    private void EditorUpdate()
    {
        SceneView.RepaintAll();
        Repaint();
        UpdateEffectsInEditor();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        DrawCharacterLoad();

        if (animator == null)
        {
            EditorGUILayout.HelpBox("Animator 없음", MessageType.Warning);
            EditorGUILayout.EndVertical();
            return;
        }

        DrawStateSelect();
        DrawTimeline();

        DrawTimeSlider();

        GUILayout.Label("Events", EditorStyles.boldLabel);

        if (selectedEventIndex != lastSelectedEventIndex)
        {
            lastSelectedEventIndex = selectedEventIndex;

            if (selectedEventIndex >= 0)
            {
                scrollPos.y = selectedEventIndex * 200f;
            }
        }

        GUILayout.Space(10);

        

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawEventUI();
        EditorGUILayout.EndScrollView();

        UpdatePlayback();

        EditorGUILayout.EndVertical();
    }

    private void SaveAnimationSet()
    {
        if (animationSet == null)
        {
            Debug.LogWarning("AnimationSet 없음");
            return;
        }

        EditorUtility.SetDirty(animationSet);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("AnimationSet 저장 완료");
    }

    private void LoadPreviewScene()
    {
        var current = SceneManager.GetActiveScene();

        if (current.path != PREVIEW_SCENE_PATH)
        {
            EditorSceneManager.OpenScene(PREVIEW_SCENE_PATH, OpenSceneMode.Single);
        }
    }

    private void DrawCharacterLoad()
    {
        EditorGUI.BeginChangeCheck();

        var newPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Character",
            prefab,
            typeof(GameObject),
            false);

        if (EditorGUI.EndChangeCheck())
        {
            if (newPrefab != prefab)
            {
                prefab = newPrefab;

                // 핵심: 캐릭터 바뀌면 AnimationSet 초기화
                animationSet = null;

                // 추가로 안전하게 초기화 (추천)
                selectedStateIndex = 0;
                stateNames = null;

                ClearInstance();
                ClearSpawnedEffects();
            }
        }

        animationSet = (CharacterAnimationSet)EditorGUILayout.ObjectField(
            "Animation Set",
            animationSet,
            typeof(CharacterAnimationSet),
            false);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load"))
            LoadCharacter();

        if (GUILayout.Button("Save"))
            SaveAnimationSet();

        EditorGUILayout.EndHorizontal();
    }

    private AnimatorController GetController()
    {
        var runtime = animator.runtimeAnimatorController;

        if (runtime is AnimatorController c)
            return c;

        if (runtime is AnimatorOverrideController oc)
            return oc.runtimeAnimatorController as AnimatorController;

        return null;
    }

    private void LoadCharacter()
    {
        ClearInstance();
        ClearSpawnedEffects();

        if (prefab == null) return;

        instance = Instantiate(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;

        animator = instance.GetComponent<Animator>();
        if (animator == null) return;

        animator.speed = 0f;

        socketNames = instance.GetComponentsInChildren<Transform>(true)
            .Select(t => t.name).ToArray();

        var controller = GetController();
        if (controller == null) return;

        var states = new List<AnimatorState>();
        CollectStates(controller.layers[0].stateMachine, states);

        stateNames = states.Select(s => s.name).ToArray();

        currentTime = 0;
        isPlaying = false;
    }

    private void CollectStates(AnimatorStateMachine sm, List<AnimatorState> list)
    {
        foreach (var s in sm.states)
            list.Add(s.state);

        foreach (var sub in sm.stateMachines)
            CollectStates(sub.stateMachine, list);
    }

    private void ClearInstance()
    {
        if (instance != null)
            DestroyImmediate(instance);
    }

    private void DrawStateSelect()
    {
        if (stateNames == null || stateNames.Length == 0)
            return;

        if (selectedStateIndex < 0)
            selectedStateIndex = 0;

        EditorGUILayout.BeginHorizontal();

        selectedStateIndex = EditorGUILayout.Popup("State", selectedStateIndex, stateNames);        

        if (GUILayout.Button(isPlaying ? "Pause" : "Play"))
        {
            float length = GetLength();

            if (!isPlaying && currentTime >= length - 0.01f)
                currentTime = 0f;

            executedEvents.Clear();
            ClearSpawnedEffects();

            isPlaying = !isPlaying;
            animator.speed = isPlaying ? 1f : 0f;
            lastTime = EditorApplication.timeSinceStartup;
        }

        if (GUILayout.Button("Reset"))
        {
            currentTime = 0;
            animator.speed = 0f;

            executedEvents.Clear();
            ClearSpawnedEffects();

            animator.Play(stateNames[selectedStateIndex], 0, 0);
            animator.Update(0);
        }

        EditorGUILayout.EndHorizontal();
    }

    private AnimationClip GetCurrentClip()
    {
        var runtime = animator.runtimeAnimatorController;

        var controller = GetController();
        if (controller == null)
            return null;

        var states = new List<AnimatorState>();
        CollectStates(controller.layers[0].stateMachine, states);

        var state = states.FirstOrDefault(s => s.name == stateNames[selectedStateIndex]);

        if (state == null || !(state.motion is AnimationClip baseClip))
            return null;

        // ======================
        // Override 적용
        // ======================
        if (runtime is AnimatorOverrideController oc)
        {
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            oc.GetOverrides(overrides);

            var pair = overrides.FirstOrDefault(x => x.Key == baseClip);

            if (pair.Value != null)
                return pair.Value;
        }

        return baseClip;
    }

    private float GetLength()
    {
        var clip = GetCurrentClip();
        return clip != null ? clip.length : 1f;
    }

    private void DrawTimeline()
    {
        float length = GetLength();
        var clip = GetCurrentClip();
        if (clip == null) return;

        float frameRate = clip.frameRate;
        int totalFrames = Mathf.RoundToInt(length * frameRate);

        Rect totalRect = GUILayoutUtility.GetRect(10, 60, GUILayout.ExpandWidth(true));
        Rect timelineRect = new Rect(totalRect.x, totalRect.y, totalRect.width, 40);

        EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));

        float width = timelineRect.width;

        // 눈금
        Handles.color = new Color(1f, 1f, 1f, 0.15f);

        int step = Mathf.Max(1, totalFrames / 20);

        for (int f = 0; f <= totalFrames; f += step)
        {
            float t = f / (float)totalFrames;
            float x = timelineRect.x + t * width;

            float h = (f % (step * 5) == 0) ? 15f : 8f;

            Handles.DrawLine(
                new Vector2(x, timelineRect.y),
                new Vector2(x, timelineRect.y + h)
            );

            if (f % (step * 5) == 0)
            {
                GUI.Label(
                    new Rect(x + 2, timelineRect.y + 15, 50, 20),
                    f.ToString(),
                    EditorStyles.miniLabel
                );
            }
        }

        // 현재 시간 라인
        float currentX = timelineRect.x + (currentTime / length) * width;
        EditorGUI.DrawRect(new Rect(currentX, timelineRect.y, 2, timelineRect.height), Color.green);

        var data = GetCurrentStateData();
        if (data == null) return;

        Event e = Event.current;

        for (int i = 0; i < data.events.Count; i++)
        {
            var evt = data.events[i];

            float x = timelineRect.x + (evt.time / length) * width;
            bool selected = i == selectedEventIndex;

            Handles.color = selected ? new Color(1f, 0.9f, 0.2f) : new Color(1f, 0.4f, 0.4f);

            Handles.DrawAAConvexPolygon(
                new Vector3(x, timelineRect.y),
                new Vector3(x - 10, timelineRect.y + 10),
                new Vector3(x + 10, timelineRect.y + 10)
            );

            Rect hit = new Rect(x - 10, timelineRect.y, 20, timelineRect.height);

            // 클릭 시 선택만 수행
            if (e.type == EventType.MouseDown && hit.Contains(e.mousePosition))
            {
                selectedEventIndex = i;
                isDraggingEvent = false;

                executedEvents.Clear();
                ClearSpawnedEffects();
                HandleEvent(evt);

                e.Use();
            }

            // 드래그 시작 조건
            if (e.type == EventType.MouseDrag && selectedEventIndex == i)
            {
                if (Mathf.Abs(e.delta.x) > 2f)
                    isDraggingEvent = true;
            }

            // 타임라인 밖으로 나가면 드래그 종료
            if (e.type == EventType.MouseDrag && !timelineRect.Contains(e.mousePosition))
            {
                isDraggingEvent = false;
            }

            // 드래그 중일 때만 시간 이동
            if (!isDraggingSlider && isDraggingEvent && selectedEventIndex == i
                && e.type == EventType.MouseDrag
                && timelineRect.Contains(e.mousePosition))
            {
                float mouseX = Mathf.Clamp(e.mousePosition.x, timelineRect.x, timelineRect.x + width);
                float t = (mouseX - timelineRect.x) / width;

                evt.time = Mathf.Clamp(t * length, 0, length);

                float frameSnap = Mathf.Round(evt.time * frameRate);
                evt.time = frameSnap / frameRate;

                e.Use();
            }
        }

        // 마우스 버튼을 떼면 드래그 종료
        if (e.type == EventType.MouseUp)
        {
            isDraggingEvent = false;
        }
    }

    private bool isDraggingSlider = false;

    private void DrawTimeSlider()
    {
        var clip = GetCurrentClip();
        if (clip == null) return;

        float length = clip.length;
        float frameRate = clip.frameRate;

        Rect rect = GUILayoutUtility.GetRect(10, 20, GUILayout.ExpandWidth(true));

        Event e = Event.current;

        // 슬라이더 클릭 감지
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            isDraggingSlider = true;
        }

        if (e.type == EventType.MouseUp)
        {
            isDraggingSlider = false;
        }

        EditorGUI.BeginChangeCheck();

        float newTime = GUI.HorizontalSlider(rect, currentTime, 0f, length);

        if (EditorGUI.EndChangeCheck())
        {
            float frame = Mathf.Round(newTime * frameRate);
            currentTime = frame / frameRate;

            isPlaying = false;
            animator.speed = 0f;

            executedEvents.Clear();
            ClearSpawnedEffects();

            animator.Play(stateNames[selectedStateIndex], 0, currentTime / length);
            animator.Update(0);

            ExecuteEventsAtTime(currentTime);

            Repaint();
        }

        EditorGUILayout.LabelField($"Time: {currentTime:F3} / {length:F3}");
    }

    private void DrawEventUI()
    {
        if (animationSet == null) return;

        var data = GetCurrentStateData();
        if (data == null)
        {
            EditorGUILayout.HelpBox("이 State에는 이벤트 없음", MessageType.Info);

            if (!Application.isPlaying && GUILayout.Button("Add Event"))
            {
                string stateName = stateNames[selectedStateIndex];

                var newData = new AnimationStateEventData();
                newData.stateName = stateName;
                newData.events = new List<AnimationEventData>()
        {
            new AnimationEventData()
            {
                time = currentTime
            }
        };

                animationSet.animations.Add(newData);

                EditorUtility.SetDirty(animationSet);
            }

            return; // 여기서만 return (버튼 그린 후)
        }

        bool editable = !Application.isPlaying;

        for (int i = 0; i < data.events.Count; i++)
        {
            var evt = data.events[i];
            bool selected = i == selectedEventIndex;

            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = selected
                ? new Color(1f, 1f, 0.6f)
                : new Color(0.75f, 0.75f, 0.75f);

            EditorGUILayout.BeginVertical("box");

            if (selected)
                GUILayout.Label("Selected Event", EditorStyles.boldLabel);

            GUI.enabled = editable;

            // ======================
            // 기본 정보
            // ======================
            evt.time = EditorGUILayout.FloatField("Time", evt.time);

            var prevType = evt.type;
            evt.type = (AnimationEventType)EditorGUILayout.EnumPopup("Type", evt.type);

            // 타입 변경 시 초기화
            if (prevType != evt.type)
            {
                if (evt.type != AnimationEventType.SpawnEffect)
                {
                    evt.prefab = null;
                    evt.effectAddressKey = null;
                    evt.attachSocket = null;
                    evt.duration = 1f;
                }
            }

            // ======================
            // Effect UI (SpawnEffect 전용)
            // ======================
            if (evt.type == AnimationEventType.SpawnEffect)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Effect", EditorStyles.boldLabel);

                var newPrefab = (GameObject)EditorGUILayout.ObjectField(
                    "Prefab",
                    evt.prefab,
                    typeof(GameObject),
                    false);

                if (newPrefab != evt.prefab)
                {
                    evt.prefab = newPrefab;


                    evt.effectAddressKey = GetAddressableAddress(newPrefab);
                    evt.duration = GetEffectDuration(newPrefab); // 자동 세팅

                    EditorUtility.SetDirty(animationSet);
                }

                // Address (읽기전용)
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Address", evt.effectAddressKey);
                EditorGUI.EndDisabledGroup();

                // Socket 드롭다운
                if (socketNames != null && socketNames.Length > 0)
                {
                    int index = System.Array.IndexOf(socketNames, evt.attachSocket);
                    if (index < 0) index = 0;

                    index = EditorGUILayout.Popup("Socket", index, socketNames);
                    evt.attachSocket = socketNames[index];
                }
                else
                {
                    EditorGUILayout.HelpBox("캐릭터 Load 필요", MessageType.Warning);
                }

                // Duration (자동 / 수정 불가)
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField("Duration", evt.duration);
                EditorGUI.EndDisabledGroup();
            }

            // ======================
            // Offset
            // ======================
            evt.positionOffset = EditorGUILayout.Vector3Field("Pos Offset", evt.positionOffset);
            evt.rotationOffset = EditorGUILayout.Vector3Field("Rot Offset", evt.rotationOffset);

            // ======================
            // 삭제
            // ======================
            if (editable && GUILayout.Button("Delete"))
            {
                data.events.RemoveAt(i);
                GUIUtility.ExitGUI();
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = prev;
        }

        // 정렬
        if (!Application.isPlaying && !isDraggingEvent)
        {
            data.events.Sort((a, b) => a.time.CompareTo(b.time));
        }

        // 추가
        if (!Application.isPlaying && GUILayout.Button("Add Event"))
        {
            data.events.Add(new AnimationEventData()
            {
                time = currentTime
            });
        }
    }

    private void ExecuteEventsAtTime(float time)
    {
        var data = GetCurrentStateData();
        if (data == null) return;

        foreach (var evt in data.events)
        {
            if (Mathf.Abs(evt.time - time) < 0.02f && !executedEvents.Contains(evt))
            {
                HandleEvent(evt);
                executedEvents.Add(evt);
            }
        }
    }

    private void HandleEvent(AnimationEventData evt)
    {
        if (evt == null || evt.attachSocket == null || evt.prefab == null)
            return;

        Transform socket = FindSocket(evt.attachSocket);

        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(evt.prefab);

        go.transform.position = socket.position + socket.TransformDirection(evt.positionOffset);
        go.transform.rotation = socket.rotation * Quaternion.Euler(evt.rotationOffset);
        go.transform.SetParent(socket);

        spawnedEffects.Add(go);

        foreach (var p in go.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            p.Simulate(0f, true, true);
            p.Play();
        }
    }

    private void UpdateEffectsInEditor()
    {
        if (Application.isPlaying) return;

        foreach (var go in spawnedEffects)
        {
            if (go == null) continue;

            foreach (var p in go.GetComponentsInChildren<ParticleSystem>())
            {
                p.Simulate(Time.deltaTime, true, false);
            }
        }
    }

    private Transform FindSocket(string socketName)
    {
        if (string.IsNullOrEmpty(socketName))
            return instance.transform;

        return instance.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == socketName) ?? instance.transform;
    }

    private AnimationStateEventData runtimeData;
    private AnimationStateEventData GetCurrentStateData()
    {
        if (animationSet == null || stateNames == null)
            return null;

        string stateName = stateNames[selectedStateIndex];

        var original = animationSet.animations
            .FirstOrDefault(a => a.stateName == stateName);

        if (original == null)
            return null;

        if (Application.isPlaying)
        {
            if (runtimeData == null || runtimeData.stateName != stateName)
            {
                runtimeData = new AnimationStateEventData();
                runtimeData.stateName = original.stateName;

                runtimeData.events = original.events
                    .Select(e => new AnimationEventData()
                    {
                        time = e.time,
                        type = e.type,

                        // Effect 관련 (빠지면 안됨)
                        prefab = e.prefab,
                        effectAddressKey = e.effectAddressKey,
                        attachSocket = e.attachSocket,
                        duration = e.duration,

                        // Transform
                        positionOffset = e.positionOffset,
                        rotationOffset = e.rotationOffset,

                        // Params
                        stringParam = e.stringParam,
                        floatParam = e.floatParam,
                        intParam = e.intParam,
                        boolParam = e.boolParam
                    })
                    .ToList();
            }

            return runtimeData;
        }

        return original;
    }

    private void ClearSpawnedEffects()
    {
        foreach (var go in spawnedEffects)
        {
            if (go != null)
                DestroyImmediate(go);
        }

        spawnedEffects.Clear();
    }

    private void UpdatePlayback()
    {
        if (!isPlaying || animator == null)
            return;

        float length = GetLength();

        double now = EditorApplication.timeSinceStartup;
        float delta = (float)(now - lastTime);
        lastTime = now;

        float speed = animator.speed;
        if (speed == 0f) speed = 1f;

        float prevTime = currentTime;

        // 시간 먼저 증가
        currentTime += delta * speed;

        // 종료 처리
        if (currentTime >= length)
        {
            currentTime = length;
            isPlaying = false;
            animator.speed = 0f;
        }

        // 애니 위치 반영
        animator.Play(stateNames[selectedStateIndex], 0, currentTime / length);
        animator.Update(0);

        // 이벤트 처리 (구간 체크)
        var data = GetCurrentStateData();
        if (data != null)
        {
            foreach (var evt in data.events)
            {
                if (executedEvents.Contains(evt))
                    continue;

                if (prevTime <= evt.time && currentTime >= evt.time)
                {
                    HandleEvent(evt);
                    executedEvents.Add(evt);
                }
            }
        }

        // 디버그
        /*var state = animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log(
            $"[TimeDebug] delta={delta:F4} / speed={speed:F2} / " +
            $"currentTime={currentTime:F3} / normalized={state.normalizedTime:F3} / " +
            $"length={length:F3}"
        );*/
    }

    private string GetAddressableAddress(GameObject prefab)
    {
        if (prefab == null)
            return null;

        string path = AssetDatabase.GetAssetPath(prefab);
        string guid = AssetDatabase.AssetPathToGUID(path);

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            return null;

        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            var group = settings.DefaultGroup;
            entry = settings.CreateOrMoveEntry(guid, group);
        }

        // 주소 규칙 (원하는대로 수정 가능)
        string address = path.Replace("Assets/_RemoteData/", "");

        entry.address = address;

        EditorUtility.SetDirty(settings);

        return address;
    }

    private float GetEffectDuration(GameObject prefab)
    {
        if (prefab == null)
            return 1f;

        float maxDuration = 0f;

        var particles = prefab.GetComponentsInChildren<ParticleSystem>(true);

        foreach (var p in particles)
        {
            var main = p.main;

            float duration = main.duration;

            // Loop면 무한이라 의미 없음 → 무시
            if (main.loop)
                continue;

            // StartLifetime 고려
            float lifetime = 0f;

            if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                lifetime = main.startLifetime.constant;
            else
                lifetime = main.startLifetime.constantMax;

            float total = duration + lifetime;

            if (total > maxDuration)
                maxDuration = total;
        }

        // 최소값 보정
        return maxDuration > 0f ? maxDuration : 1f;
    }
}