using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class RandomBallGeneration : MonoBehaviour
{
    public Terrain _terrain; 
    public GameObject ballPrefab; 
    public int numberOfBalls = 10; 
    public float navMeshSampleDistance = 1f; 
    void Start()
    {
        SpawnBalls();
    }

    void SpawnBalls()
    {
        TerrainData terrainData = _terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPosition = _terrain.transform.position;

        int spawnedBalls = 0;

        if (!IsNavMeshAvailable())
            return;
        
        while (spawnedBalls < numberOfBalls)
        {
            Vector3 randomPosition = GetRandomTerrainPosition(terrainPosition, terrainSize);

            if (TryGetNavMeshPosition(randomPosition, out Vector3 validNavMeshPosition))
            {
                Instantiate(ballPrefab, validNavMeshPosition, Quaternion.identity);
                spawnedBalls++;
            }
        }
    }

    Vector3 GetRandomTerrainPosition(Vector3 terrainPosition, Vector3 terrainSize)
    {
        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);

        float y = _terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPosition.y;

        Vector3 randomTerrainPos = new Vector3(randomX, y, randomZ);

        return randomTerrainPos;
    }

    bool TryGetNavMeshPosition(Vector3 position, out Vector3 navMeshPosition)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            navMeshPosition = hit.position;
            return true;
        }

        navMeshPosition = Vector3.zero;
        return false;
    }

    bool IsNavMeshAvailable()
    {
        // NavMesh mevcut deðilse calculateTraingulation bos döner
        var triangulation = NavMesh.CalculateTriangulation();
        return triangulation.vertices != null && triangulation.vertices.Length > 0;
    }

}
