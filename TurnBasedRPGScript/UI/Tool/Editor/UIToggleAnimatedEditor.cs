#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIToggleAnimated))]
public class UIToggleAnimatedEditor : UIToggleBaseEditor
{
    private UIToggleAnimated uiToggle;
    private AnimationClip playingClip;
    private GameObject previewTarget;
    private float previewTime;
    private float startTime;
    private bool isPreviewing;

    private void OnEnable()
    {
        uiToggle = (UIToggleAnimated)target;
        previewTarget = uiToggle.gameObject;
    }

    private void OnDisable()
    {
        StopPreview();
        AnimationMode.StopAnimationMode();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (target == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("\u25B6 Animation Preview", EditorStyles.boldLabel);

        DrawPreviewButton("Play Normal", uiToggle.NormalClip);
        DrawPreviewButton("Play Highlighted", uiToggle.HighlightedClip);
        DrawPreviewButton("Play Pressed", uiToggle.PressedClip);
        DrawPreviewButton("Play Selected", uiToggle.SelectedClip);
        DrawPreviewButton("Play Disabled", uiToggle.DisabledClip);
        DrawPreviewButton("Play ON", uiToggle.OnClip);
        DrawPreviewButton("Play OFF", uiToggle.OffClip);
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
            // 마지막 프레임 유지 적용
            AnimationMode.SampleAnimationClip(previewTarget, playingClip, playingClip.length);
            playingClip = null;
            isPreviewing = false;
            SceneView.RepaintAll();
            return;
        }

        previewTime = elapsedTime;
        AnimationMode.SampleAnimationClip(previewTarget, playingClip, previewTime);
        SceneView.RepaintAll();
    }
}
#endif
