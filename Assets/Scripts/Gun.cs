using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour 
{
    /** Where projectiles are emitted from. */
    public Transform Emitter;

    /** Target transform (used to work around issues with player mirroring). */
    public Transform Target;

    /** Delay range between successive projectiles. */
    public Vector2 DelayRange;

    /** Speed range of the projectile. */
    public Vector2 SpeedRange;

    /** Angular speed range of projectile. */
    public Vector2 AngularSpeedRange;

    /** Range at which objects get sucked into the emitter. */
    public float CaptureRange = 1;

    /** Layer mask for things that can be captured. */
    public LayerMask CaptureMask;

    /** Range for sucking objects in. */
    public float SuckRange = 5;

    /** Radius for sucking objects in. */
    public float SuckRadius = 1;

    /** Sucktion power! */
    public float SuckForce = 100;

    /** Falloff of suction power with distance. */
    public AnimationCurve SuckFalloff;

    /** Suck effect. */
    public GameObject SuckEffect;

    /** Blow effect. */
    public GameObject BlowEffect;


    /** Time that next projectile can be fired. */
    private float _nextProjectile;

    /** Emitter direction vector. */
    private Vector3 _direction;

    /** List of previously created projectiles. */
    private readonly Queue<GameObject> _projectiles = new Queue<GameObject>();

    private ParticleSystem[] _suckParticles;
    private ParticleSystem[] _blowParticles;

    void Start()
    {
        _suckParticles = SuckEffect.GetComponentsInChildren<ParticleSystem>();
        _blowParticles = BlowEffect.GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        var blowing = Input.GetButton("Fire1");
        var sucking = !blowing && Input.GetButton("Fire2");

        if (blowing)
            Blow();
        else if (sucking)
            Suck();

        foreach (var system in _suckParticles)
            system.enableEmission = sucking;
        foreach (var system in _blowParticles)
            system.enableEmission = blowing;
    }

    void LateUpdate()
    {
        SuckEffect.transform.position = Emitter.position;
        SuckEffect.transform.LookAt(Target.position, Vector3.up);
        BlowEffect.transform.position = Emitter.position;
        BlowEffect.transform.LookAt(Target.position, Vector3.up);
    }

    /** Suck projectiles into the emitter. */
    void Suck()
    {
        var direction = (Emitter.TransformPoint(Vector3.right) - Emitter.position).normalized;
        var suckers = Physics2D.CircleCastAll(Emitter.position, SuckRadius, direction, SuckRange, CaptureMask);
        foreach (var sucker in suckers)
        {
            var r = sucker.rigidbody;
            if (r == null)
                continue;

            var delta = Emitter.position - sucker.transform.position;
            var d = delta.magnitude;

            if (d < CaptureRange)
            {
                // Object is close enough, grab it!
                var projectile = sucker.collider.gameObject;
                projectile.SetActive(false);
                _projectiles.Enqueue(projectile);
            }
            else
            {
                // Not close enough, suck it towards us.
                var f = delta.normalized * SuckFalloff.Evaluate(d/SuckRange)*SuckForce;
                r.AddForce(f);
            }
        }
    }


    /** Fire a projectile from the emitter. */
    void Blow()
    {
        // Check if we can fire again yet.
        if (Time.time < _nextProjectile)
            return;

        // Check if player has any projectiles.
        if (_projectiles.Count <= 0)
            return;

        // Create new projectile.
        var projectile = _projectiles.Dequeue();
        projectile.transform.position = Emitter.position;
        projectile.transform.rotation = Emitter.rotation;
        projectile.SetActive(true);

        // Give projectile its initial velocity.
        var r = projectile.GetComponent<Rigidbody2D>();
        if (r == null)
            return;

        var speed = Random.Range(SpeedRange.x, SpeedRange.y);
        var direction = (Emitter.TransformPoint(Vector3.right) - Emitter.position).normalized;
        r.velocity = (direction * speed);
        r.angularVelocity = Random.Range(AngularSpeedRange.x, AngularSpeedRange.y);

        // Schedule next projectile.
        _nextProjectile = Time.time + Random.Range(DelayRange.x, DelayRange.y);
    }

}
