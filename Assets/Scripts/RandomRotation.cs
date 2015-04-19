using UnityEngine;
using System.Collections;

public class RandomRotation : MonoBehaviour
{

    public Vector3 Axis = Vector3.up;

    public Vector2 Range = new Vector2(0, 360);

	void OnEnable()
	{
	    var angle = Random.Range(Range.x, Range.y);
	    var r = Quaternion.AngleAxis(angle, Axis);
	    transform.localRotation = r;
	}
	
}
