using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour
{
    public LayerMask BreakMask;

    public float BreakSpeed = 1;

    public string BrokenLayer;

    public GameObject BreakEffect;

    public float HitSpeed = 1;
    public GameObject HitEffect;


    private HingeJoint2D _joint;

	void Start ()
	{
	    _joint = GetComponent<HingeJoint2D>();
	}
	
    void OnCollisionEnter2D(Collision2D collision)
    {
        var go = collision.gameObject;
        if ((BreakMask.value & 1 << go.layer) == 0)
            return;

        var deltaV = collision.relativeVelocity.magnitude;
        if (deltaV > HitSpeed && deltaV < BreakSpeed)
            Instantiate(HitEffect, collision.contacts[0].point, Quaternion.identity);

        if (deltaV < BreakSpeed)
            return;

        gameObject.layer = LayerMask.NameToLayer(BrokenLayer);

        if (_joint)
            Destroy(_joint);

        Destroy(this);

        Instantiate(BreakEffect, collision.contacts[0].point, Quaternion.identity);

    }
}
