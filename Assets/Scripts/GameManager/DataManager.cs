using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DataManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    [Header("Player Related")]
    public Collider2D playercollider; 
    public PlayerMove player; 
    public int health;

    [Header("Map Related")]
    public BoxCollider2D pitfall;
    public GameObject[] stages;

    [Header("UI Related")]
    public Image[] UIHealth;
    public TMP_Text UIPoints;
    public TMP_Text UIStage;
    public Button button; 
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

        //UI Related 
        Canvas canvas = FindAnyObjectByType<Canvas>();
        UIHealth = FindObjectsByType<Image>();
        UIPoints = canvas.transform.Find("Points")?.GetComponent<TMP_Text>();
        UIStage = canvas.transform.Find("Stage")?.GetComponent<TMP_Text>();
        button = canvas.transform.Find("Button")?.GetComponent<Button>();
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
                break;
            case "SilverCoin":
                stagePoint += 100;
                break;
            case "BronzeCoin":
                stagePoint += 50;
                break;
        }
        Destroy(obj.gameObject);
    }

    public void NextStage()
    {
        //Change Stage 
        if (stageIndex < stages.Length -1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIStage.text = $"STAGE {stageIndex + 1}";
        }

        else
        {
            //게임 완주상황 
            Time.timeScale = 0;// 완주시 시간을 정지 
            button.text = "Game Clear"; 
            button.SetEnabled(true);
        }


        //Calculate game Points 
        totalPoint += stagePoint; 
        stagePoint = 0;
    }
    /// <summary>
    /// Unity Event 로 사실 관리가 가능하지만, 여기서는 직접적인 참조로 관리할수도 있다는 사례를 보여준다. 
    /// </summary>
    public void HealthDown()
    {
        if (health > 0)
        {
            health--;
            UIHealth[health].tintColor = Color.gray; 
        }
        else
        {
            //Player die, 
            player.onDeath();
            button.SetEnabled(true); 
            //Change UI 
            // Retry Button UI 
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Player falling");
            // 체력 줄어든다. 
            health--;
            if (health > 0) //체력이 0보다 더 클때는, 
            {
                //Player 위치 원상 복귀 
                PlayerReposition(); 
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
        button.SetEnabled(false); 
        //Reset Data 
        health = 3; stages[stageIndex].SetActive(false);  stages[0].SetActive(true);
        stageIndex = 0;
        Time.timeScale = 1; 
    }
}
