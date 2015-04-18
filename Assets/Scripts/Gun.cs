using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour 
{
    /** Where projectiles are emitted from. */
    public Transform Emitter;

    /** Prefabs to use for when creating projectiles. */
    public List<GameObject> Projectiles;

    /** Delay range between successive projectiles. */
    public Vector2 DelayRange;

    /** Speed range of the projectile. */
    public Vector2 SpeedRange;

    /** Angular speed range of projectile. */
    public Vector2 AngularSpeedRange;


    /** Recycling threshold. */
    public int MaxProjectiles;


    /** Time that next projectile can be fired. */
    private float _nextProjectile;

    /** List of previously created projectiles. */
    private readonly Queue<GameObject> _projectiles = new Queue<GameObject>();
    

    /** Fire a projectile from the emitter. */
    public void Fire()
    {
        // Check if we can fire again yet.
        if (Time.time < _nextProjectile)
            return;

        // Ensure projectile count doesn't get outta control by recycling old ones.
        if (_projectiles.Count >= MaxProjectiles)
            Destroy(_projectiles.Dequeue());

        // Create new projectile.
        var prefab = Projectiles[Random.Range(0, Projectiles.Count)];
        var projectile = Instantiate(prefab, Emitter.position, Emitter.rotation) as GameObject;
        if (projectile == null)
            return;

        // Give projectile its initial velocity.
        var r = projectile.GetComponent<Rigidbody2D>();
        if (r == null)
            return;

        var speed = Random.Range(SpeedRange.x, SpeedRange.y);
        var direction = (Emitter.TransformPoint(Vector3.right) - Emitter.position).normalized;
        r.velocity = (direction * speed);
        r.angularVelocity = Random.Range(AngularSpeedRange.x, AngularSpeedRange.y);

        // Add projectile to the recycling queue.
        _projectiles.Enqueue(projectile);

        // Schedule next projectile.
        _nextProjectile = Time.time + Random.Range(DelayRange.x, DelayRange.y);
    }

}
