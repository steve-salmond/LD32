using System.Collections;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{

    public Transform Target;

    public Transform Eye;

    public Camera Camera;


    public float SmoothTime = 0.2f;

    public Vector3 Offset = new Vector3(0, 5, -10);
    public Vector3 AimOffset = new Vector3(0, 0, 0);
    public Vector2 Leading = new Vector3(1, 0.1f);
    
    private Vector3 _velocity = Vector3.zero;

    private float _shakeAttack;
    private float _shakeDecay;
    private float _shakeStrength;
    private float _targetShake;

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

    public void Shake(float strength, float attack, float decay)
    {
        _targetShake = strength;
        _shakeAttack = attack;
        _shakeDecay = decay;
        StopAllCoroutines();
        StartCoroutine("ShakeRoutine");
    }

    IEnumerator ShakeRoutine()
    {
        var c0 = _shakeStrength;
        var c1 = _targetShake;
        var wait = new WaitForEndOfFrame();

        // Fade up to target color.
        if (_shakeAttack > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + _shakeAttack;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                SetShake(Mathf.Lerp(c0, c1, f));
                yield return wait;
            }
        }

        // Now at target strength.
        SetShake(c1);

        // Fade down to black.
        if (_shakeDecay > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + _shakeDecay;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                SetShake(Mathf.Lerp(c1, 0, f));
                yield return wait;
            }
        }

        // Now fully black.
        SetShake(0);

    }

    void SetShake(float strength)
    {
        _shakeStrength = strength;
        Eye.localPosition = Random.insideUnitCircle * strength;
    }
}

