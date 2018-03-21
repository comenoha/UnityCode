using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Group
{
    public City(Floop[] _initFloops, HexCell _center) : base(_initFloops, _center)
    {
        group = GROUPLIST.CITY;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {

    }
}
