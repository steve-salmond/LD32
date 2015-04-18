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

    private Camera _camera;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    private bool _grounded;
    private bool _facingRight = true;
    private bool _aimingRight = true;

    private bool _jump;

    private Plane _worldPlane = new Plane(Vector3.back, 0);

	void Start()
	{
        _camera = CameraController.Instance.camera;
	    _rigidbody2D = GetComponent<Rigidbody2D>();
	    _animator = GetComponent<Animator>();
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
        _grounded = hit != null;
    }

    void UpdateMovement()
    {
        // Get input force scaling factor.
        var scale = _grounded ? InputForceScale : AirborneForceScale;

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
        if (Mathf.Abs(velocity.x) < 0.1f)
            _facingRight = _aimingRight;
        else 
            _facingRight = velocity.x >= 0;

        // Update pelvis scale to match facing.
        Pelvis.localScale = _facingRight ? Vector3.one : new Vector3(-1, 1, 1);
    }

    void UpdateJump()
    {
        // Check if player wishes to jump and is grounded.
        if (!_jump || !_grounded)
            return;

        // Apply jump force.
        _rigidbody2D.AddForce(Vector2.up * JumpForce);

        // Inform animator that jump has occurred.
        _animator.SetTrigger("Jump");

        // Don't jump until next frame.
        _jump = false;
    }

    void UpdateAim()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var d = 0.0f;
        _worldPlane.Raycast(ray, out d);
        

        // Get mouse's angle relative to player.
        Vector2 mouse = ray.GetPoint(d); // _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pelvis = Pelvis.position;
        var delta = mouse - pelvis;
        var angle = Vector2.Angle(delta, Vector2.up);
        var flip = _facingRight ? (delta.x >= 0) : (delta.x < 0);

        // Determine if player is aiming to the right or left.
        _aimingRight = delta.x >= 0;

        // Update player's torso to point gun at mouse.
        Torso.localRotation = Quaternion.Euler(0, 0, flip ? 90 - angle : angle - 90);
        Torso.localScale = flip ? Vector3.one : new Vector3(-1, 1, 1);

        CameraController.Instance.AimOffset = delta.normalized * AimOffsetScale;
    }
}
