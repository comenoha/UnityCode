using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    [Range(0, 100)]
    public int height = 60;
    [Range(0, 90)]
    public int rotationX = 30;
    [Range(0, 90)]
    public int rotationY = 0;
    [Range(0, 90)]
    public int rotationZ = 0;

    public float moveSpeed = 1f;
    // Use this for initialization
    void Start () {
        height = 60;

        rotationX = 30;

        moveSpeed = 3f;
    }

    // Update is called once per frame
    void Update () {
        transform.rotation = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));

        //moving

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if(h != 0 && v != 0)
        {
            h *= .8f;
            v *= .8f;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x + (h * moveSpeed), 20, 1000), height, Mathf.Clamp(transform.position.z + (v * moveSpeed), -50, 550));

        //height changing

        float d = Input.GetAxisRaw("Down");

        if (d == 1)
        {
            height -= 1;
        }
        else if (d == -1)
        {
            height += 1;
        }

        height = Mathf.Clamp(height, 10, 100);

        //camera move speed - uses equation with speed 4 at height 100 and speed 1.5 at height 10;

        moveSpeed = ((2.5f / 90f) * height) + (110f / 90f);
    }
}
