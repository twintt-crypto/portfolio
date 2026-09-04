using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform _target;
    private int _hitIndex = 0;
    private float _speed;
    private Action<int> _onHit;

    public void Init(Transform target, float speed, int hitIndex, Action<int> onHit)
    {
        _target = target;
        _speed = speed;
        _hitIndex = hitIndex;
        _onHit = onHit;
    }

    private void Update()
    {
        if (_target == null)
            return;

        transform.LookAt(_target);
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null)
            return;

        if (_target.parent.gameObject.layer == LayerMask.NameToLayer("Monter"))
        {
            _onHit?.Invoke(_hitIndex);
            ResourceManager.Free(gameObject);
        }        
    }
}