#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// [EditorButton], [GameButton] Attribute가 붙은 메서드를 Inspector에 버튼으로 표시.
/// 별도 CustomEditor가 없는 모든 MonoBehaviour에 자동 적용.
/// 이미 CustomEditor가 있는 클래스는 OnInspectorGUI에서 ButtonAttributeDrawer.DrawButtons(target) 호출.
/// </summary>
[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class ButtonAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ButtonAttributeDrawer.DrawButtons(target);
    }
}

public static class ButtonAttributeDrawer
{
    private struct ButtonEntry
    {
        public string Label;
        public MethodInfo Method;
        public bool IsGameButton;
        public ParameterInfo[] Parameters;
    }

    private static readonly Dictionary<Type, List<ButtonEntry>> _cache = new Dictionary<Type, List<ButtonEntry>>();
    private static readonly Dictionary<string, object[]> _paramValues = new Dictionary<string, object[]>();

    public static void DrawButtons(UnityEngine.Object target)
    {
        if (target == null) return;

        Type type = target.GetType();
        if (!_cache.TryGetValue(type, out List<ButtonEntry> entries))
        {
            entries = new List<ButtonEntry>();
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                EditorButtonAttribute editorAttr = method.GetCustomAttribute<EditorButtonAttribute>();
                if (editorAttr != null)
                {
                    entries.Add(new ButtonEntry
                    {
                        Label = editorAttr.Label ?? method.Name,
                        Method = method,
                        IsGameButton = false,
                        Parameters = method.GetParameters(),
                    });
                }

                GameButtonAttribute gameAttr = method.GetCustomAttribute<GameButtonAttribute>();
                if (gameAttr != null)
                {
                    entries.Add(new ButtonEntry
                    {
                        Label = gameAttr.Label ?? method.Name,
                        Method = method,
                        IsGameButton = true,
                        Parameters = method.GetParameters(),
                    });
                }
            }

            _cache[type] = entries;
        }

        if (entries.Count == 0) return;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

        foreach (ButtonEntry entry in entries)
        {
            bool isPlaying = Application.isPlaying;
            bool enabled = entry.IsGameButton ? isPlaying : !isPlaying;

            object[] args = null;
            if (entry.Parameters.Length > 0)
            {
                string key = $"{target.GetInstanceID()}_{entry.Method.DeclaringType?.FullName}_{entry.Method.Name}";
                if (!_paramValues.TryGetValue(key, out args) || args.Length != entry.Parameters.Length)
                {
                    args = new object[entry.Parameters.Length];
                    for (int i = 0; i < entry.Parameters.Length; i++)
                        args[i] = GetDefaultValue(entry.Parameters[i].ParameterType);
                    _paramValues[key] = args;
                }

                for (int i = 0; i < entry.Parameters.Length; i++)
                    args[i] = DrawParameterField(entry.Parameters[i], args[i]);
            }

            EditorGUI.BeginDisabledGroup(!enabled);
            if (GUILayout.Button(entry.Label))
            {
                entry.Method.Invoke(entry.Method.IsStatic ? null : target, args);
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private static object GetDefaultValue(Type type)
    {
        if (type == typeof(bool)) return false;
        if (type == typeof(int)) return 0;
        if (type == typeof(float)) return 0f;
        if (type == typeof(string)) return "";
        if (type.IsEnum) return Enum.ToObject(type, 0);
        if (type.IsValueType) return Activator.CreateInstance(type);
        return null;
    }

    private static object DrawParameterField(ParameterInfo param, object value)
    {
        string label = param.Name;
        Type type = param.ParameterType;

        if (type == typeof(bool)) return EditorGUILayout.Toggle(label, (bool)value);
        if (type == typeof(int)) return EditorGUILayout.IntField(label, (int)value);
        if (type == typeof(float)) return EditorGUILayout.FloatField(label, (float)value);
        if (type == typeof(string)) return EditorGUILayout.TextField(label, (string)value ?? "");
        if (type.IsEnum) return EditorGUILayout.EnumPopup(label, (Enum)value);

        EditorGUILayout.LabelField(label, $"({type.Name} not supported)");
        return value;
    }
}
#endif
