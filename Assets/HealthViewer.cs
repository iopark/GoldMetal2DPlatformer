using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthViewer : MonoBehaviour
{
    [Header("HealthUI List")]
    public Image[] healthImage;

    public void Start()
    {
        //int count = 0; 
        //foreach (Image image in healthImage)
        //{
        //    healthImage[count] = image.GetComponent<Image>();
        //    count++;    
        //}
    }

    private void OnEnable()
    {
        GameManager.Data.HealthChange += HealthBarChange; 
    }

    private void OnDisable()
    {
        GameManager.Data.HealthChange -= HealthBarChange;
    }

    private void HealthBarChange(int value)
    {
        if (value == 3)
        {
            foreach (Image image in healthImage)
            {
                image.color = Color.white;
            }
        }
        healthImage[value].color = Color.grey; 
    }
}
