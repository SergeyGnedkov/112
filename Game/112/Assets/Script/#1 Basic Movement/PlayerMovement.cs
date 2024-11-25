using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool moving = true;
    float speed = 5.0f;
    
    void Start ()
    {

    }

    void Update ()
    {
        if (moving == true)
        {
            movement();
        }
        movementCheck();
    }
         
    public void setMoving(bool val)
    {
        moving = val;
    }

    void movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate (Vector3.up * speed * Time.deltaTime,Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
        }



    }
    void movementCheck()
    {
        if (Input.GetKey(KeyCode.D) != true && Input.GetKey(KeyCode.A) != true && Input.GetKey(KeyCode.S) != true && Input.GetKey(KeyCode.W) != true)
        {
            moving = false;
        }
        else
        {
            moving = true;
        }
    }
}