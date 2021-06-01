using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    private void Update()
    {
        transform.position += Vector3.forward * (bulletSpeed * Time.deltaTime);
    }
}
