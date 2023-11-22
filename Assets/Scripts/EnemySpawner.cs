using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _waypoints;

    private void Start()
    {
        if(_enemyPrefab.GetComponentInChildren<EnemyScript>() ==null)
        {
            Debug.LogError("Enemy prefab must have EnemyScript component attached");
            return;
        }
    }

    public void Spawn()
    {
        var go =Instantiate(_enemyPrefab,transform.position,Quaternion.identity);
        go.GetComponent<EnemyScript>().Waypoints = _waypoints;
    }

}
