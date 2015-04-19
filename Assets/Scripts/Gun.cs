using System.Collections;
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

    /** Sucking start sound. */
    public AudioSource SuckStartSound;

    /** Sucking loop sound. */
    public AudioSource SuckLoopSound;

    /** Sucking end sound. */
    public AudioSource SuckStopSound;

    /** Blowing start sound. */
    public AudioSource BlowStartSound;

    /** Blowing loop sound. */
    public AudioSource BlowLoopSound;

    /** Blowing end sound. */
    public AudioSource BlowStopSound;

    /** Capture effect. */
    public GameObject CaptureEffect;

    /** Captured object effect. */
    public GameObject CapturedEffect;

    /** Fire effect. */
    public GameObject FireEffect;


    public string JunkLayer;

    /** Time that next projectile can be fired. */
    private float _nextProjectile;

    /** Emitter direction vector. */
    private Vector3 _direction;

    /** Whether gun is sucking. */
    private bool _sucking;

    /** Whether gun is blowing. */
    private bool _blowing;

    /** List of previously created projectiles. */
    private readonly Queue<GameObject> _projectiles = new Queue<GameObject>();

    private ParticleSystem[] _suckParticles;
    private ParticleSystem[] _blowParticles;

    void Start()
    {
        _suckParticles = SuckEffect.GetComponentsInChildren<ParticleSystem>();
        _blowParticles = BlowEffect.GetComponentsInChildren<ParticleSystem>();

        StartCoroutine("SuckRoutine");

        // Disabled blowing sounds since there are now firing effects.
        // StartCoroutine("BlowRoutine");
    }

    void Update()
    {
        _blowing = Input.GetButton("Fire1");
        _sucking = !_blowing && Input.GetButton("Fire2");

        if (_blowing)
            Blow();
        else if (_sucking)
            Suck();

        foreach (var system in _suckParticles)
            system.enableEmission = _sucking;
        foreach (var system in _blowParticles)
            system.enableEmission = _blowing;
    }

    IEnumerator SuckRoutine()
    {
        var wait = new WaitForEndOfFrame();
        while (gameObject.activeSelf)
        {
            while (!_sucking)
                yield return wait;

            SuckStartSound.Play();
            yield return new WaitForSeconds(0.5f);
            SuckLoopSound.Play();

            while (_sucking)
                yield return wait;

            SuckStopSound.Play();
            yield return new WaitForSeconds(0.1f);
            SuckLoopSound.Stop();
        }
    }

    IEnumerator BlowRoutine()
    {
        var wait = new WaitForEndOfFrame();
        while (gameObject.activeSelf)
        {
            while (!_blowing)
                yield return wait;

            BlowStartSound.Play();
            yield return new WaitForSeconds(0.5f);
            BlowLoopSound.Play();

            while (_blowing)
                yield return wait;

            BlowStopSound.Play();
            yield return new WaitForSeconds(0.1f);
            BlowLoopSound.Stop();
        }
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
        var captured = false;

        foreach (var sucker in suckers)
        {
            var r = sucker.rigidbody;
            if (r == null)
                continue;

            var delta = Emitter.position - sucker.transform.position;
            var d = delta.magnitude;

            if (d < CaptureRange && !captured)
            {
                // Object is close enough, grab it!
                var projectile = sucker.collider.gameObject;
                projectile.SetActive(false);
                _projectiles.Enqueue(projectile);

                // Capture the object.
                Capture(sucker.collider.gameObject);

                // No more captures this frame.
                captured = true;
                
                // Play capture effect.
                var parent = SuckEffect.transform;
                var effect = Instantiate(CaptureEffect, parent.position, parent.rotation) as GameObject;
                effect.transform.parent = parent.transform;

                // Play captured object effect.
                Instantiate(CapturedEffect, sucker.transform.position, sucker.transform.rotation);
            }
            else
            {
                // Not close enough, suck it towards us.
                var f = delta.normalized * SuckFalloff.Evaluate(d/SuckRange)*SuckForce;
                r.AddForce(f);
            }
        }
    }

    void Capture(GameObject go)
    {
        // Break any joints on the object.
        var joint = go.GetComponent<Joint2D>();
        if (joint)
            Destroy(joint);

        // Unparent object and turn it into junk.
        go.layer = LayerMask.NameToLayer(JunkLayer);
        go.transform.parent = null;

        // Chapture any child objects.
        while (go.transform.childCount > 0)
            Capture(go.transform.GetChild(0).gameObject);
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

        // Play fire effect.
        var parent = BlowEffect.transform;
        var effect = Instantiate(FireEffect, parent.position, parent.rotation) as GameObject;
        effect.transform.parent = parent.transform;
    }

}
