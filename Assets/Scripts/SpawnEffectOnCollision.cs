using UnityEngine;
using System.Collections;

public class SpawnEffectOnCollision : MonoBehaviour 
{

    public LayerMask CollidableMask;

    public float CollideSpeed = 1;

    public GameObject CollideEffect;

    public float MinDelayBetweenEffects = 0.5f;

    private float _nextEffect;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < _nextEffect)
            return;

        var go = collision.gameObject;
        if ((CollidableMask.value & 1 << go.layer) == 0)
            return;

        var deltaV = collision.relativeVelocity.magnitude;
        if (deltaV > CollideSpeed)
            Instantiate(CollideEffect, collision.contacts[0].point, Quaternion.identity);

        _nextEffect = Time.time + MinDelayBetweenEffects;
    }
}
