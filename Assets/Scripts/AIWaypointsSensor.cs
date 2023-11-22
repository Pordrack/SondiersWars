using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIWaypointsSensor : MonoBehaviour
{
    [SerializeField] private UnityEvent<Transform> _onWaypointEnter;

    private void OnTriggerEnter(Collider other)
    {
        _onWaypointEnter.Invoke(other.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _onWaypointEnter.Invoke(collision.transform);
    }
}
