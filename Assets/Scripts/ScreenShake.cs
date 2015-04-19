using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{

    public float Strength = 1;
    public float Attack = 0;
    public float Decay = 0.2f;

	void Start()
	{ CameraController.Instance.Shake(Strength, Attack, Decay); }
}
