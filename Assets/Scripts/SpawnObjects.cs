using UnityEngine;
using System.Collections;

public class SpawnObjects : MonoBehaviour 
{

    public Bounds Extents;

    public int MinCount;
    public int MaxCount = 10;

    public GameObject[] Prefabs;

    public Vector2 ScaleRange = Vector2.one;

	void Start () 
    {

        int n = Random.Range(MinCount, MaxCount);
        for (var i = 0; i < n; i++)
            Spawn();

	}

    void Spawn()
    {
        var prefab = Prefabs[Random.Range(0, Prefabs.Length)];

        var min = Extents.min;
        var max = Extents.max;

        var x = Random.Range(min.x, max.x);
        var y = Random.Range(min.y, max.y);
        var z = Random.Range(min.z, max.z);

        var p = new Vector3(x, y, z);
        var r = Quaternion.Euler(0, 0, Random.Range(0, 360));
        var go = Instantiate(prefab, p, r) as GameObject;

        var s = Random.Range(ScaleRange.x, ScaleRange.y);
        go.transform.localScale = new Vector3(s, s, s);
        go.transform.parent = transform;
    }
	
}
