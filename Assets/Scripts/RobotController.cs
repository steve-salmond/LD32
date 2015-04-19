using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class RobotController : MonoBehaviour 
{

    public Transform Torso;

    public float MaxSpeed = 5;
    public Vector2 InputForceScale = Vector2.one;
    public Vector2 AirborneForceScale = Vector2.one;
    public float JumpForce = 1000;

    public LayerMask GroundMask;
    public Vector2 GroundArea = new Vector2(3, 1);

    public float IdleDrag = 1;
    public float MovingDrag = 0;

    public float AimOffsetScale = 5;

    /** Jump effect. */
    public GameObject JumpEffect;

    /** Footstep effect. */
    public GameObject FootstepEffect;


    public Vector2 PatrolWalkInterval = new Vector2(5, 5);
    public Vector2 PatrolWaitInterval = new Vector2(2, 2);

    public Vector2 FireSpeedRange = new Vector2(15, 20);
    public Transform FireEmitter;
    public GameObject FireProjectile;
    public float FireRange = 20;
    public LayerMask FireMask;
    public GameObject FireEffect;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    public bool Grounded { get; private set; }
    public bool FacingRight { get; private set; }
    public bool AimingRight { get; private set; }

    private Vector2 _input;
    private bool _jump;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        StartCoroutine("PatrolRoutine");
        StartCoroutine("AttackRoutine");
    }

    IEnumerator PatrolRoutine()
    {
        var patrolLeft = Random.value > 0.5f;

        _input = Vector2.zero;

        while (true)
        {
            var wait = Random.Range(PatrolWaitInterval.x, PatrolWaitInterval.y);
            yield return new WaitForSeconds(wait);

            patrolLeft = !patrolLeft;
            _input.x = patrolLeft ? -1 : 1;

            var walk = Random.Range(PatrolWalkInterval.x, PatrolWalkInterval.y);
            yield return new WaitForSeconds(walk);
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);

            var hit = Physics2D.Raycast(Torso.position,
                FacingRight ? Vector2.right : new Vector2(-1, 0), FireRange, FireMask);

            if (!hit.collider)
                continue;

            Fire();
        }
    }

    /** Fire a projectile from the emitter. */
    void Fire()
    {

        // Create new projectile.
        var projectile = Instantiate(FireProjectile, FireEmitter.position, FireEmitter.rotation) as GameObject;

        // Give projectile its initial velocity.
        var r = projectile.GetComponent<Rigidbody2D>();
        if (r == null)
            return;

        var speed = Random.Range(FireSpeedRange.x, FireSpeedRange.y);
        var direction = (FireEmitter.TransformPoint(Vector3.right) - FireEmitter.position).normalized;
        r.velocity = (direction * speed);

        // Play fire effect.
        var parent = FireEmitter.transform;
        var effect = Instantiate(FireEffect, parent.position, parent.rotation) as GameObject;
        effect.transform.parent = parent.transform;
    }

    void FixedUpdate()
    {
        UpdateGrounded();
        UpdateMovement();
    }

    void UpdateGrounded()
    {
        // Check if robot is grounded.
        Vector2 p = transform.position;
        var a = p - GroundArea * 0.5f;
        var b = p + GroundArea * 0.5f;
        var hit = Physics2D.OverlapArea(a, b, GroundMask);
        Grounded = hit != null;
    }

    void UpdateMovement()
    {
        // Get input force scaling factor.
        var scale = Grounded ? InputForceScale : AirborneForceScale;

        // Get robot's inputs.
        var dx = _input.x * scale.x;
        var dy = _input.y * scale.y;

        // Ajust robot's drag depending on whether they wish to move or not.
        _rigidbody2D.drag = Mathf.Approximately(dx, 0) ? IdleDrag : MovingDrag;

        // Update robot's movement force from inputs.
        var velocity = _rigidbody2D.velocity;
        var speed = velocity.magnitude;
        if (Math.Sign(dx) != Math.Sign(velocity.x) || speed < MaxSpeed)
            _rigidbody2D.AddForce(new Vector2(dx, dy));

        // Update animator state.
        // var runSpeed = _grounded ? Input.GetAxis("Horizontal") : 0;
        _animator.SetFloat("RunSpeed", Input.GetAxis("Horizontal"));

        // Update robot's overall facing.
        if (Mathf.Abs(velocity.x) < 0.25f)
            FacingRight = AimingRight;
        else
            FacingRight = velocity.x >= 0;

        // Update pelvis scale to match facing.
        Torso.localScale = FacingRight ? new Vector3(-1, 1, 1) : Vector3.one;
    }

    void Footstep()
    {
        if (!Grounded)
            return;

        var effect = Instantiate(FootstepEffect, transform.position, transform.rotation) as GameObject;
        effect.transform.parent = transform.transform;
    }
}
