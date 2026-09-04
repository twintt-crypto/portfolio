using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class AnimatorOverrideAutoAssignWindow : EditorWindow
{
    private AnimatorOverrideController overrideController;
    private DefaultAsset animationFolder;
    private AnimatorClipMappingPreset mappingPreset;

    private Vector2 scrollPos;
    private List<MatchResult> results = new();
    private bool hasResults;

    private enum MatchState
    {
        SUCCESS,
        MULTIPLE,
        NO_MATCH,
        NO_RULE,
    }

    private class MatchResult
    {
        public string baseClipName;
        public AnimationClip baseClip;
        public MatchState state;
        public List<AnimationClip> candidates = new();
        public int selectedIndex;
        public bool isFallback;
    }

    [MenuItem("Tools/Animator Override Auto Assign")]
    public static void Open()
    {
        AnimatorOverrideAutoAssignWindow window = GetWindow<AnimatorOverrideAutoAssignWindow>("Override Auto Assign");
        window.minSize = new Vector2(500, 400);
    }

    private void OnGUI()
    {
        DrawHeader();

        if (!hasResults)
            return;

        DrawResults();
        DrawApplyButtons();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(5);

        overrideController = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Override Controller", overrideController, typeof(AnimatorOverrideController), false);

        animationFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Animation Folder", animationFolder, typeof(DefaultAsset), false);

        mappingPreset = (AnimatorClipMappingPreset)EditorGUILayout.ObjectField(
            "Mapping Preset", mappingPreset, typeof(AnimatorClipMappingPreset), false);

        EditorGUILayout.Space(5);

        EditorGUI.BeginDisabledGroup(overrideController == null || animationFolder == null || mappingPreset == null);
        if (GUILayout.Button("Auto Assign", GUILayout.Height(30)))
            RunAutoAssign();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);
    }

    private void RunAutoAssign()
    {
        results.Clear();

        string folderPath = AssetDatabase.GetAssetPath(animationFolder);
        List<AnimationClip> clips = LoadClipsFromFolder(folderPath);
        List<AnimationClip> baseClips = GetBaseClips();

        foreach (AnimationClip baseClip in baseClips)
        {
            MatchResult result = new MatchResult();
            result.baseClipName = baseClip.name;
            result.baseClip = baseClip;

            AnimatorClipMappingPreset.Entry rule = FindMatchingRule(baseClip.name);

            if (rule == null)
            {
                result.state = MatchState.NO_RULE;
                results.Add(result);
                continue;
            }

            FindOverrideClips(clips, rule, result);
            results.Add(result);
        }

        hasResults = true;
    }

    private AnimatorClipMappingPreset.Entry FindMatchingRule(string baseClipName)
    {
        foreach (AnimatorClipMappingPreset.Entry entry in mappingPreset.entries)
        {
            if (entry.baseClipName == baseClipName)
                return entry;
        }

        return null;
    }

    private void FindOverrideClips(List<AnimationClip> clips, AnimatorClipMappingPreset.Entry rule, MatchResult result)
    {
        for (int i = 0; i < rule.overridePatterns.Length; i++)
        {
            string pattern = rule.overridePatterns[i];
            if (string.IsNullOrEmpty(pattern)) continue;

            List<AnimationClip> matched = new();

            foreach (AnimationClip clip in clips)
            {
                if (Regex.IsMatch(clip.name, pattern, RegexOptions.IgnoreCase))
                    matched.Add(clip);
            }

            if (matched.Count > 0)
            {
                result.candidates = matched;
                result.isFallback = i > 0;

                if (matched.Count == 1)
                {
                    result.state = MatchState.SUCCESS;
                    result.selectedIndex = 0;
                }
                else
                {
                    result.state = MatchState.MULTIPLE;
                    result.selectedIndex = 0;
                }

                return;
            }
        }

        result.state = MatchState.NO_MATCH;
    }

    private List<AnimationClip> LoadClipsFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
        List<AnimationClip> clips = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null) clips.Add(clip);
        }

        return clips;
    }

    private List<AnimationClip> GetBaseClips()
    {
        AnimatorController baseController = overrideController.runtimeAnimatorController as AnimatorController;
        if (baseController == null) return new List<AnimationClip>();

        List<AnimationClip> clips = new();

        foreach (AnimatorControllerLayer layer in baseController.layers)
            CollectClips(layer.stateMachine, clips);

        return clips;
    }

    private void CollectClips(AnimatorStateMachine sm, List<AnimationClip> clips)
    {
        foreach (ChildAnimatorState state in sm.states)
        {
            if (state.state.motion is AnimationClip clip)
                clips.Add(clip);
        }

        foreach (ChildAnimatorStateMachine sub in sm.stateMachines)
            CollectClips(sub.stateMachine, clips);
    }

    private void DrawResults()
    {
        int successCount = results.Count(r => r.state == MatchState.SUCCESS);
        int warnCount = results.Count(r => r.state == MatchState.MULTIPLE || r.state == MatchState.NO_MATCH || r.state == MatchState.NO_RULE);

        EditorGUILayout.LabelField($"Results: {successCount} matched, {warnCount} warnings", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (MatchResult result in results)
        {
            switch (result.state)
            {
                case MatchState.SUCCESS:
                    DrawSuccessRow(result);
                    break;
                case MatchState.MULTIPLE:
                    DrawMultipleRow(result);
                    break;
                case MatchState.NO_MATCH:
                    DrawNoMatchRow(result);
                    break;
                case MatchState.NO_RULE:
                    DrawNoRuleRow(result);
                    break;
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSuccessRow(MatchResult result)
    {
        EditorGUILayout.BeginHorizontal("box");
        string label = result.isFallback ? "~" : "O";
        EditorGUILayout.LabelField(label, GUILayout.Width(20));
        EditorGUILayout.LabelField(result.baseClipName, GUILayout.Width(180));
        EditorGUILayout.LabelField("<-", GUILayout.Width(25));
        string clipLabel = result.candidates[0].name;
        if (result.isFallback) clipLabel += " (fallback)";
        EditorGUILayout.LabelField(clipLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawMultipleRow(MatchResult result)
    {
        EditorGUILayout.BeginHorizontal("helpbox");
        string label = result.isFallback ? "~?" : "?";
        EditorGUILayout.LabelField(label, GUILayout.Width(20));
        EditorGUILayout.LabelField(result.baseClipName, GUILayout.Width(180));
        EditorGUILayout.LabelField("<-", GUILayout.Width(25));

        string[] names = result.candidates.Select(c => c.name).ToArray();
        result.selectedIndex = EditorGUILayout.Popup(result.selectedIndex, names);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawNoMatchRow(MatchResult result)
    {
        EditorGUILayout.BeginHorizontal("helpbox");
        EditorGUILayout.LabelField("X", GUILayout.Width(20));
        EditorGUILayout.LabelField(result.baseClipName, GUILayout.Width(180));
        EditorGUILayout.LabelField("No matching clip found", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawNoRuleRow(MatchResult result)
    {
        EditorGUILayout.BeginHorizontal("helpbox");
        EditorGUILayout.LabelField("!", GUILayout.Width(20));
        EditorGUILayout.LabelField(result.baseClipName, GUILayout.Width(180));
        EditorGUILayout.LabelField("No rule defined in preset", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawApplyButtons()
    {
        EditorGUILayout.Space(5);

        bool hasAnyMatch = results.Any(r => r.state == MatchState.SUCCESS || r.state == MatchState.MULTIPLE);

        EditorGUI.BeginDisabledGroup(!hasAnyMatch);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply All Matched", GUILayout.Height(28)))
            ApplyMatched(false);

        if (GUILayout.Button("Apply All (Include Multiple)", GUILayout.Height(28)))
            ApplyMatched(true);

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }

    private void ApplyMatched(bool includeMultiple)
    {
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new();
        overrideController.GetOverrides(overrides);

        int appliedCount = 0;

        foreach (MatchResult result in results)
        {
            bool shouldApply = result.state == MatchState.SUCCESS ||
                               (includeMultiple && result.state == MatchState.MULTIPLE);

            if (!shouldApply) continue;
            if (result.candidates.Count == 0) continue;

            AnimationClip selectedClip = result.candidates[result.selectedIndex];

            for (int i = 0; i < overrides.Count; i++)
            {
                if (overrides[i].Key == result.baseClip)
                {
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(result.baseClip, selectedClip);
                    appliedCount++;
                    break;
                }
            }
        }

        overrideController.ApplyOverrides(overrides);
        EditorUtility.SetDirty(overrideController);
        AssetDatabase.SaveAssets();

        Debug.Log($"[Override Auto Assign] {appliedCount} clips applied to {overrideController.name}");
    }
}
