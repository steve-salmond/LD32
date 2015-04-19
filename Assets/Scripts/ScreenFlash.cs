using UnityEngine;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{

    public Color Color = Color.white;
    public float Attack = 0;
    public float Decay = 0.2f;

	void Start()
	{ ScreenFlashManager.Instance.Flash(Color, Attack, Decay); }
}
