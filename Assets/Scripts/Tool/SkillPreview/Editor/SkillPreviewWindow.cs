using Cysharp.Threading.Tasks;
using S7;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillPreviewWindow : EditorWindow
{
    [MenuItem("Tools/S7/Skill Preview")]
    public static void Open()
    {
        GetWindow<SkillPreviewWindow>("Skill Preview");
    }

    // ================================
    // ������
    // ================================

    private int _selectedCharacter;
    private int _selectedMonster;
    private int _selectedGraph;

    private List<T_UnitData> _characters = new();
    private List<T_UnitData> _monsters = new();
    private List<PresentationGraphAsset> _graphs = new();

    private List<string> _characterNames = new();
    private List<string> _monsterNames = new();
    private List<string> _graphNames = new();

    private SkillPreviewRunner _runner;

    private const string PREVIEW_SCENE_PATH = "Assets/Scenes/SceneSkillPreview.unity";

    // ================================
    // �ʱ�ȭ
    // ================================

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        // �÷��� ��� �ƴ� ���� �� �ε� + �÷��� ����
        if (!EditorApplication.isPlaying)
        {
            LoadPreviewSceneAndPlay();
        }
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        _runner?.Destroy();
    }

    private void OnDestroy()
    {
        _runner?.Destroy();

        // �ʿ��ϸ� �÷��� ����
        if (EditorApplication.isPlaying)
            EditorApplication.isPlaying = false;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Init().Forget();
        }
    }

    private async UniTask Init()
    {
        _runner = new SkillPreviewRunner();

        // �� ������ �ö�� ������ ���
        await UniTask.NextFrame();
        await UniTask.NextFrame();

        await LoadUnitData();
        LoadGraphs();

        ObjectPoolManager.Instance.Initialize();
    }

    // ================================
    // �� + �÷���
    // ================================

    private void LoadPreviewSceneAndPlay()
    {
        var currentScene = SceneManager.GetActiveScene();

        // �̹� ������ ���̸� �ٷ� �÷���
        if (currentScene.path == PREVIEW_SCENE_PATH)
        {
            StartPlayMode();
            return;
        }

        // ���� ���� Ȯ��
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        // �� ����
        EditorSceneManager.OpenScene(PREVIEW_SCENE_PATH);

        // �÷��� ��� ����
        StartPlayMode();
    }

    private void StartPlayMode()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
        }
    }

    // ================================
    // ������ �ε�
    // ================================

    private async UniTask LoadUnitData()
    {
        await AddressableManager.LoadGameDataAsync();

        _characters.Clear();
        _monsters.Clear();

        foreach (var unit in T_UnitData.GetAll())
        {
            if (unit == null) continue;

            if (unit.UnitType == UnitType.Character)
                _characters.Add(unit);
            else if (unit.UnitType == UnitType.Monster)
                _monsters.Add(unit);
        }

        _characterNames = _characters.Select(x => x.Name).ToList();
        _monsterNames = _monsters.Select(x => x.Name).ToList();

        _selectedCharacter = 0;
        _selectedMonster = 0;

        OnCharacterChanged().Forget();
        OnMonsterChanged().Forget();
    }

    private void LoadGraphs()
    {
        const string path = "Assets/_RemoteData/Presentation/Skill";

        _graphs.Clear();

        if (!AssetDatabase.IsValidFolder(path))
        {
            Debug.LogError($"���� ����: {path}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets(
            "t:PresentationGraphAsset",
            new[] { path });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<PresentationGraphAsset>(assetPath);

            if (asset != null)
                _graphs.Add(asset);
        }

        _graphs = _graphs.OrderBy(x => x.name).ToList();
        _graphNames = _graphs.Select(x => x.name).ToList();

        if (_selectedGraph >= _graphs.Count)
            _selectedGraph = 0;
    }

    // ================================
    // UI
    // ================================

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Play중에 사용가능합니다.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);

        DrawUnitSection();
        DrawGraphSection();
        DrawControlButtons();
    }

    private void DrawUnitSection()
    {
        EditorGUILayout.LabelField("Unit Selection", EditorStyles.boldLabel);

        if (_characterNames.Count > 0)
        {
            int newIndex = EditorGUILayout.Popup(
                "Character",
                _selectedCharacter,
                _characterNames.ToArray());

            if (newIndex != _selectedCharacter)
            {
                _selectedCharacter = newIndex;
                OnCharacterChanged().Forget();
            }
        }

        if (_monsterNames.Count > 0)
        {
            int newIndex = EditorGUILayout.Popup(
                "Monster",
                _selectedMonster,
                _monsterNames.ToArray());

            if (newIndex != _selectedMonster)
            {
                _selectedMonster = newIndex;
                OnMonsterChanged().Forget();
            }
        }
    }

    private void DrawGraphSection()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Skill Graph", EditorStyles.boldLabel);

        if (_graphNames.Count > 0)
        {
            _selectedGraph = EditorGUILayout.Popup(
                "Graph",
                _selectedGraph,
                _graphNames.ToArray());
        }

        if (GUILayout.Button("Refresh Graphs"))
        {
            LoadGraphs();
        }
    }

    private void DrawControlButtons()
    {
        EditorGUILayout.Space(20);

        if (GUILayout.Button("Play", GUILayout.Height(40)))
        {
            Play();
        }

        if (GUILayout.Button("Stop", GUILayout.Height(30)))
        {
            _runner?.Stop();
        }
    }

    // ================================
    // ����
    // ================================

    private void Play()
    {
        if (_characters.Count == 0 || _monsters.Count == 0)
        {
            Debug.LogWarning("���� ������ ����");
            return;
        }

        if (_graphs.Count == 0)
        {
            Debug.LogWarning("�׷��� ����");
            return;
        }

        var graph = _graphs[_selectedGraph];
        _runner?.Play(graph);
    }

    private async UniTask OnCharacterChanged()
    {
        if (_characters.Count == 0) return;

        await _runner.PreviewCharacter(
            _characters[_selectedCharacter],
            GameObject.Find("Character").transform);
    }

    private async UniTask OnMonsterChanged()
    {
        if (_monsters.Count == 0) return;

        await _runner.PreviewMonster(
            _monsters[_selectedMonster],
            GameObject.Find("Enemy").transform);
    }
}