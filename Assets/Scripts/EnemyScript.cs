using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public Transform[] Waypoints;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _despawnTime = 10f;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _minWalkTime = 20;
    [SerializeField] private float _maxWalkTime = 30;
    [SerializeField] private float _minStopTime = 0;
    [SerializeField] private float _maxStopTime = 10;
    private int _currentWaypoint = 0;
    private Coroutine _walkAndStopCoroutine;

    private void Start()
    {
        GoToNextWaypoints();
        DisableRagdoll();
        _walkAndStopCoroutine = StartCoroutine(WalkAndStopCoroutine());
    }

    public void OnWaypointEnter(Transform waypoint)
    {
        if (Waypoints[_currentWaypoint] != waypoint) return;
        GoToNextWaypoints();
    }

    private void GoToNextWaypoints()
    {
        int oldWaypoint=_currentWaypoint;

        while (_currentWaypoint == oldWaypoint)
        {
            _currentWaypoint = Random.Range(0, Waypoints.Length);
        }

        StartWalking();
    }

    private void StopWalking()
    {
        _animator.SetBool("Headbanging", true);
        _agent.destination = transform.position;

    }

    private void StartWalking()
    {
        _animator.SetBool("Headbanging", false);
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
        StopCoroutine(_walkAndStopCoroutine);
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

    private IEnumerator WalkAndStopCoroutine()
    {
        while(true)
        {
            StartWalking();
            yield return new WaitForSeconds(Random.Range(_minWalkTime, _maxWalkTime));
            StopWalking();
            yield return new WaitForSeconds(Random.Range(_minStopTime,_maxStopTime));
        }
    }
}
