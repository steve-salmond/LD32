using UnityEngine;

public class DetonateOnContact : MonoBehaviour 
{

    public LayerMask DetonationMask;

    public float Strength = 100;

    public float Radius = 1;

    private Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

	void OnCollisionEnter2D(Collision2D collision)
    {
        var go = collision.gameObject;
        if ((DetonationMask.value & 1 << go.layer) == 0)
            return;

        Detonate();
        Destroy(this);
    }

    void Detonate()
    {
        var colliders = Physics2D.OverlapCircleAll(_rigidbody.position, Radius, DetonationMask);
        foreach (var c in colliders)
        {
            var r = c.GetComponent<Rigidbody2D>();
            if (!r)
                continue;
            
            var delta = r.position - _rigidbody.position;
            var d = delta.magnitude;

            if (d <= 0.1f)
                continue;

            var f = (1 - Mathf.Clamp01(d/Radius)) * Strength;
            r.AddForce(delta.normalized * f);
        }
    }
}
