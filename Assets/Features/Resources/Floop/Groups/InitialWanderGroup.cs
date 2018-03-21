using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialWanderGroup : Group{

    public static readonly float DISTANCE_OF_SIGHT = 1000;

    public InitialWanderGroup(Floop[] _initFloops, HexCell _center) : base(_initFloops, _center)
    {
        //empty
        group = GROUPLIST.INITIALWANDER;
    }
	
	// Update is called once per frame
	override public void Update () {
        if (Vector3.Distance(floops[0].transform.position, floops[1].transform.position) < DISTANCE_OF_SIGHT)
        {
            floops[0].travelTo(floops[0].getCurrentCell());
            floops[1].travelTo(floops[0].getCurrentCell());
            if(floops[0].getCurrentCell() == floops[1].getCurrentCell())
            {
                readyForAdvance = true;
            }
        }else
        {
            foreach (Floop floop in floops)
            {
                if (floop.readyForTask)
                {
                    wanderAround(floop);
                }
            }
        }
	}

    void wanderAround(Floop floop)
    {
        //Debug.Log(floop.name + " is wandering to a new position again!");
        floop.travelTo(GridFactory.getRandomWalkableCell());
    }
}
