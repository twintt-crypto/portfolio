using UnityEditor;
using S7.Game.Field.Enemy;

namespace S7.Game.Field
{
    [CustomEditor(typeof(FieldEnemyAI))]
    public class FieldEnemyAIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            SerializedProperty combatStyleProp = serializedObject.FindProperty("_combatStyle");
            ENEMY_COMBAT_STYLE combatStyle = (ENEMY_COMBAT_STYLE)combatStyleProp.intValue;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                switch (iterator.name)
                {
                    case "_leashRange":
                        if (combatStyle == ENEMY_COMBAT_STYLE.STAY)
                            EditorGUILayout.PropertyField(iterator, true);
                        break;
                    case "_targetLostTime":
                        if (combatStyle == ENEMY_COMBAT_STYLE.CHASE)
                            EditorGUILayout.PropertyField(iterator, true);
                        break;
                    case "_optimalRange":
                        if (combatStyle == ENEMY_COMBAT_STYLE.KITE)
                            EditorGUILayout.PropertyField(iterator, true);
                        break;
                    default:
                        EditorGUILayout.PropertyField(iterator, true);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
