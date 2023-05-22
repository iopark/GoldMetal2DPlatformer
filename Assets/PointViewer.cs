using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointViewer : MonoBehaviour
{
    TMP_Text pointText; 
    // Start is called before the first frame update
    void Start()
    {
        pointText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        GameManager.Data.PointChange += ChangeText; 
    }

    private void OnDisable()
    {
        GameManager.Data.PointChange -= ChangeText;
    }

    private void ChangeText(int value)
    {
        pointText.text = value.ToString();
    }
}
