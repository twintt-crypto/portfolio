using UnityEngine;
using UnityEngine.UI;
using S7.Game.Field.Enemy;

namespace S7.UI.Field
{
    public class EnemyGaugeUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private GameObject _root;

        private FieldEnemyAI _ai;

        private void Awake()
        {
            _ai = GetComponentInParent<FieldEnemyAI>();
            _ai.OnGaugeChanged += OnGaugeChanged;
            _ai.OnStateChanged += OnStateChanged;
            _root.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_ai == null) return;
            _ai.OnGaugeChanged -= OnGaugeChanged;
            _ai.OnStateChanged -= OnStateChanged;
        }

        private void OnGaugeChanged(float gauge)
        {
            _root.SetActive(gauge > 0f);
            
            _slider.value = gauge;
        }

        private void OnStateChanged(ENEMY_AI_STATE state)
        {
            if (state == ENEMY_AI_STATE.DEATH)
            {
                _root.SetActive(false);
            }
        }
    }
}
