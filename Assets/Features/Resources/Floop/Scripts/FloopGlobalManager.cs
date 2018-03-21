using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloopGlobalManager : MonoBehaviour {

    public Floop floopPrefab;

    [Range(0f, 0.01f)]
    public float initialFloopSpawnRate = 0;

    public int maxFloops;

    public Tent tent;

    List<Floop> floops;

    public List<STATE> TODO;

    public List<Group> groups;


    void Start () {
        floops = new List<Floop>();
        TODO = new List<STATE>();

        maxFloops = 2;

        TODO.Add(STATE.BUILD);

        initSpawn();

        groups = new List<Group>();
        initGroupAssign();
    }

    //instantiates floops and gives them their birth properties

    void initSpawn()
    {
        for (int i = 0; i < GridFactory.cells.Length; i++)
        {
            HexCell cell = GridFactory.cells[i];
            if (cell.type == BIOME.forest && Random.Range(0f, 1f) <= initialFloopSpawnRate && floops.Count < maxFloops)
            {
                // creates floops - adds to master list - assigns gender and name
                floops.Add(spawnFloop(cell));

                Floop tempFloop = floops[floops.Count - 1];

                tempFloop.setState(STATE.WANDER);

                tempFloop.gender = (floops.Count == 1) ? (GENDER) 0 : (GENDER) 1;

                tempFloop.name = tempFloop.gender == GENDER.MALE ? (NAME)Random.Range(0, 30) : (NAME)Random.Range(31, 60);
            }
        }
        if (floops.Count < maxFloops) initSpawn();
    }

    //creates first group

    void initGroupAssign()
    {
        Group initialGroup = new InitialWanderGroup(floops.ToArray(), null);
        groups.Add(initialGroup);
    }
	
    //function to spawn floop

    Floop spawnFloop(HexCell cell)
    {
        Vector3 floopPos = new Vector3(cell.transform.position.x, cell.transform.position.y + (transform.localScale.y / 2 + 0.2f), cell.transform.position.z);
        return Instantiate<Floop>(floopPrefab, floopPos, Quaternion.identity);
    }

	// Update is called once per frame

	void Update () {
        foreach(Group group in groups.ToList())
        {
            group.Update();

            if (group.readyForAdvance)
            {
                Debug.Log("Advancing!!");
                groups.Remove(group);
                Group newGroup;
                Floop[] floopsToAdd= group.floops.ToArray();

                if (group.group == GROUPLIST.INITIALWANDER) newGroup = new AdamEve(floopsToAdd, floopsToAdd[0].getCurrentCell());
                else if (group.group == GROUPLIST.ADAMEVE) newGroup = new Camp(floopsToAdd, floopsToAdd[0].getCurrentCell());
                else if (group.group == GROUPLIST.CAMP) newGroup = new AdamEve(floopsToAdd, floopsToAdd[0].getCurrentCell());
                else newGroup = null;


                groups.Add(newGroup);
            }
        }
    }

    //starts traveling to designated cell

    void move(Floop floop, HexCell cell)
    {
        floop.travelTo(cell);
    }

    //begins building process - instantiates building, sets it as the floops target, and sets all perameters

    void startBuild(Floop floop, HexCell cell, BUILDINGTYPE type)
    {
        House_Blueprint h = floop.targetBlueprint = Instantiate<House_Blueprint>(((GameObject) Resources.Load("Structure/Prefabs/Crane")).GetComponent<House_Blueprint>());

        h.gameObject.transform.position = cell.transform.position;

        h.effortNeeded = BUILDINGEFFORT.TENT;
        h.complete = false;
        h.cell = cell;

        floop.setState(STATE.BUILD);
    }
}
