using UnityEngine;
using System.Collections;

public class RandomClip : MonoBehaviour {

	public AudioClip[] Clips;

	public float Probability = 1.0f;
    public float Volume = 1;

    private AudioSource _audio;

    void Awake()
    { _audio = GetComponent<AudioSource>(); }

	void OnEnable() 
	{
		if (Random.value > Probability)
			return;

		var index = Random.Range(0, Clips.Length);
		_audio.PlayOneShot(Clips[index], Volume);
	}
	
}
