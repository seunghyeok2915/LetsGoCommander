using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    public GameObject _mapPrefab;

    public void GenerateNavmesh()
    {
        NavMeshSurface[] surfaces = gameObject.GetComponentsInChildren<NavMeshSurface>();

        foreach (var s in surfaces)
        {
            s.RemoveData();
            s.BuildNavMesh();
        }
    }
}