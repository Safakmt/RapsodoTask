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
    public int textureIndex = 2; // kum 2. indexte
    private List<GameObject> _balls = new List<GameObject>();

    public void SpawnBalls()
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
                GameObject instantiatedObj = Instantiate(ballPrefab, validNavMeshPosition, Quaternion.identity);
                _balls.Add(instantiatedObj);
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
        // NavMesh yoksa calculateTraingulation bos döner
        var triangulation = NavMesh.CalculateTriangulation();
        return triangulation.vertices != null && triangulation.vertices.Length > 0;
    }


    public void AssignPointsToBalls()
    {
        List<GameObject> highValueBalls = GetBallsOnTexture();

        foreach(var ballObj in _balls)
        {
            Ball ball = ballObj.GetComponent<Ball>();
            if (ballObj.transform.position.y > 5)
            {
                ball.UpdatePoint(3);
            }
            else
            {
                ball.UpdatePoint(1);
            }
        }

        foreach (var ballObj in highValueBalls)
        {
            Ball ball = ballObj.GetComponent<Ball>();
            ball.UpdatePoint(5);
        }

    }
    List<GameObject> GetBallsOnTexture()
    {
        List<GameObject> ballsOnTexture = new List<GameObject>();
        TerrainData terrainData = _terrain.terrainData;

        Vector3 terrainPosition = _terrain.transform.position;
        Vector3 terrainSize = terrainData.size;

        // alpha mapi hesapla
        int alphaMapWidth = terrainData.alphamapWidth;
        int alphaMapHeight = terrainData.alphamapHeight;
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
        foreach (var ball in _balls)
        {
            // topun terraindaki koordinati 
            Vector3 ballPosition = ball.transform.position;
            float normalizedX = (ballPosition.x - terrainPosition.x) / terrainSize.x;
            float normalizedZ = (ballPosition.z - terrainPosition.z) / terrainSize.z;

            // Alphamap koordinati
            int mapX = Mathf.RoundToInt(normalizedX * (alphaMapWidth - 1));
            int mapZ = Mathf.RoundToInt(normalizedZ * (alphaMapHeight - 1));

            // aradigimiz textureun bu koordinattaki degeri
            float textureValue = alphaMaps[mapZ, mapX, textureIndex];

            // deger 0.2 uzerinde ise topu listeye ekle
            if (textureValue > 0.2f) 
            {
                ballsOnTexture.Add(ball);
            }
        }

        return ballsOnTexture;
    }

    public List<Ball> GetBalls()
    {
        List<Ball> balls = new List<Ball>();
        foreach (var ballObj in _balls)
        {
            balls.Add(ballObj.GetComponent<Ball>());
        }
        return balls;
    }
}
