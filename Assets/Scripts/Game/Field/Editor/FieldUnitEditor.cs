using UnityEditor;

namespace S7.Game.Field
{
    [CustomEditor(typeof(FieldUnit), true)]
    public class FieldUnitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.name == "_projectileId")
                {
                    SerializedProperty attackTypeProp = serializedObject.FindProperty("_attackType");
                    if (attackTypeProp.enumValueIndex == (int)AttackType.Projectile)
                        EditorGUILayout.PropertyField(iterator, true);
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
