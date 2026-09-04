using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.AI;
using S7.Game.Field.Enemy;

namespace S7.Game.Field
{
    public class FieldEnemySetupWindow : EditorWindow
    {
        private const string PREFAB_PATH = "Assets/_RemoteData/Unit/Prefab";
        private const string ENEMY_UI_PREFAB_PATH = "Assets/_RemoteData/UI/Prefabs/Field/EnemyUI.prefab";

        private string[] _prefabNames;
        private string[] _prefabPaths;
        private int _selectedPrefabIndex = -1;
        private string _enemyName = "Enemy";

        [MenuItem("Tools/Field Enemy Setup")]
        public static void Open()
        {
            GetWindow<FieldEnemySetupWindow>("Field Enemy Setup");
        }

        private void OnEnable()
        {
            LoadPrefabList();
        }

        private void LoadPrefabList()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PREFAB_PATH });
            _prefabNames = new string[guids.Length];
            _prefabPaths = new string[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                _prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                _prefabNames[i] = System.IO.Path.GetFileNameWithoutExtension(_prefabPaths[i]);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Field Enemy Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            _enemyName = EditorGUILayout.TextField("Enemy Name", _enemyName);

            // Body Prefab 드롭다운
            if (_prefabNames == null || _prefabNames.Length == 0)
            {
                EditorGUILayout.HelpBox("프리팹을 찾을 수 없습니다: " + PREFAB_PATH, MessageType.Warning);
                if (GUILayout.Button("Refresh")) LoadPrefabList();
                return;
            }

            _selectedPrefabIndex = EditorGUILayout.Popup("Body Prefab", _selectedPrefabIndex, _prefabNames);

            EditorGUILayout.Space(10);

            EditorGUI.BeginDisabledGroup(_selectedPrefabIndex < 0);
            if (GUILayout.Button("Create Field Enemy", GUILayout.Height(30)))
            {
                CreateFieldEnemy();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CreateFieldEnemy()
        {
            // 루트 GameObject 생성
            GameObject go = new GameObject(_enemyName);
            Undo.RegisterCreatedObjectUndo(go, "Create FieldEnemy");

            go.layer = LayerMask.NameToLayer("Enemy");

            // MonoBehaviour
            Undo.AddComponent<FieldEnemy>(go);
            Undo.AddComponent<FieldEnemyAI>(go);
            Undo.AddComponent<UnitActionController>(go);
            Mover mover = Undo.AddComponent<Mover>(go);
            SerializedObject moverSo = new SerializedObject(mover);
            moverSo.Update();
            moverSo.FindProperty("isRotate").boolValue = true;
            moverSo.ApplyModifiedProperties();
            NavMeshAgent agent = Undo.AddComponent<NavMeshAgent>(go);
            agent.radius = 0.5f;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.height = 2f;

            // Rigidbody & Collider
            Rigidbody rb = Undo.AddComponent<Rigidbody>(go);
            rb.mass = 1f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            CapsuleCollider col = Undo.AddComponent<CapsuleCollider>(go);
            col.radius = 0.2f;
            col.height = 2f;
            col.center = new Vector3(0f, 1f, 0f);

            // Detector 자식
            GameObject detectorObj = new GameObject("Detector");
            Undo.RegisterCreatedObjectUndo(detectorObj, "Create Detector");
            detectorObj.transform.SetParent(go.transform);
            detectorObj.transform.localPosition = Vector3.zero;
            detectorObj.transform.localRotation = Quaternion.identity;

            // SphereCollider, Rigidbody 먼저 추가 후 Detector 추가
            SphereCollider sphereCol = detectorObj.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = 0.00001f;

            Rigidbody detectorRb = detectorObj.AddComponent<Rigidbody>();
            detectorRb.isKinematic = true;
            detectorRb.useGravity = false;

            Detector detectorComp = detectorObj.AddComponent<Detector>();

            // Inspector 순서 조정: Detector를 맨 위로
            ComponentUtility.MoveComponentUp(detectorComp);
            ComponentUtility.MoveComponentUp(detectorComp);

            SerializedObject detectorSo = new SerializedObject(detectorComp);
            detectorSo.Update();
            detectorSo.FindProperty("_targetLayer").intValue = 1 << LayerMask.NameToLayer("Player");
            detectorSo.FindProperty("_detectionRange").floatValue = 5f;
            detectorSo.FindProperty("_lostRange").floatValue = 8f;
            detectorSo.FindProperty("_detectionAngle").floatValue = 90f;
            detectorSo.FindProperty("_detectionMode").enumValueIndex = (int)DetectionMode.FIRST_ENTER;
            detectorSo.ApplyModifiedProperties();

            // _detector 필드 연결
            FieldEnemy enemy = go.GetComponent<FieldEnemy>();
            SerializedObject so = new SerializedObject(enemy);
            so.Update();
            SerializedProperty detectorProp = so.FindProperty("_detector");
            if (detectorProp != null)
            {
                detectorProp.objectReferenceValue = detectorObj.GetComponent<Detector>();
                so.ApplyModifiedProperties();
            }

            // EnemyUI 프리팹
            SetupEnemyUI(go);

            // Body 프리팹
            GameObject bodyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPaths[_selectedPrefabIndex]);
            if (bodyPrefab != null)
            {
                GameObject body = (GameObject)PrefabUtility.InstantiatePrefab(bodyPrefab, go.transform);
                body.transform.localPosition = Vector3.zero;
                body.transform.localRotation = Quaternion.identity;
                Undo.RegisterCreatedObjectUndo(body, "Create Body Prefab");
            }

            // 선택
            Selection.activeGameObject = go;
            Debug.Log($"[FieldEnemySetup] 생성 완료: {go.name}");
        }

        private void SetupEnemyUI(GameObject parent)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ENEMY_UI_PREFAB_PATH);
            if (prefab == null)
            {
                Debug.LogWarning($"[FieldEnemySetup] EnemyUI 프리팹을 찾을 수 없습니다: {ENEMY_UI_PREFAB_PATH}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            instance.name = "EnemyUI";
            instance.transform.localPosition = new Vector3(0f, 2.04f, 0f);
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            Undo.RegisterCreatedObjectUndo(instance, "Create EnemyUI");
        }
    }
}
