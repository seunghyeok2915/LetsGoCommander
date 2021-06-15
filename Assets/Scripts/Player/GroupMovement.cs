using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMovement : MonoBehaviour
{
    public Joystick joystick;

    public float moveSpeed;

    public static Vector3 JoyStickDirection;
    bool bOnBossAttack = false;

    private void Update()
    {
        if (bOnBossAttack)
            JoyStickDirection = Vector3.zero;
        else
            JoyStickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        Move();
    }

    public void SetDirctionZero()
    {
        bOnBossAttack = true;
    }

    void Move()
    {
        if (JoyStickDirection == Vector3.zero) return;

        transform.position += JoyStickDirection * (moveSpeed * Time.deltaTime);
    }
}
