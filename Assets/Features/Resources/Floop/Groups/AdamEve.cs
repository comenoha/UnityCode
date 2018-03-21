using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamEve : Group
{
    public AdamEve(Floop[] _initFloops, HexCell _center) : base(_initFloops, _center)
    {
        //empty
        group = GROUPLIST.ADAMEVE;
    }

    public override void Update()
    {
        base.Update();
        if(floops[0].readyForProcreation && floops[1].readyForProcreation)
        {
            procreate(floops[0], floops[1]);
        }

        if(housingSpace < 2)
        {
            foreach(Floop floop in floops)
            {
                if (floop.readyForTask)
                {
                    if(center.blueprints.Count == 0) center.blueprints.Add(floop.placeBlueprint(BUILDINGTYPE.TENT));
                    floop.build(center.blueprints[0]);
                }
            }
        }
    }
}
