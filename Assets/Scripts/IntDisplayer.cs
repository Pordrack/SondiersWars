using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMeshPro;
    public void DisplayInt(int value)
    {
        _textMeshPro.text = value.ToString();
    }
}
