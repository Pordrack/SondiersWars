using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    [Tooltip("The prefab to instantiate when the rock hits something")]
    [SerializeField] GameObject _impactPrefab;
    [Tooltip("After how many seconds are impacts objects deleted ?")]
    [SerializeField] float _impactObjectLifetime;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject impact = Instantiate(_impactPrefab, transform.position, Quaternion.identity);
        Destroy(impact, _impactObjectLifetime);
    }
}
