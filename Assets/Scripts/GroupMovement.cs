using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMovement : MonoBehaviour
{
    public Joystick joystick;

    public float movementSpeed;
    public float moveSpeedParameter = 0;

    public Vector3 dir = Vector3.zero;

    private void Update()
    {
        dir = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        moveSpeedParameter = dir.magnitude;

        Move();
    }

    void Move()
    {
        if (dir == Vector3.zero) return;

        transform.position += dir * (movementSpeed * Time.deltaTime);
    }
}
