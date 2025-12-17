using UnityEngine;
using System.Collections.Generic;

public class CandleSpawner : MonoBehaviour
{
    public GameObject candlePrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public int maxCandlesAlive = 3;
    public float respawnCheckSeconds = 5f;

    private readonly List<GameObject> alive = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(SpawnIfNeeded), 0f, respawnCheckSeconds);
    }

    private void SpawnIfNeeded()
    {
        alive.RemoveAll(x => x == null);

        while (alive.Count < maxCandlesAlive)
        {
            Transform p = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject c = Instantiate(candlePrefab, p.position, p.rotation);
            alive.Add(c);
        }
    }
}
