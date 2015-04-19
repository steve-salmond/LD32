using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerRecycleLabel : MonoBehaviour
{

    private Text _text;

	void Start ()
	{
	    _text = GetComponent<Text>();
	}
	
	void Update ()
	{
        var recycles = GameManager.Instance.Recycles;
        _text.text = string.Format("{0:D3}", recycles);
	}
}
