using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonViewer : MonoBehaviour
{
    public Button button;
    public TMP_Text buttonText;
    public int test;
    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        buttonText.text = ""; 
        GameManager.Data.ButtonAct += ChangeText;
        test = 1; 
    }
    private void ChangeText(bool value)
    {
        button.interactable = true; 
        if (value)
            buttonText.text = "Complete!";
        else
            buttonText.text = "Retry?";
        gameObject.SetActive(true);
    }

    private void PostClick()
    {
        gameObject.SetActive(false);
    }
}
