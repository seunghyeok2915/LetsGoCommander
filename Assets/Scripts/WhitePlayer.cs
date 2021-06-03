using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePlayer : MonoBehaviour
{
    private GroupManager groupManager;

    private void Start()
    {
        groupManager = FindObjectOfType<GroupManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            groupManager.MakeSoldier(transform);
            gameObject.SetActive(false);
        }
    }
}
