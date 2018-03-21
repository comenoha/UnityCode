using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Floop : MonoBehaviour {

    public int X;
    public int Y;
    public GENDER gender;
    public NAME name;

    public Vector3[] corners;

    public HexCell targetCell;

    public House_Blueprint targetBlueprint;

    public Vector3 wanderPoint;

    public float timer;
    public float waitTime;
    public float speed;
    float travelSpeed;

    public STATE state;

    float idleTimer;
    float randomTimeBeforeFindTask;
    float minTimeBeforeFindTask;
    float maxTimeBeforeFindTask;

    public bool readyForTask;
    public bool readyForProcreation;

    public float chanceToProcreate;
    public float procreationTimer;
    public float procreationCooldown;

    public int age;
    public int birthYear;

    public int maxWater;
    public int water;

    private Group currentGroup;

    //initializes all variables
    void Start()
    {
        corners = new Vector3[6];
        corners = getCorners();

        //random variable declarations

        idleTimer = 0;
        minTimeBeforeFindTask = 6;
        maxTimeBeforeFindTask = 8;
        randomTimeBeforeFindTask = Random.Range(minTimeBeforeFindTask, maxTimeBeforeFindTask);

        timer = 0;
        waitTime = 2f;
        speed = 5f;
        travelSpeed = 15f;

        wanderPoint = newWanderPoint();

        readyForTask = false;

        readyForProcreation = true;
        chanceToProcreate = .22f;
        procreationTimer = 0;
        procreationCooldown = 60;
        

        age = 0;
        birthYear = TimeManager.year;

        maxWater = 50000;
        water = maxWater;
    }

    //runs a raycast and returns current cell

    public HexCell getCurrentCell()
    {
        HexCell currentCell = null;

        RaycastHit hit;

        int layerMask = LayerMask.GetMask("HexCell");
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, 3f, layerMask))
        {
            currentCell = hit.transform.GetComponent<HexCell>();
        }

        return currentCell;
    }

    //gets corners of current cell for use by wander function

    Vector3[] getCorners()
    {
        Vector3[] localCorners = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            localCorners[i] = getCurrentCell().transform.position + HexMetrics.corners[i];
        }

        return localCorners;
    }

    //aimlessly wander

    void wander()
    {
        float distance = Vector3.Distance(transform.position, wanderPoint);

        int randNum = Random.Range(0, 501);

        if (onPoint(wanderPoint) || randNum < 10 - distance) {
            wanderPoint = newWanderPoint();
            state = STATE.WAIT;
            speed = Random.Range(.6f, 1.6f);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, wanderPoint, speed * Time.deltaTime);
    }

    //gets new random point in cell

    private Vector3 newWanderPoint()
    {
        return corners[Random.Range(0, 6)];
    }

    //takes in cell, computes straight route and moves toward cell - DO NOT CALL EXPLICITLY, IS CALLED IN TRAVELTO FUNCTION

    void moveTowardCell(HexCell target)
    {
        if (target == null) return;
        if (getCurrentCell() == target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, travelSpeed * Time.deltaTime);
        }
        else if (getCurrentCell().X < target.X && GridFactory.getCell(getCurrentCell().X + 1, getCurrentCell().Y) != null && GridFactory.getCell(getCurrentCell().X + 1, getCurrentCell().Y).walkable == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GridFactory.getCell(getCurrentCell().X + 1, getCurrentCell().Y).transform.position, travelSpeed * Time.deltaTime);
        }
        else if (getCurrentCell().X > target.X && GridFactory.getCell(getCurrentCell().X - 1, getCurrentCell().Y) != null && GridFactory.getCell(getCurrentCell().X - 1, getCurrentCell().Y).walkable == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GridFactory.getCell(getCurrentCell().X - 1, getCurrentCell().Y).transform.position, travelSpeed * Time.deltaTime);
        }
        else if (getCurrentCell().Y < target.Y && GridFactory.getCell(getCurrentCell().X, getCurrentCell().Y + 1) != null && GridFactory.getCell(getCurrentCell().X + 1, getCurrentCell().Y).walkable == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GridFactory.getCell(getCurrentCell().X, getCurrentCell().Y + 1).transform.position, travelSpeed * Time.deltaTime);
        }
        else if (getCurrentCell().Y > target.Y && GridFactory.getCell(getCurrentCell().X, getCurrentCell().Y - 1) != null && GridFactory.getCell(getCurrentCell().X - 1, getCurrentCell().Y).walkable == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GridFactory.getCell(getCurrentCell().X, getCurrentCell().Y - 1).transform.position, travelSpeed * Time.deltaTime);
        }

        
    }

    //checks if object is near target

    bool onPoint(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) < 2f;
    }

    //takes position, moves toward position in space - Discontinued

    /*void moveToward(Vector3 position, float speed)
    {
        float velX = 0;
        float velY = 0;
        float velZ = 0;

        if(transform.position.x < position.x)
        {
            velX = speed;
        }else if (transform.position.x > position.x)
        {
            velX = -speed;
        }

        if (transform.position.z < position.z)
        {
            velZ = speed;
        }else if (transform.position.z > position.z)
        {
            velZ = -speed;
        }

        rb.velocity = new Vector3(velX, velY, velZ);
    }*/

    //takes cell, moves floop toward that cell by using its pathfinding

    public void travelTo(HexCell target)
    {
        if (targetCell != target)
        {
            targetCell = target;
            state = STATE.TRAVEL;
        }else
        {
            return;
        }
    }

    //sets state to wander

    public void setWander()
    {
        wanderPoint = newWanderPoint();
        state = STATE.WANDER;
    }

    //builds things

    public void build(House_Blueprint structure)
    {
        if (state != STATE.BUILD) state = STATE.BUILD;
        targetBlueprint = structure;

        //checks if structure is done being built. If so build structure and remove blueprint.

        if (structure.isComplete())
        {
            Tent tent = Instantiate<Tent>(((GameObject)Resources.Load("Structure/Prefabs/TentObject")).GetComponent<Tent>());

            tent.transform.position = structure.transform.position;

            //destroy blueprint
            structure.destroyBlueprint();

            //reset state
            state = STATE.WANDER;

            //add to floop's group's housingspace
            if (this.currentGroup != null) currentGroup.addHousing(BUILDINGHOUSINGAMOUNT.TENT);
            return;
        }

        //moves to structure before starting to build

        if (!structure.cell.Equals(getCurrentCell()))
        {
            Debug.Log("Moving to new Cell");
            moveTowardCell(structure.cell);
        }
        else
        {
            if (Vector3.Distance(transform.position, structure.gameObject.transform.position) <= 2)
            {
                structure.build(1);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, structure.gameObject.transform.position, travelSpeed * Time.deltaTime);
            }
        }

        
    }

    //places structure blueprint

    public House_Blueprint placeBlueprint(BUILDINGTYPE type)
    {
        House_Blueprint bp = Instantiate<House_Blueprint>(((GameObject)Resources.Load("Structure/Prefabs/Crane")).GetComponent<House_Blueprint>());
        if (type == BUILDINGTYPE.TENT)
        {
            bp.name = "Tent Blueprint";
            bp.effortNeeded = BUILDINGEFFORT.TENT;
            bp.transform.position = new Vector3(Random.Range(getCurrentCell().transform.position.x - HexMetrics.innerRadius, getCurrentCell().transform.position.x + HexMetrics.innerRadius), 0, Random.Range(getCurrentCell().transform.position.z - HexMetrics.innerRadius, getCurrentCell().transform.position.z + HexMetrics.innerRadius));
            bp.cell = getCurrentCell();
        }
        return bp;
    }

    //runs timer and things

    void runIdleTimer()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer > randomTimeBeforeFindTask)
        {
            idleTimer = 0;
            randomTimeBeforeFindTask = Random.Range(minTimeBeforeFindTask, maxTimeBeforeFindTask);

            readyForTask = true;
        }
    }

    //procreation random amount

    public bool procreationChance()
    {
        return (Random.Range(0, 1f) < chanceToProcreate);
    }

    //resets procreation timer

    public void resetProcreationTimer()
    {
        procreationTimer = 0;
        readyForProcreation = false;
        setWander();
    }

    //resets with specific amount

    public void resetProcreationTimer(float amount)
    {
        procreationTimer = amount;
        readyForProcreation = false;
        setWander();
    }

    //Spawns new floop

    public Floop procreateFloop()
    {
        return Instantiate<Floop>(this);
    }

    //dies
    public void die()
    {
        Debug.Log(name + " has died");
        //Destroy(this.gameObject);
    }

    //runs every frame - mostly handles states, coroutines, and its timer

    void Update()
    {
        corners = getCorners();

        //Timers
        if(state == STATE.WAIT || state == STATE.WANDER)
        {
            runIdleTimer();
            procreationTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0;
            readyForTask = false;
            procreationTimer = 0;
        }
        
        if(procreationTimer > procreationCooldown)
        {
            readyForProcreation = true;
        }
        else
        {
            readyForProcreation = false;
        }



        //STATE MANAGER
        if (state == STATE.WANDER)
        {
            StopAllCoroutines();
            StartCoroutine("wander");
        }
        else if (state == STATE.WAIT)
        {
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                state = STATE.WANDER;
                timer = 0;
                waitTime = Random.Range(1f, 5f);
            }
        }
        else if (state == STATE.TRAVEL)
        {
            if (onPoint(targetCell.transform.position))
            {
                setWander();
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine("moveTowardCell", targetCell);
            }
        }
        else if (state == STATE.BUILD)
        {
            //StopAllCoroutines();
            if (targetBlueprint != null)
            {
                StartCoroutine("build", targetBlueprint);
            }
            else
            {
                Debug.Log("There is no target building - Switching " + name + " to default wander");
                state = STATE.WANDER;
            }
        }
        else if(state == STATE.PROCREATE)
        {
            StopAllCoroutines();
        }

        //age manager
        age = TimeManager.year - birthYear;
        if(age >= 10)
        {
            die();
        }

        water--;

        if (water <= 0) die();
    }

    //getters and setters for state

    public STATE getState()
    {
        return state;
    }

    public void setState(STATE s)
    {
        state = s;
    }

    public void setGroup(Group newGroup)
    {
        currentGroup = newGroup;
        newGroup.housingNeed++;
    }
}

