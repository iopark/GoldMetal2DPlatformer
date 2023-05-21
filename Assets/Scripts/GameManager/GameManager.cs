using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    public static GameManager Instance
    {
        get { return instance; }
    }
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this); 
        }
        instance = this;
        DontDestroyOnLoad(this); 
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null; 
        }
    }
}
