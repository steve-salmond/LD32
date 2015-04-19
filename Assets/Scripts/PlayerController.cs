using System;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    public Transform Pelvis;
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

    public float Health
    { get { return _damageable.Health; } }

    public Gun Gun
    { get { return _gun; } }

    private Camera _camera;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Damageable _damageable;
    private Gun _gun;

    public bool Grounded { get; private set; }
    public bool FacingRight { get; private set; }
    public bool AimingRight { get; private set; }

    private bool _jump;

    private Plane _worldPlane = new Plane(Vector3.back, 0);

	void Start()
	{
        _camera = CameraController.Instance.Camera;
	    _rigidbody2D = GetComponent<Rigidbody2D>();
	    _animator = GetComponent<Animator>();
	    _damageable = GetComponent<Damageable>();
	    _gun = GetComponentInChildren<Gun>();
	}

    void Update()
    {
        // Detect when player wishes to jump.
        // Do this in Update, since it can be missed in FixedUpdate().
        _jump = Input.GetButtonDown("Jump");
        UpdateJump();
    }

    void FixedUpdate()
    {
        UpdateGrounded();
        UpdateMovement();
        UpdateJump();
        UpdateAim();
    }

    void UpdateGrounded()
    {
        // Check if player is grounded.
        Vector2 p = transform.position;
        var a = p - GroundArea * 0.5f;
        var b = p + GroundArea * 0.5f;
        var hit = Physics2D.OverlapArea(a, b, GroundMask);
        Grounded = hit != null;
    }

    void UpdateMovement()
    {
        // Check if player is dead.
        if (Health <= 0)
            return;

        // Get input force scaling factor.
        var scale = Grounded ? InputForceScale : AirborneForceScale;

        // Get player's inputs.
        var dx = Input.GetAxisRaw("Horizontal") * scale.x;
        var dy = Input.GetAxisRaw("Vertical") * scale.y;

        // Ajust player's drag depending on whether they wish to move or not.
        _rigidbody2D.drag = Mathf.Approximately(dx, 0) ? IdleDrag : MovingDrag;

        // Update player's movement force from inputs.
        var velocity = _rigidbody2D.velocity;
        var speed = velocity.magnitude;
        if (Math.Sign(dx) != Math.Sign(velocity.x) || speed < MaxSpeed)
            _rigidbody2D.AddForce(new Vector2(dx, dy));

        // Update animator state.
        // var runSpeed = _grounded ? Input.GetAxis("Horizontal") : 0;
        _animator.SetFloat("RunSpeed", Input.GetAxis("Horizontal"));

        // Update player's overall facing.
        if (Mathf.Abs(velocity.x) < 0.25f)
            FacingRight = AimingRight;
        else 
            FacingRight = velocity.x >= 0;

        // Update pelvis scale to match facing.
        Pelvis.localScale = FacingRight ? Vector3.one : new Vector3(-1, 1, 1);
    }

    void UpdateJump()
    {
        // Check if player is dead.
        if (Health <= 0)
            return;

        // Check if player wishes to jump and is grounded.
        if (!_jump || !Grounded)
            return;

        // Apply jump force.
        _rigidbody2D.AddForce(Vector2.up * JumpForce);

        // Inform animator that jump has occurred.
        _animator.SetTrigger("Jump");

        // Play jump effect
        var effect = Instantiate(JumpEffect, transform.position, transform.rotation) as GameObject;
        effect.transform.parent = transform.transform;

        // Don't jump until next frame.
        _jump = false;
    }

    void UpdateAim()
    {
        // Check if player is dead.
        if (Health <= 0)
            return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var d = 0.0f;
        _worldPlane.Raycast(ray, out d);
        

        // Get mouse's angle relative to player.
        Vector2 mouse = ray.GetPoint(d); // _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pelvis = Pelvis.position;
        var delta = mouse - pelvis;
        var angle = Vector2.Angle(delta, Vector2.up);
        var flip = FacingRight ? (delta.x >= 0) : (delta.x < 0);

        // Determine if player is aiming to the right or left.
        AimingRight = delta.x >= 0;

        // Update player's torso to point gun at mouse.
        Torso.localRotation = Quaternion.Euler(0, 0, flip ? 90 - angle : angle - 90);
        Torso.localScale = flip ? Vector3.one : new Vector3(-1, 1, 1);

        CameraController.Instance.AimOffset = delta.normalized * AimOffsetScale;
    }

    void Footstep()
    {
        if (!Grounded)
            return;
        
        var effect = Instantiate(FootstepEffect, transform.position, transform.rotation) as GameObject;
        effect.transform.parent = transform.transform;
    }
}
