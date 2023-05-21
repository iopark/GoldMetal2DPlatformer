using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Related")]
    public float maxSpeed;
    public float jumpPower; 

    Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator anim;


    private void Start()
    {
        jumpPower = 15;
        maxSpeed = 3; 
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        //Jump 
        if (Input.GetButtonDown("Jump") && !anim.GetBool("IsJump"))
        {
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJump", true); 
        }

        //Landing Platform 
        Debug.DrawRay(rigidbody.position, Vector3.down, new Color(0, 1, 0)); 
            // If key released, halves the current velocity(normalized)(directional value) 
        if (Input.GetButtonUp("Horizontal"))
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.normalized.x * 0.8f, rigidbody.velocity.y); 
        }

        //SpriteRender Flip for Direciton : Player should flip based on the most current user input only 
        if (Input.GetButton("Horizontal")) // if player presses either L or R, 
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; //(default) // if its L, flipX = true,
                                                                         //if not, false. thus, flipX is only updated based on most recent inputKey

        if (rigidbody.velocity.x != 0)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false); 
    }

    /// <summary>
    /// 주로 물리 연산은 fixedupdate에서 진행하는것이 정배 
    /// </summary>
    private void FixedUpdate()
    {
        //Move by Control 
        float h = Input.GetAxisRaw("Horizontal");
        rigidbody.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        if (rigidbody.velocity.x > maxSpeed) // Right MaxSpeed 
        {
            rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
        }
        else if (rigidbody.velocity.x <  maxSpeed* (-1)) // Left MaxSpeed 
        {
            rigidbody.velocity = new Vector2(maxSpeed * (-1), rigidbody.velocity.y); 
        }
        //Jumping, utilizing RaycastHit .collider, .distance 
        if (rigidbody.velocity.y < 0) // y값, 즉 떨어지는 상태일 때에만 
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigidbody.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            // Unit 을 통일하면 이렇게 파라미터값으로 벡터의 길이가 필요한 경우 적용이 편리하여진다. 
            if (rayHit.collider != null) // rayhit collider에 뭐가 있다면, 
            {
                if (rayHit.distance < 0.5f) // 생성유닛에 대해서 사이즈 단일화를 적용하면 이런 값을 찾아야하는 상황에서도 유리할수 있다. 
                    anim.SetBool("IsJump", false);
            }
        }
    }

    private Coroutine DamageReact; 
    /// <summary>
    /// Collider 간 충격이 발생했을때, 공격을 하거나 받거나 하게 된다. 
    /// 플레이어는 점프로(위에서 아래로) 공격이 가능하며, 
    /// 그것이 아니라면 공격을 받는다. 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if(rigidbody.velocity.y <0 && transform.position.y > collision.transform.position.y && collision.gameObject.tag == "Enemy")
            {
                OnAttack(collision.transform);
            }
            DamageReact = StartCoroutine(DamageCycle(collision));
            //StopCoroutine(DamageReact); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //item 을 먹었을때 생기는 변화 
            // Earn Point 
            GameManager.Instance.stagePoint += 100; 
            // Deactive Item 
            Destroy(collision.gameObject); 
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //move to Next Stage. (Controlled by GameManager) 
        }
    }

    private void OnAttack(Transform target)
    {
        // Point 
        // Player Reaction 
        rigidbody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Enemy dies 
        EnemyMove enemyMove = target.GetComponent<EnemyMove>();
        enemyMove.OnDamage(); // 해당 gameObj의 컴포넌트를 불러와 참조할수도 있겠다. 
    }

    IEnumerator DamageCycle(Collision2D collision)
    {
        bool cycle = false; 
        while (!cycle)
        {
            cycle = true; 
            OnDamaged(collision.transform.position);
            yield return new WaitForSeconds(3); 
        }
        OffDamage();
        yield break; // ensures the coroutine stops 
    }
    /// <summary>
    /// 맞았을때 나타나는 현상들 
    /// 1. 한동안은 공격당할수 없음: 레이어 변화 
    /// 2. 맞았을때 나타나는 현상들 
    /// 2-1. 색상 변화 
    /// 2-2. 밀림 현상 
    /// </summary>
    /// <param name="target">피격하는 대상 </param>
    private void OnDamaged(Vector2 target)
    {

        //Change Layer( if damaged) 
        gameObject.layer = 11; // layer에 해당하는 int 값을 이용하여 layer 변경 
        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Reaction Force = Given the gameObj is hit by another object, make it move in that direction
        //
        int attackDir = transform.position.x - target.x > 0 ? 1: -1; 
        rigidbody.AddForce(new Vector2(attackDir, 1)*7, ForceMode2D.Impulse);

        //Animation 피격 
        anim.SetTrigger("doDamage");
    }
    /// <summary>
    /// 맞은 이후의 변화 
    /// 1. 색깔 다시 변경 
    /// 2. 레이어 원상복구, if not dead. 
    /// </summary>
    private void OffDamage()
    {
        gameObject.layer = 10; 
        spriteRenderer.color = Color.white;
    }


}
