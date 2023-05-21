using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        InitManager(); 
    }

    private static void InitManager()
    {
        GameObject gameManager = new GameObject() { name = "Game Manager" }; 
        gameManager.AddComponent<GameManager>();
    }
}
