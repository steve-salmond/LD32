using UnityEngine;

public class CameraController : Singleton<CameraController>
{

    public Transform Target;

    public float SmoothTime = 0.2f;

    public Vector3 Offset = new Vector3(0, 5, -10);
    public Vector3 AimOffset = new Vector3(0, 0, 0);
    public Vector2 Leading = new Vector3(1, 0.1f);
    
    private Vector3 _velocity = Vector3.zero;

    void Start()
    {
        Target = PlayerController.Instance.transform;
    }

    void FixedUpdate()
    {
        if (Target == null)
            return;

        var targetRigidbody = Target.GetComponent<Rigidbody2D>();
        var velocity = targetRigidbody != null ? targetRigidbody.velocity : Vector2.zero;
        var leadingOffset = new Vector3(velocity.x * Leading.x, velocity.y * Leading.y);

        transform.position = Vector3.SmoothDamp(transform.position,
            Target.position + Offset + AimOffset + leadingOffset, 
            ref _velocity, SmoothTime);
    }

}

