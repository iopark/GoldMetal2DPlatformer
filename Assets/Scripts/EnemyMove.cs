using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider; 

    [Header("AI Related")]
    public int nextMove;  // 행동지표를 결정할 변수 하나를 생성 
    public float nextThinkTime; 

    private Coroutine aiRun; 
    private void Awake()
    {
        capsulecollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        aiRun = StartCoroutine(BasicAI());
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check : if there's no more block, than do otherwise 
        Vector2 front = new Vector2(rigid.position.x+ nextMove*0.5f, rigid.position.y);
        Debug.DrawRay(front, Vector3.down, Color.yellow);
        RaycastHit2D rayHit = Physics2D.Raycast(front, Vector3.down, 1, LayerMask.GetMask("Platform"));
        // Unit 을 통일하면 이렇게 파라미터값으로 벡터의 길이가 필요한 경우 적용이 편리하여진다. 
        if (rayHit.collider == null)
        {
            Debug.Log("Nothing here"); 
            Turn(); 
        }
    }

    void Turn()
    {
        nextMove = nextMove * (-1); // change to opposite direction 
        new WaitForSeconds(nextThinkTime);
        Debug.Log($"{nextThinkTime}");
        spriteRenderer.flipX = nextMove == 1; //원래 기존의 flip과 충돌 발생이 되니 수정한다. 
    }

    IEnumerator BasicAI()
    {
         // waitforseconds interval determined by random value 2 - 5 seconds 
        while (true)
        {
            nextThinkTime = Random.Range(2f, 5f);
            Think();
            Debug.Log(nextThinkTime); 
            yield return new WaitForSeconds(nextThinkTime); 
        }
    }
    private void Think()
    {
        nextMove = Random.Range(-1, 2);

        //Sprite Animation 
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite 
        if (nextMove != 0) // when idle, no need to deal with flipX 
            spriteRenderer.flipX = nextMove == 1; 
    }

    private void OnDestroy()
    {
        StopCoroutine(aiRun);
    }

    public void OnDamage()
    {
        // Sprite change 
        spriteRenderer.color = Color.gray;
        // Sprite flip 
        spriteRenderer.flipY = true;
        // Collider disable 
        capsulecollider.enabled = false;
        // Die Effect Jump 
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Destroy 
        Destroy(gameObject, 5f);
    }
}
