using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group{

    public List<Floop> floops;
    public HexCell center;

    public int housingSpace;
    public int housingNeed;

    public bool readyForAdvance;

    public GROUPLIST group;

    public Group(Floop[] _initFloops, HexCell _center)
    {
        readyForAdvance = false;
        housingSpace = 0;
        center = _center;

        floops = new List<Floop>();

        foreach (Floop floop in _initFloops)
        {
            floops.Add(floop);
            floop.setGroup(this); 
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
        
    }

    public void procreate(Floop male, Floop female)
    {
        if (male.getCurrentCell() == female.getCurrentCell())
        {
            male.setState(STATE.PROCREATE);
            female.setState(STATE.PROCREATE);
            if (Vector3.Distance(male.transform.position, female.transform.position) < 1f)
            {
                if(male.procreationChance() && female.procreationChance())
                {
                    Debug.Log("Procreation attempted...procreation successful!");

                    female.procreateFloop();

                    male.resetProcreationTimer(male.procreationCooldown * 20);
                    female.resetProcreationTimer(male.procreationCooldown * 20);
                }
                else
                {
                    Debug.Log("Procreation attempted...but failed");
                    male.resetProcreationTimer();
                    female.resetProcreationTimer();
                }
                
            }
            else
            {
                male.transform.position = Vector3.MoveTowards(male.transform.position, female.transform.position, male.speed * Time.deltaTime);
            }
        }
    }

    public void addHousing(int amount)
    {
        housingSpace += amount;
    }
}
