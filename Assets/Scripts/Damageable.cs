using System;
using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour {

    public float Health = 100;

	public LayerMask DamageMask;
    public Vector2 DamageSpeedRange = new Vector2(5, 100);
    public Vector2 DamageRange = new Vector2(5, 100);
    public GameObject DamageEffect;
    public GameObject DestroyEffect;
    public Transform DestroyEffectEmitter;

    public bool DeactivateOnDeath;

    public float DelayBetweenHits = 0.1f;

    private float _nextHit;

    void Start()
    {
        if (!DestroyEffectEmitter)
            DestroyEffectEmitter = transform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we can't be hit yet.
        if (Time.time < _nextHit)
            return;

        // Check if already dead.
        if (Health <= 0)
            return;

        // Can this object hurt us?
        var go = collision.gameObject;
        if ((DamageMask.value & 1 << go.layer) == 0)
            return;

        // Determine how fast the object hit us.
        var speed = collision.relativeVelocity.magnitude;
        if (speed < DamageSpeedRange.x)
            return;

        // Determine how hard we were hit.
        var range = DamageSpeedRange.y - DamageSpeedRange.x;
        var strength = Mathf.Clamp01((speed - DamageSpeedRange.x) / range);
        var damage = Mathf.Lerp(DamageRange.x, DamageRange.y, strength);
        // Debug.Log("Damaged: (damage = " + damage + ")");
        
        // Apply damage.
        Health = Mathf.Max(0, Health - damage);
        _nextHit = Time.time + DelayBetweenHits;

        // Spawn damage effect.
        Instantiate(DamageEffect, collision.contacts[0].point, Quaternion.identity);

        // Check if we just died.
        if (Health <= 0)
            Die();
    }

    void Die()
    {
        // Spawn death effect.
        var t = DestroyEffectEmitter;
        Instantiate(DestroyEffect, t.position, t.rotation);

        // Spawn death parts.
        var parts = GetComponentsInChildren<DamagePart>();
        foreach (var part in parts)
            part.Spawn(this);

        // Kill this object.
        if (DeactivateOnDeath)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}
