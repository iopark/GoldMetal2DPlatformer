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
    /// �ַ� ���� ������ fixedupdate���� �����ϴ°��� ���� 
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
        if (rigidbody.velocity.y < 0) // y��, �� �������� ������ ������ 
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigidbody.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            // Unit �� �����ϸ� �̷��� �Ķ���Ͱ����� ������ ���̰� �ʿ��� ��� ������ ���Ͽ�����. 
            if (rayHit.collider != null) // rayhit collider�� ���� �ִٸ�, 
            {
                if (rayHit.distance < 0.5f) // �������ֿ� ���ؼ� ������ ����ȭ�� �����ϸ� �̷� ���� ã�ƾ��ϴ� ��Ȳ������ �����Ҽ� �ִ�. 
                    anim.SetBool("IsJump", false);
            }
        }
    }

    private Coroutine DamageReact; 
    /// <summary>
    /// Collider �� ����� �߻�������, ������ �ϰų� �ްų� �ϰ� �ȴ�. 
    /// �÷��̾�� ������(������ �Ʒ���) ������ �����ϸ�, 
    /// �װ��� �ƴ϶�� ������ �޴´�. 
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
            //item �� �Ծ����� ����� ��ȭ 
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
        enemyMove.OnDamage(); // �ش� gameObj�� ������Ʈ�� �ҷ��� �����Ҽ��� �ְڴ�. 
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
    /// �¾����� ��Ÿ���� ����� 
    /// 1. �ѵ����� ���ݴ��Ҽ� ����: ���̾� ��ȭ 
    /// 2. �¾����� ��Ÿ���� ����� 
    /// 2-1. ���� ��ȭ 
    /// 2-2. �и� ���� 
    /// </summary>
    /// <param name="target">�ǰ��ϴ� ��� </param>
    private void OnDamaged(Vector2 target)
    {

        //Change Layer( if damaged) 
        gameObject.layer = 11; // layer�� �ش��ϴ� int ���� �̿��Ͽ� layer ���� 
        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Reaction Force = Given the gameObj is hit by another object, make it move in that direction
        //
        int attackDir = transform.position.x - target.x > 0 ? 1: -1; 
        rigidbody.AddForce(new Vector2(attackDir, 1)*7, ForceMode2D.Impulse);

        //Animation �ǰ� 
        anim.SetTrigger("doDamage");
    }
    /// <summary>
    /// ���� ������ ��ȭ 
    /// 1. ���� �ٽ� ���� 
    /// 2. ���̾� ���󺹱�, if not dead. 
    /// </summary>
    private void OffDamage()
    {
        gameObject.layer = 10; 
        spriteRenderer.color = Color.white;
    }


}
