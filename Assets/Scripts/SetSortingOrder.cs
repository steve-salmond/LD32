using UnityEngine;

public class SetSortingOrder : MonoBehaviour
{
    public string SortingLayer;
    public int SortingOrder;

    void Start()
    {
        var r = GetComponent<Renderer>();
        if (r == null)
            return;

        if (!string.IsNullOrEmpty(SortingLayer))
            r.sortingLayerName = SortingLayer;

        r.sortingOrder = SortingOrder;
    }

}
