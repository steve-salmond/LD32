using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthLabel : MonoBehaviour
{

    private Text _text;

	void Start ()
	{
	    _text = GetComponent<Text>();
	}
	
	void Update ()
	{
	    var health = Mathf.RoundToInt(PlayerController.Instance.Health);
	    _text.text = string.Format("{0:D3}", health);
	}
}
