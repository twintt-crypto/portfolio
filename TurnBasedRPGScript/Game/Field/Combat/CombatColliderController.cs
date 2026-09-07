using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatColliderController : MonoBehaviour
{
	[SerializeField] private Collider hitCollider;
	[SerializeField] private Collider hurtCollider;

	private readonly HashSet<CombatColliderController> alreadyHit = new();

    public Collider HurtCollider { get => hurtCollider; set => hurtCollider = value; }

    public event Action<CombatColliderController> OnHitTarget;
	public event Action<CombatColliderController> OnHurt;
	public event Action<int> OnHitEvent;

	private void Awake()
	{
		if(hitCollider) hitCollider.gameObject.layer = LayerMask.NameToLayer("HitBox");
		if(hurtCollider) hurtCollider.gameObject.layer = LayerMask.NameToLayer("HurtBox");
		if(GetComponent<Rigidbody>() == null)
		{
			Rigidbody rb = gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
		}
		HitColliderOff();
	}	

    public void HitColliderOn()
    {
		alreadyHit.Clear();
		if(hitCollider) hitCollider.enabled = true;
	}

	public void HitColliderOff()
	{
		if(hitCollider) hitCollider.enabled = false;
	}

	public void DisableAll()
	{
		if (hitCollider) hitCollider.enabled = false;
		if (hurtCollider) hurtCollider.enabled = false;
	}

	public void ReceiveHit(CombatColliderController attacker)
	{
		OnHurt?.Invoke(attacker);
	}

	// hit 가 hurt 호출
	private void OnTriggerEnter(Collider other)
	{
		CombatColliderController target = other.GetComponentInParent<CombatColliderController>();
		if (target == null || target == this || alreadyHit.Contains(target)) return;

		alreadyHit.Add(target);
		target.ReceiveHit(this);
		OnHitTarget?.Invoke(target);
	}
}
