using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
    public LayerMask PickupMask;

    public float Bonus;

    public GameObject PickupEffect;

    private bool _pickedUp;

    void OnTriggerEnter2D(Collider2D collider)
    {
        var go = collider.gameObject;
        if ((PickupMask.value & 1 << go.layer) == 0)
            return;

        if (_pickedUp)
            return;

        _pickedUp = true;

        PlayerController.Instance.Damageable.AddHealth(Bonus);
        Instantiate(PickupEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
