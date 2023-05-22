using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageViewer : MonoBehaviour
{
    TMP_Text stageText;
    // Start is called before the first frame update
    void Start()
    {
        stageText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        GameManager.Data.LevelChange += ChangeText;
    }

    private void OnDisable()
    {
        GameManager.Data.LevelChange -= ChangeText;
    }

    private void ChangeText(int value)
    {
        if (value ==2)
        {
            stageText.text = "STAGE FINALE";
        }
        stageText.text = $"STAGE : {value + 1}"; 
    }
}
