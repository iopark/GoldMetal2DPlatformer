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
    public int nextMove;  // �ൿ��ǥ�� ������ ���� �ϳ��� ���� 
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
        // Unit �� �����ϸ� �̷��� �Ķ���Ͱ����� ������ ���̰� �ʿ��� ��� ������ ���Ͽ�����. 
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
        spriteRenderer.flipX = nextMove == 1; //���� ������ flip�� �浹 �߻��� �Ǵ� �����Ѵ�. 
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
