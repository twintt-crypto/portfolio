using Game.QTE;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(QTEConfig))]
public class QTEConfigDrawer : PropertyDrawer
{
    private static readonly string[] _commonFields = { "type", "delay", "position", "duration" };
    private static readonly string[] _timingFields = { "timingPoint", "perfectNegative", "perfectPositive", "goodNegative", "goodPositive" };
    private static readonly string[] _mashFields = { "mashThreshold", "mashGoodThreshold" };

    private float LineH => EditorGUIUtility.singleLineHeight;
    private float Spacing => EditorGUIUtility.standardVerticalSpacing;
    private float Step => LineH + Spacing;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return LineH;

        int lines = 1 + _commonFields.Length;

        var type = (QTE_TYPE)property.FindPropertyRelative("type").enumValueIndex;
        switch (type)
        {
            case QTE_TYPE.TAP:
            case QTE_TYPE.RELEASE:
                lines += _timingFields.Length;
                break;
            case QTE_TYPE.SWIPE:
                lines += _timingFields.Length + 1; // requiredDir
                break;
            case QTE_TYPE.MASH:
                lines += _mashFields.Length;
                break;
        }

        return lines * Step;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, LineH);

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
        rect.y += Step;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            foreach (var field in _commonFields)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(field));
                rect.y += Step;
            }

            var type = (QTE_TYPE)property.FindPropertyRelative("type").enumValueIndex;
            switch (type)
            {
                case QTE_TYPE.TAP:
                case QTE_TYPE.RELEASE:
                    foreach (var field in _timingFields)
                    {
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative(field));
                        rect.y += Step;
                    }
                    break;
                case QTE_TYPE.SWIPE:
                    foreach (var field in _timingFields)
                    {
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative(field));
                        rect.y += Step;
                    }
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("requiredDir"));
                    break;
                case QTE_TYPE.MASH:
                    foreach (var field in _mashFields)
                    {
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative(field));
                        rect.y += Step;
                    }
                    break;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
