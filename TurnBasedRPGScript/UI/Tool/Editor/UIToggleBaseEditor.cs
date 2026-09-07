#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIToggleBase), true)]
public class UIToggleBaseEditor : Editor
{
    private UIToggleBase uiToggle;
    private SerializedProperty onObjectsProp;
    private SerializedProperty offObjectsProp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        uiToggle = (UIToggleBase)target;

        // 공통 기본 인스펙터 표시
        DrawDefaultInspector();

        // 상태 프리뷰 (토글 상태)
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▶ Toggle 상태 미리보기", EditorStyles.boldLabel);

        Toggle toggle = uiToggle.GetComponent<Toggle>();
        if (toggle != null)
        {
            EditorGUI.BeginChangeCheck();
            bool newIsOn = EditorGUILayout.Toggle("Is On", toggle.isOn);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(toggle, "Change Toggle isOn");
                toggle.isOn = newIsOn;
                EditorUtility.SetDirty(toggle);

                // on/off 오브젝트 활성화 갱신
                ApplyOnOffObjects(uiToggle, newIsOn);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ApplyOnOffObjects(UIToggleBase toggleBase, bool isOn)
    {
        var so = new SerializedObject(toggleBase);
        var onList = so.FindProperty("onObjects");
        var offList = so.FindProperty("offObjects");

        for (int i = 0; i < onList.arraySize; i++)
        {
            var obj = onList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            if (obj != null) obj.SetActive(isOn);
        }

        for (int i = 0; i < offList.arraySize; i++)
        {
            var obj = offList.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            if (obj != null) obj.SetActive(!isOn);
        }
    }
}
#endif
