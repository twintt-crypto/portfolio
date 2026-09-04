#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIButton), true)]
public class UIButtonEditor : Editor
{
    private UIButton uiButton;
    private float previewTime = 0f;
    private float startTime = 0f;
    private AnimationClip playingClip;
    private GameObject previewTarget;
    private bool isPreviewing = false;

    private void OnEnable()
    {
        uiButton = (UIButton)target;

        if (uiButton != null && !EditorUtility.IsPersistent(uiButton.gameObject))
        {
            previewTarget = uiButton.gameObject;
        }
    }

    private void OnDisable()
    {
        StopPreview();
        if (AnimationMode.InAnimationMode()) AnimationMode.StopAnimationMode();
    }

    public override void OnInspectorGUI()
    {
        if (target == null) return;
        
        DrawDefaultInspector();
        
        if (previewTarget == null)
        {
            EditorGUILayout.HelpBox("씬에 배치된 GameObject를 선택하세요. (Project 뷰의 프리팹 자산은 미리보기 불가)", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▶ Animation Preview (Edit Mode)", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Register Clips", GUILayout.Height(25)))
        {
            uiButton.RegisterClips();
            EditorUtility.SetDirty(uiButton);
        }

        DrawPreviewButton("Play Normal", uiButton.NormalClip);
        DrawPreviewButton("Play Highlighted", uiButton.HighlightedClip);
        DrawPreviewButton("Play Pressed", uiButton.PressedClip);
        DrawPreviewButton("Play Selected", uiButton.SelectedClip);
        DrawPreviewButton("Play Disabled", uiButton.DisabledClip);

        // 2. 경로 기반 자동 할당 섹션
        // EditorGUILayout.Space();
        // EditorGUILayout.LabelField("📂 Auto Assemble Clips", EditorStyles.boldLabel);
        // using (new EditorGUILayout.HorizontalScope())
        // {
        //     if (GUILayout.Button("HUD Command", GUILayout.Height(25)))
        //     {
        //         // UIButton의 메서드 호출
        //         AssembleClip();
        //     }
        //
        //     if (GUILayout.Button("Battle Royale", GUILayout.Height(25)))
        //     {
        //         AssembleBattleRoyaleClip();
        //     }
        // }
    }
    
    private void TryAddClip(Animation anim, AnimationClip clip)
    {
        if (clip == null) return;
        if (anim.GetClip(clip.name) != null) return;

        anim.AddClip(clip, clip.name);
    }
    
    private void DrawPreviewButton(string label, AnimationClip clip)
    {
        using (new EditorGUI.DisabledScope(clip == null))
        {
            if (GUILayout.Button(label))
            {
                PlayPreview(clip);
            }
        }
    }

    private void PlayPreview(AnimationClip clip)
    {
        if (clip == null || previewTarget == null) return;

        StopPreview();

        playingClip = clip;
        previewTime = 0f;
        startTime = (float)EditorApplication.timeSinceStartup;

        AnimationMode.StartAnimationMode();

        isPreviewing = true;
        EditorApplication.update += OnEditorUpdate;
    }

    private void StopPreview()
    {
        if (!isPreviewing) return;

        isPreviewing = false;
        EditorApplication.update -= OnEditorUpdate;

        playingClip = null;
        SceneView.RepaintAll();
    }

    private void StopPreviewCompletely()
    {
        StopPreview();
        AnimationMode.StopAnimationMode();
        SceneView.RepaintAll();
    }

    private void OnEditorUpdate()
    {
        if (playingClip == null || previewTarget == null)
        {
            StopPreview();
            return;
        }

        float elapsedTime = (float)(EditorApplication.timeSinceStartup - startTime);

        if (elapsedTime > playingClip.length)
        {
            // 마지막 프레임 적용하고 정지 상태 유지
            AnimationMode.SampleAnimationClip(previewTarget, playingClip, playingClip.length);
            playingClip = null;
            isPreviewing = false; // 더 이상 재생 중은 아니지만, 모드는 유지
            SceneView.RepaintAll();
            return;
        }

        previewTime = elapsedTime;
        AnimationMode.SampleAnimationClip(previewTarget, playingClip, previewTime);
        SceneView.RepaintAll();
    }

    [ContextMenu("Assemble Clip By Path")]
    public void AssembleClip()
    {
        const string dir = "Assets/01_REIW/Art/UI/Animation/HUD/Btn_Command";
        AssignClips(dir, "Btn_Command_Normal", "Btn_Command_Pressed", "Btn_Command_Normal", "Btn_Command_Pressed", "Btn_Command_CoolTime");
    }

    [ContextMenu("Assemble Clip By BattleRoyale Path")]
    public void AssembleBattleRoyaleClip()
    {
        const string dir = "Assets/01_REIW/Art/UI/Prefab/BattleRoyale/Animation";
        AssignClips(dir, "Btn_Battle_Command_Normal", "Btn_Battle_Command_Pressed", "Btn_Battle_Command_Normal", "Btn_Battle_Command_Normal", "Btn_Battle_Command_Disabled");
    }

    private void AssignClips(string dir, string n, string p, string h, string s, string d)
    {
        // private 변수이기 때문에, 리플렉션으로 강제 접근
        FieldInfo normal = typeof(UIButton).GetField("normal", BindingFlags.NonPublic | BindingFlags.Instance);
        AnimationClip normalClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{dir}/{n}.anim");
        normal.SetValue(uiButton, normalClip);
        
        FieldInfo pressed = typeof(UIButton).GetField("pressed", BindingFlags.NonPublic | BindingFlags.Instance);
        AnimationClip pressedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{dir}/{p}.anim");
        pressed.SetValue(uiButton, pressedClip);
        
        FieldInfo highlighted = typeof(UIButton).GetField("highlighted", BindingFlags.NonPublic | BindingFlags.Instance);
        AnimationClip highlightedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{dir}/{h}.anim");
        highlighted.SetValue(uiButton, highlightedClip);
        
        FieldInfo selected = typeof(UIButton).GetField("selected", BindingFlags.NonPublic | BindingFlags.Instance);
        AnimationClip selectedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{dir}/{s}.anim");
        selected.SetValue(uiButton, selectedClip);
        
        FieldInfo disabled = typeof(UIButton).GetField("disabled", BindingFlags.NonPublic | BindingFlags.Instance);
        AnimationClip disabledClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{dir}/{d}.anim");
        disabled.SetValue(uiButton, disabledClip);
        
        // OnValidate(); // Animation 컴포넌트에 클립 즉시 등록
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}
#endif