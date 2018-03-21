using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public static int year;
    public static readonly float TIME_PER_YEAR = 60f;

    float timer;

    // Use this for initialization
    void Start () {
        timer = 0;
        year = 1;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if(timer > TIME_PER_YEAR)
        {
            year++;
            timer = 0;
        }
	}
}
