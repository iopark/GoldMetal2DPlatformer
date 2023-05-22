using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static DataManager dataManager; 
    public static GameManager Instance
    {
        get { return instance; }
    }
    public static DataManager Data
    {
        get { return dataManager; }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this); 
        }
        instance = this;
        DontDestroyOnLoad(this);
        SetManagers(); 
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null; 
        }
    }

    private void SetManagers()
    {
        GameObject datamanager = new GameObject() { name = "Data Manager" }; 
        datamanager.transform.SetParent(transform);
        dataManager = datamanager.AddComponent<DataManager>();
    }

}
