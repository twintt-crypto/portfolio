using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class FieldUnitStatusItem : MonoBehaviour
    {
        [SerializeField] private Image _hpFillImage;
        // [SerializeField] private TextMeshProUGUI _hpText;

        public UnitData Info { get; private set; }

        public void Bind(UnitData unit)
        {
            Info = unit;
            OnHpChanged((int)unit._stat.hp, (int)unit.MaxHp);
        }

        public void OnHpChanged(int hp, int maxHp)
        {
            float ratio = maxHp > 0 ? (float)hp / maxHp : 0f;
            _hpFillImage.fillAmount = Mathf.Max(0, ratio);
            // if (_hpText != null) _hpText.text = $"{hp}/{maxHp}";
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}
