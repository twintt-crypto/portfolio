using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PresentationGraphWindow : EditorWindow
{
    private const string LastFolderKey = "PresentationGraph_LastFolder";

    private PresentationGraphView _graphView;
    private PresentationGraphAsset _currentAsset;

    [MenuItem("Tools/S7/Presentation Graph")]
    public static void Open()
    {
        var window = GetWindow<PresentationGraphWindow>();
        window.titleContent = new GUIContent("Presentation Graph");
    }

    private void OnEnable()
    {
        CreateUI();
    }

    private void CreateUI()
    {
        rootVisualElement.Clear();

        var toolbar = new Toolbar();

        var objectField = new ObjectField("Graph")
        {
            objectType = typeof(PresentationGraphAsset),
            allowSceneObjects = false,
            value = _currentAsset
        };

        objectField.RegisterValueChangedCallback(evt =>
        {
            _currentAsset = evt.newValue as PresentationGraphAsset;
            _graphView.Load(_currentAsset);
        });


        toolbar.Add(objectField);

        var newButton = new Button(CreateNewGraph) { text = "New" };
        toolbar.Add(newButton);

        var loadButton = new Button(LoadGraph) { text = "Load" };
        toolbar.Add(loadButton);

        var saveButton = new Button(SaveGraph) { text = "Save" };
        toolbar.Add(saveButton);

        var saveAsButton = new Button(SaveAsGraph)
        {
            text = "Save As"
        };

        toolbar.Add(saveAsButton);

        var autoLayoutButton = new Button(() =>
        {
            _graphView.AutoLayout();
        })
        { text = "Auto Layout" };

        toolbar.Add(autoLayoutButton);

        var addNodeMenu = new ToolbarMenu
        {
            text = "Add Node"
        };

        foreach (PresentationNodeType type in Enum.GetValues(typeof(PresentationNodeType)))
        {
            addNodeMenu.menu.AppendAction(type.ToString(), (a) =>
            {
                AddNode(type);
            });
        }

        toolbar.Add(addNodeMenu);

        rootVisualElement.Add(toolbar);

        _graphView = new PresentationGraphView();
        _graphView.OnGraphChanged += Repaint;
        rootVisualElement.Add(_graphView);

        if (_currentAsset != null)
            _graphView.Load(_currentAsset);
        else
            _graphView.NewGraph();
    }

    private void CreateNewGraph()
    {
        string defaultFolder = EditorPrefs.GetString(LastFolderKey, "Assets");

        var path = EditorUtility.SaveFilePanelInProject(
            "Create Presentation Graph",
            "NewPresentationGraph",
            "asset",
            "Create new presentation graph asset",
            defaultFolder);

        if (string.IsNullOrEmpty(path))
            return;

        var asset = ScriptableObject.CreateInstance<PresentationGraphAsset>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        // 폴더 기억
        string folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
        {
            EditorPrefs.SetString(LastFolderKey, folder);
        }

        _currentAsset = asset;
        _graphView.Load(_currentAsset);
    }

    private void SaveGraph()
    {
        if (_currentAsset == null)
        {
            EditorUtility.DisplayDialog("Presentation Graph", "저장할 Graph Asset을 먼저 선택하세요.", "OK");
            return;
        }

        _graphView.Save(_currentAsset);

        string assetPath = AssetDatabase.GetAssetPath(_currentAsset);
        SaveLastFolderFromAssetPath(assetPath, LastFolderKey);

        EditorUtility.DisplayDialog("Presentation Graph", "저장 완료", "OK");
    }

    private void SaveAsGraph()
    {
        if (_currentAsset == null)
            return;

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Presentation Graph As",
            _currentAsset.name,
            "asset",
            "Save graph as new asset"
        );

        if (string.IsNullOrEmpty(path))
            return;

        var newAsset = ScriptableObject.Instantiate(_currentAsset);

        AssetDatabase.CreateAsset(newAsset, path);
        AssetDatabase.SaveAssets();

        _currentAsset = newAsset;

        _graphView.Load(_currentAsset);
    }

    private void AddNode(PresentationNodeType type)
    {
        Vector2 position = _graphView.GetNextNodePosition();
        _graphView.CreateNode(type, position);
    }

    private void LoadGraph()
    {
        string defaultFolder = EditorPrefs.GetString(LastFolderKey, Application.dataPath);

        string path = EditorUtility.OpenFilePanel(
            "Load Presentation Graph",
            defaultFolder,
            "asset");

        if (string.IsNullOrEmpty(path))
            return;

        string folderPath = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folderPath))
        {
            EditorPrefs.SetString(LastFolderKey, folderPath);
        }

        path = FileUtil.GetProjectRelativePath(path);

        var asset = AssetDatabase.LoadAssetAtPath<PresentationGraphAsset>(path);

        if (asset == null)
        {
            EditorUtility.DisplayDialog("Error", "PresentationGraphAsset 로드 실패", "OK");
            return;
        }

        _currentAsset = asset;
        _graphView.Load(_currentAsset);
    }

    private void SaveLastFolderFromAssetPath(string assetPath, string prefsKey)
    {
        if (string.IsNullOrEmpty(assetPath))
            return;

        string folder = Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(folder))
            return;

        EditorPrefs.SetString(prefsKey, folder);
    }
}