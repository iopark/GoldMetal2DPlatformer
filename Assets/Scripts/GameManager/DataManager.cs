using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DataManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public event UnityAction<int> StageControl; 

    [Header("Player Related")]
    public Collider2D playercollider; 
    public PlayerMove player; 
    private int health;
    public event UnityAction<int> HealthChange; 

    [Header("Map Related")]
    public BoxCollider2D pitfall;
    public GameObject[] stages;

    //[Header("UI Related")]
    public event UnityAction<int> PointChange; 
    public event UnityAction<int> LevelChange;
    public event UnityAction<bool> ButtonAct;
    //public Image[] UIHealth;
    //public TMP_Text UIPoints;
    //public TMP_Text UIStage;
    //public Button button; Requires boundary for the MVC model -> through UnityEvent 

    public int Health
    {
        get { return health; } 
        set
        {
            HealthChange?.Invoke(value);
            health = value;
        }
    }

    public int StageIndex
    {
        get { return stageIndex; }
        set
        {
            StageControl?.Invoke(value);
            stageIndex = value;
        }
    }
    private void Start()
    {
        //Player data related 
        GameObject user = GameObject.FindGameObjectWithTag("Player"); 
        player = user.GetComponent<PlayerMove>();
        health = 3;
        GameObject findGrid = GameObject.Find("Pitfall"); 
        pitfall = findGrid.GetComponent<BoxCollider2D>();
        //Map related 
        stages = GameObject.FindGameObjectsWithTag("Maps"); 
        BoxCollider2D newCollider = gameObject.AddComponent<BoxCollider2D>();
        newCollider.size = pitfall.size;
        newCollider.offset = pitfall.offset;
        newCollider.isTrigger = pitfall.isTrigger;
        pitfall.enabled = false;
    }
    /// <summary>
    /// Seperatable using UnityEvent, thus maintain MVC 
    /// </summary>
    private void Update()
    {
        
    }
    public void GetCoin(GameObject obj)
    {
        switch(obj.gameObject.name)
        {
            case "GoldCoin":
                stagePoint += 150;
                PointChange?.Invoke(stagePoint);
                break;
            case "SilverCoin":
                stagePoint += 100;
                PointChange?.Invoke(stagePoint);
                break;
            case "BronzeCoin":
                stagePoint += 50;
                PointChange?.Invoke(stagePoint);
                break;
        }
        Destroy(obj.gameObject);
    }

    public void NextStage()
    {
        //Change Stage 
        if (StageIndex < 2)
        {
            ++StageIndex; 
            PlayerReposition();
            LevelChange?.Invoke(StageIndex);
        }

        else
        {
            //���� ���ֻ�Ȳ 
            Time.timeScale = 0;// ���ֽ� �ð��� ���� 
            ButtonAct?.Invoke(true); 
        }
        //Calculate game Points 
        totalPoint += stagePoint; 
        stagePoint = 0;
    }
    /// <summary>
    /// Unity Event �� ��� ������ ����������, ���⼭�� �������� ������ �����Ҽ��� �ִٴ� ��ʸ� �����ش�. 
    /// </summary>
    public void HealthDown()
    {
        if (Health > 0)
        {
            Health--;
            if (Health == 0)
                PlayerDeath(); 
        }
    }

    public void PlayerDeath()
    {
        player.onDeath();
        ButtonAct?.Invoke(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playercollider = collision; 
            Debug.Log("Player falling");
            // ü�� �پ���. 
            Health--;
            if (Health > 0) //ü���� 0���� �� Ŭ����, 
            {
                //Player ��ġ ���� ���� 
                PlayerReposition(); 
            }
            else if (Health == 0)
            {
                PlayerReposition(); 
                PlayerDeath(); 
            }
        }
    }
    /// <summary>
    /// Facading 
    /// </summary>
    private void PlayerReposition()
    {
        playercollider.attachedRigidbody.velocity = Vector2.zero;
        playercollider.transform.position = new Vector3(0, 0, -1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        //Reset Data 
        Health = 3;
        StageIndex = 0;
        Time.timeScale = 1; 
    }
}
