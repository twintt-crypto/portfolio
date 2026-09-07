using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelBattleUnitSlots : UIPanelBattleBase
    {
        [SerializeField] Transform _slotContainer;
        [SerializeField] UIBattleUnitSlot _slotPrefab;
        [SerializeField] private Image actionPointsImage; 

        readonly List<UIBattleUnitSlot> _slots = new();

        public void SetUnits(List<BattleUnitInfo> units)
        {
            Clear();

            foreach (BattleUnitInfo info in units)
            {
                UIBattleUnitSlot slot = Instantiate(_slotPrefab, _slotContainer);
                slot.Bind(info);
                _slots.Add(slot);
            }
        }

        public void Clear()
        {
            foreach (Transform child in _slotContainer) Destroy(child.gameObject);
            _slots.Clear();
        }
    }

}
