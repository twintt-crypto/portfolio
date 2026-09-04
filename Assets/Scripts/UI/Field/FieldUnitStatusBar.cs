using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public class FieldUnitStatusBar : MonoBehaviour
    {
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private FieldUnitStatusItem _itemPrefab;

        private readonly List<FieldUnitStatusItem> _items = new();

        public void Initialize(IReadOnlyList<UnitData> units)
        {
            Clear();

            for (int i = 0; i < units.Count; i++)
                AddItem(units[i]);

            gameObject.SetActive(false);
        }

        public void Release()
        {
            Clear();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public FieldUnitStatusItem AddItem(UnitData unit)
        {
            FieldUnitStatusItem item = Instantiate(_itemPrefab, _itemContainer);
            item.Bind(unit);
            _items.Add(item);
            return item;
        }

        public void RemoveItem(int unitId)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Info.unitId != unitId) continue;
                _items[i].Remove();
                _items.RemoveAt(i);
                return;
            }
        }

        public FieldUnitStatusItem GetItem(int unitId)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Info.unitId == unitId) return _items[i];
            }
            return null;
        }

        public void Clear()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null) _items[i].Remove();
            }
            _items.Clear();
            
            foreach (Transform child in _itemContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
