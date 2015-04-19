using UnityEngine;
using System.Collections;

public class DamagePart : MonoBehaviour
{

    /** Part to spawn when destructible dies. */
    public GameObject Prefab;

    public Vector2 SpeedRange = new Vector2(0, 20);
    public Vector2 AngularSpeedRange = new Vector2(-200, 200);


    /** Spawn the damage part. */
    public void Spawn(Damageable damageable)
    {
        var center = damageable.DestroyEffectEmitter.position;

        var go = Instantiate(Prefab, transform.position, transform.rotation) as GameObject;
        var r = go.GetComponent<Rigidbody2D>();
        if (r == null)
            return;

        var speed = Random.Range(SpeedRange.x, SpeedRange.y);
        var direction = (transform.position - center).normalized;
        r.velocity = (direction * speed);
        r.angularVelocity = Random.Range(AngularSpeedRange.x, AngularSpeedRange.y);
    }

}
