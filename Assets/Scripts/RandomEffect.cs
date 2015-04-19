using UnityEngine;
using System.Collections;

public class RandomEffect : MonoBehaviour 
{

    public GameObject[] Prefabs;

    public Transform Emitter;
    public Vector2 DelayRange;

    public bool Repeat;
    public bool Reparent;
    public bool MatchPosition = true;
    public bool MatchRotation = true;

	void Start () 
    {
        if (!Emitter)
            Emitter = transform;

        StartCoroutine("UpdateRoutine");
    }
	
	IEnumerator UpdateRoutine()
    {
        while (Repeat)
        {
            var delay = Random.Range(DelayRange.x, DelayRange.y);
            yield return new WaitForSeconds(delay);

            var prefab = Prefabs[Random.Range(0, Prefabs.Length)];
            var p = MatchPosition ? Emitter.position : Vector3.zero;
            var r = MatchRotation ? Emitter.rotation : Quaternion.identity;
            var go = Instantiate(prefab, p, r) as GameObject;
            if (!go)
                continue;

            if (Reparent)
                go.transform.parent = Emitter;
        }
    }
}
