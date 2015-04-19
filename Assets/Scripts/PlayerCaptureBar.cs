using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCaptureBar : MonoBehaviour 
{

    private Image _bar;

	void Start() 
    { _bar = GetComponent<Image>();	}
	
	void Update() 
    {
        var gun = PlayerController.Instance.Gun;
        var scale = gun.CaptureCount / (float) gun.CaptureCapacity;
        _bar.transform.localScale = new Vector3(scale, 1, 1);
	}
}
