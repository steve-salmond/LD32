using UnityEngine;
using System.Collections;

public class RandomPitch : MonoBehaviour {

	public float MinPitch;
	public float MaxPitch;


	void Start () {
		GetComponent<AudioSource>().pitch = Random.Range(MinPitch, MaxPitch);
	}

}
