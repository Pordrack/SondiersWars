using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPicker : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    [SerializeField] private GameObject _pickPrompt;

    private List<GameObject> _pickableItems = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        _pickableItems.Add(other.gameObject);
        UpdatePrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        _pickableItems.Remove(other.gameObject);
        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
       _pickPrompt.SetActive(_pickableItems.Count>=1);
    }

    public void Pick()
    {
       _controller.RocksStock+=_pickableItems.Count;
        foreach (var item in _pickableItems)
        {
            Destroy(item);
        }
        _pickableItems.Clear();
        UpdatePrompt();
    }

    public void Start()
    {
        UpdatePrompt();
    }
}
