using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RockSensor : MonoBehaviour
{
    [SerializeField] private UnityEvent _onRock;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Rock") return;
        _onRock.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Rock") return;
        _onRock.Invoke();
    }
}
