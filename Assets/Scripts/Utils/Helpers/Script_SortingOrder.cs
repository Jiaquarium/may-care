using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SortingOrder : MonoBehaviour
{
    public int defaultSortingOrder;
    public int sortingOrderBase;
    public int offset;
    public bool runOnlyOnce;
    public bool sortingOrderIsAxisZ = true;
    
    private Renderer r;

    void Update()
    {
        if (r == null)    r = GetComponent<Renderer>();
        
        if (sortingOrderIsAxisZ)
        {
            r.sortingOrder = (int)(((sortingOrderBase - transform.position.z) * 10) + offset);
        }
        else
        {
            r.sortingOrder = (int)(((sortingOrderBase + transform.position.x) * 10) + offset);
        }

        if (runOnlyOnce)
        {
            Destroy(this);
        }
    }

    public void EnableWithOffset(int _offset, bool isAxisZ)
    {
        this.enabled = true;
        offset = _offset;
        sortingOrderIsAxisZ = isAxisZ;
    }

    public void DefaultSortingOrder()
    {
        r = GetComponent<Renderer>();
        r.sortingOrder = defaultSortingOrder;
        this.enabled = false;
    }
}
