using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public class UIBattleTurnOrderBar : UIBase
    {
        [SerializeField] Transform _itemContainer;
        [SerializeField] UIBattleTurnOrderItem _itemPrefab;
        [SerializeField] UIBattleTurnOrderItem _activeItemPrefab;

        readonly List<UIBattleTurnOrderItem> _items = new();
        private UIBattleTurnOrderItem _activeItem;

        public void Build(List<BattleUnitInfo> orderedUnits)
        {
            Clear();

            for (int i = 0; i < orderedUnits.Count; i++)
            {
                if (i == 0)
                {
                    _activeItem = CreateActiveItem(orderedUnits[i]);
                }
                else
                {
                    AddItem(orderedUnits[i]);
                }
            }
        }

        public void AdvanceTurn()
        {
            if (_activeItem == null || _items.Count == 0) return;

            BattleUnitInfo oldInfo = _activeItem.Info;
            _activeItem.Remove();
            AddItem(oldInfo);

            UIBattleTurnOrderItem nextItem = _items[0];
            BattleUnitInfo nextInfo = nextItem.Info;
            _items.RemoveAt(0);
            nextItem.Remove();

            _activeItem = CreateActiveItem(nextInfo);
        }

        private void Clear()
        {
            _activeItem = null;

            foreach (Transform child in _itemContainer)
            {
                Destroy(child.gameObject);
            }

            _items.Clear();
        }

        public void AddItem(BattleUnitInfo unit)
        {
            UIBattleTurnOrderItem item = Instantiate(_itemPrefab, _itemContainer);
            item.Bind(unit);
            _items.Add(item);
        }

        private UIBattleTurnOrderItem CreateActiveItem(BattleUnitInfo info)
        {
            UIBattleTurnOrderItem item = Instantiate(_activeItemPrefab, _itemContainer);
            item.Bind(info);
            item.transform.SetAsFirstSibling();
            return item;
        }

        public void RemoveItem(int unitId)
        {
            if (_activeItem != null && _activeItem.Info.unitId == unitId)
            {
                _activeItem.Remove();
                _activeItem = null;

                if (_items.Count > 0)
                {
                    UIBattleTurnOrderItem nextItem = _items[0];
                    BattleUnitInfo nextInfo = nextItem.Info;
                    _items.RemoveAt(0);
                    nextItem.Remove();

                    _activeItem = CreateActiveItem(nextInfo);
                }
                return;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Info.unitId != unitId) continue;
                UIBattleTurnOrderItem removeItem = _items[i];

                _items.RemoveAt(i);
                removeItem.Remove();

                i--;
            }
        }

#if UNITY_EDITOR
        [GameButton("AdvanceTurn")]
        private void TestAdvanceTurn() => AdvanceTurn();
#endif
    }
}