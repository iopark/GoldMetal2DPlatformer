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
        if (Input.GetButtonDown("Horizontal")) // if player presses either L or R, 
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
        if (rigidbody.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigidbody.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            // Unit 을 통일하면 이렇게 파라미터값으로 벡터의 길이가 필요한 경우 적용이 편리하여진다. 
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f) // 생성유닛에 대해서 사이즈 단일화를 적용하면 이런 값을 찾아야하는 상황에서도 유리할수 있다. 
                    anim.SetBool("IsJump", false);
            }
        }

    }
}
