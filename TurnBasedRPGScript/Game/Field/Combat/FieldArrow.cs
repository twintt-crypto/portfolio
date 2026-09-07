using UnityEngine;

namespace S7.Game.Field
{
    public class FieldArrow : MonoBehaviour
    {
        private float _speed;
        private float _travelDistance;
        private CombatColliderController _attacker;
        private const float MaxRange = 10f; // temp

        public void Launch(Vector3 direction, float speed, CombatColliderController attacker)
        {
            _speed = speed;
            _travelDistance = 0f;
            _attacker = attacker;
            transform.forward = direction;
        }

        private void FixedUpdate()
        {
            if (_speed == 0f) return;

            float delta = _speed * Time.fixedDeltaTime;
            transform.Translate(Vector3.forward * delta);
            _travelDistance += delta;

            if (_travelDistance >= MaxRange) Free();
        }

        private void OnTriggerEnter(Collider other)
        {
            CombatColliderController target = other.GetComponentInParent<CombatColliderController>();
            if (target == null || target == _attacker) return;

            target.ReceiveHit(_attacker);
            Free();
        }

        private void Free()
        {
            Destroy(this);
            ObjectPoolManager.Instance.Free(gameObject);
        }
    }
}
