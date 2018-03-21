using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Blueprint : MonoBehaviour {

    public Vector3 position;

    private int totalEffort;

    public int effortNeeded;

    public bool complete;
    
    public BUILDINGTYPE buildingType;

    public HexCell cell;

    private Vector3 pos;

    //puts effort into the building

    public void build(int _effort)
    { 
        totalEffort += _effort;

        if(totalEffort >= effortNeeded)
        {
            complete = true;
        }
    }

    public bool isComplete()
    {
        return complete;
    }

    public void destroyBlueprint()
    {
        Destroy(gameObject);
    }
}
