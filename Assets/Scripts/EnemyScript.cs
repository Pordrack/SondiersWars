using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public Transform[] Waypoints;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _despawnTime = 10f;
    private int _currentWaypoint = 0;

    private void Start()
    {
        GoToNextWaypoints();
        DisableRagdoll();
    }

    public void OnWaypointEnter(Transform waypoint)
    {
        if (Waypoints[_currentWaypoint] != waypoint) return;
        GoToNextWaypoints();
    }

    private void GoToNextWaypoints()
    {
        _currentWaypoint = (_currentWaypoint + 1) % Waypoints.Length;
        _agent.destination = Waypoints[_currentWaypoint].position;
    }

    private void DisableRagdoll()
    {
        //Disable each child rigidbody
        var childColliders = GetComponentsInChildren<Collider>();
        var childRigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (var rb in childRigidbodies)
        {
            rb.isKinematic = true;
        }

        GetComponent<Collider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Animator>().enabled = true;
    }

    private void EnableRagdoll()
    {
        //Enable each child rigidbody
        var childRigidbodies=GetComponentsInChildren<Rigidbody>();

        foreach (var rb in childRigidbodies)
        {
            rb.isKinematic = false;
        }

        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Animator>().enabled = false;
    }

    public void Die()
    {
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        EnableRagdoll();
        yield return new WaitForSeconds(_despawnTime);
        //We clip through the ground before destroying ourselves
        var childColliders = GetComponentsInChildren<Collider>();
        foreach (var col in childColliders)
        {
            col.enabled = false;
        }
        Destroy(this.gameObject,2);
    }
}
