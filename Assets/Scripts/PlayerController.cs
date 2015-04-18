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
    public float GroundDistance = 0.1f;

    public float IdleDrag = 1;
    public float MovingDrag = 0;

    public float AimOffsetScale = 5;

    private Camera _camera;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    private bool _grounded;
    private bool _facingRight;

	void Start()
	{
        _camera = CameraController.Instance.camera;
	    _rigidbody2D = GetComponent<Rigidbody2D>();
	    _animator = GetComponent<Animator>();
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
        var hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), GroundDistance, GroundMask);
        _grounded = hit.collider != null;
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

        // Update pelvis facing.
        _facingRight = velocity.x >= 0;
        Pelvis.localScale = _facingRight ? Vector3.one : new Vector3(-1, 1, 1);
    }

    void UpdateJump()
    {
        // Check if player wishes to jump and is grounded.
        if (!Input.GetButtonDown("Jump") || !_grounded)
            return;

        // Apply jump force.
        _rigidbody2D.AddForce(Vector2.up * JumpForce);

        // Inform animator that jump has occurred.
        _animator.SetTrigger("Jump");
    }

    void UpdateAim()
    {
        // Get mouse's angle relative to player.
        Vector2 mouse = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pelvis = Pelvis.position;
        var delta = mouse - pelvis;
        var angle = Vector2.Angle(delta, Vector2.up);
        var aimingRight = _facingRight ? (delta.x >= 0) : (delta.x < 0);

        // Update player's torso to point gun at mouse.
        Torso.localRotation = Quaternion.Euler(0, 0, aimingRight ? 90 - angle : angle - 90);
        Torso.localScale = aimingRight ? Vector3.one : new Vector3(-1, 1, 1);

        CameraController.Instance.AimOffset = delta.normalized * AimOffsetScale;
    }
}
