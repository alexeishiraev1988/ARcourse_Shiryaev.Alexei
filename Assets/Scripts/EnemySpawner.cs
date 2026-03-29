using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _enemyCount = 2;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            float x = i == 0 ? -4f : 4f;
            Vector3 spawnPos = new Vector3(x, 3f, 0);
            Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
