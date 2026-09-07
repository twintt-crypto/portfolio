using UnityEngine;
using UnityEditor;

namespace S7
{
    public class PlayerSwitchWindow : EditorWindow
    {
        private int _unitId;
        private AttackType _attackType;

        [MenuItem("Test/Player Switch")]
        private static void ShowWindow()
        {
            GetWindow<PlayerSwitchWindow>("Player Switch");
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            _unitId = EditorGUILayout.IntField("Unit ID", _unitId);
            if (GUILayout.Button("Switch Unit"))
            {
                FieldManager.Instance.SwitchPlayerUnit(_unitId);
            }

            EditorGUILayout.Space();

            _attackType = (AttackType)EditorGUILayout.EnumPopup("Attack Type", _attackType);
            if (GUILayout.Button("Change Attack Type"))
            {
                FieldManager.Instance.ChangePlayerAttackType(_attackType);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
