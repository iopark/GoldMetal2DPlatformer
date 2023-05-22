using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject[] stages;
    private int previous_stage;
    public int currentstage; 

    private void Start()
    {
        previous_stage = 0;
        GameManager.Data.StageControl += StageSwitch;
    }

    private void OnEnable()
    {
        
    }

    private void StageSwitch(int index)
    {
        stages[index].SetActive(true);
        stages[previous_stage].SetActive(false);
        previous_stage = index;
        currentstage = index; 
    }
}
