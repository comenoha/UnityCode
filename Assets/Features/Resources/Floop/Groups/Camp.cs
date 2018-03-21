using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : Group
{
    public Camp(Floop[] _initFloops, HexCell _center) : base(_initFloops, _center)
    {
        group = GROUPLIST.CAMP;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
		
	}
}
