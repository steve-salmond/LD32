using UnityEngine;
using System.Collections;

public class RecycleBin : MonoBehaviour
{


    public LayerMask JunkMask;

    public string RecycleableTag;

    public GameObject RecycleEffect;
    public GameObject JunkEffect;

    void OnTriggerEnter2D(Collider2D collider)
    {
        var go = collider.gameObject;
        if ((JunkMask.value & 1 << go.layer) == 0)
            return;
        
        var t = go.transform;

        var recycleable = Equals(go.tag, RecycleableTag);

        if (recycleable)
        {
            GameManager.Instance.Recycle(go);
            Instantiate(RecycleEffect, t.position, t.rotation);
        }
        else
            Instantiate(JunkEffect, t.position, t.rotation);

        Destroy(go);
    }
}
