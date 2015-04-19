using UnityEngine;
using System.Collections;

public class RandomBob : MonoBehaviour 
{

    public Vector2 DepthRange = new Vector2(0, 10);
    public Vector2 HeightRange = new Vector2(0, 1);
    public Vector2 BobSpeedRange = new Vector2(1, 5);
    public Vector2 RotationSpeedRange = new Vector2(-1, 1);

    private float _height;
    private float _bobSpeed;
    private float _rotationSpeed;

    private Vector3 _initialPosition;

	void Start ()
	{
	    var depth = Random.Range(DepthRange.x, DepthRange.y);
        var p = transform.position;
        _initialPosition = new Vector3(p.x, p.y, p.z + depth);
	    transform.position = _initialPosition;

        _height = Random.Range(HeightRange.x, HeightRange.y);
        _bobSpeed = Random.Range(BobSpeedRange.x, BobSpeedRange.y);
        _rotationSpeed = Random.Range(RotationSpeedRange.x, RotationSpeedRange.y);
	}
	
	void Update ()
	{
	    var p = _initialPosition;
        var y = p.y + _height * Mathf.Sin(Time.time * _bobSpeed);
        transform.position = new Vector3(p.x, y, p.z);
        transform.Rotate(0, 0, Time.deltaTime * _rotationSpeed);
	}
}
