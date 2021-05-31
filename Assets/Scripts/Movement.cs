using System;
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Joystick joystick;
    public float movementSpeed;
    public Vector3 dir = Vector3.zero;
    public float moveSpeedParameter = 0;

    private void Update()
    {
        dir = new Vector3(joystick.Horizontal,0,joystick.Vertical).normalized;
        Move();
    }

    void Move()
    {
        if (dir == Vector3.zero) return;
        transform.position += dir * (movementSpeed * Time.deltaTime);
    }
}
