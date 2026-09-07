using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;
using S7;

namespace S7.Game.Field
{
	public class PlayerManager
	{
		private readonly FieldPlayer _player;

		private readonly Dictionary<int, GameObject> _playerObjects = new Dictionary<int, GameObject>();

		private GameObject _activeObject;
		private bool _initialized;

		// TODO: temp
		private int[] _playerCharacters = new int[] { 1 };

		public int CurrentUnitId { get; private set; } = 0;

		public PlayerManager(FieldPlayer player)
		{
			_player = player;
		}

		public async UniTask Initialize()
		{
			if (_initialized) return;
			_initialized = true;

			UniTask[] characterLoads = new UniTask[_playerCharacters.Length];
			for (int i = 0; i < _playerCharacters.Length; i++)
			{
				characterLoads[i] = RegistPlayer(_playerCharacters[i]);
			}
			await UniTask.WhenAll(characterLoads);

			CurrentUnitId = _playerCharacters[0];
		}
		
		public void SwitchUnit() => SwitchUnit(CurrentUnitId);

		public void SwitchUnit(int unitId)
		{
			if (unitId == 0) return;
			if (!_playerObjects.ContainsKey(unitId)) return;

			if (_activeObject != null) _activeObject.SetActive(false);

			CurrentUnitId = unitId;
			_playerObjects[unitId].SetActive(true);
			_activeObject = _playerObjects[unitId];

			_player.SetPlayerObject();
		}

		public async UniTask RegistPlayer(int unitId)
		{
			if (unitId == 0) return;
			if (_playerObjects.ContainsKey(unitId)) return;

			T_UnitData unitData = T_UnitData.Get(unitId);

			GameObject playerObject = await ResourceManager.NewAsync(unitData.ModelPrefab, _player.transform, usePooling: true);
			if (playerObject == null) return;
			playerObject.transform.rotation = Quaternion.identity;
			playerObject.SetActive(false);

			Animator animator = playerObject.GetComponentInChildren<Animator>();
			RuntimeAnimatorController controller = await ResourceManager.LoadAssetAsync<RuntimeAnimatorController>(unitData.FieldAnimator);
			if(animator!= null && controller != null) animator.runtimeAnimatorController = controller;

			_playerObjects.Add(unitId, playerObject);
		}

		public void MovePlayer(Transform anchor)
		{
			_player.transform.SetPositionAndRotation(
				anchor.position,
				anchor.rotation
			);
		}

		public void RegistLoadResource()
		{
			for (int i = 0; i < _playerCharacters.Length; i++)
			{
				T_UnitData unitData = T_UnitData.Get(_playerCharacters[i]);

				GameSceneManager.Instance.RegistLoadResource(new LoadResourceData<GameObject>()
				{
					assetName = unitData.ModelPrefab,
					loadType = ResourceLoadType.Prefab,
					count = 1,
				});
			}
		}
	}
}
