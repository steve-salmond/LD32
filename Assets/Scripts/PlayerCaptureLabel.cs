using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCaptureLabel : MonoBehaviour
{

    private Text _text;

	void Start ()
	{
	    _text = GetComponent<Text>();
	}
	
	void Update ()
	{
        var gun = PlayerController.Instance.Gun;
        _text.text = string.Format("{0:D2}", gun.CaptureCount);
	}
}
