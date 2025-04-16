using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Combat
    }

    public EnemyState curState;

    public bool isDetected = false;

    public float enemySpeed = 2f;
    public float moveDuration = 3f;
    public int isRight = 1;

    public GameObject enemyAttack;
    public GameObject enemyView;
    public GameObject findUI;


    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAttack.SetActive(false);
        findUI.SetActive(false);

        switch (curState)
        {
            case EnemyState.Idle:
                Idle();

                break;
            case EnemyState.Patrol:
                StartCoroutine(Patrol());

                break;
            case EnemyState.Combat:
                Combat();

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        findUI.SetActive(isDetected);

        if (isDetected)
        {
            StopAllCoroutines();


            PlayerDetect();
        }
    }

    private void LateUpdate()
    {
        if (isRight < 0)
        {
            sprite.flipX = isRight < 0;

            enemyAttack.transform.localPosition = new Vector3(isRight < 0 ? -1.62f : 1.62f, 6, 0);
            enemyView.transform.localPosition = new Vector3(isRight < 0 ? -0.5f : 0.5f, 6.32f, 0);
            enemyView.transform.localScale = new Vector3(isRight < 0 ? -1.1f : 1.1f, 1.1f, 1.1f);
        }
    }

    void PlayerDetect() // �÷��̾� ����
    {
        // ���� ���� �̺�Ʈ

        curState = EnemyState.Combat;
    }

    void Idle() // ����
    {
        anim.SetBool("IsWalk", false);

        rb.linearVelocityX = 0;
    }

    void Walk() // �̵�
    {
        anim.SetBool("IsWalk", true);

        rb.linearVelocityX = isRight * enemySpeed * Time.deltaTime;
    }
    
    void Turn() // �ڵ���
    {
        isRight *= -1;

        sprite.flipX = isRight < 0;

        enemyAttack.transform.localPosition = new Vector3(isRight < 0 ? -1.62f : 1.62f, 6, 0);
        enemyView.transform.localPosition = new Vector3(isRight < 0 ? -0.5f : 0.5f, 6.32f, 0);
        enemyView.transform.localScale = new Vector3(isRight < 0 ? -1.1f : 1.1f, 1.1f, 1.1f);
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            // �̵�
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                Walk();
                elapsed += Time.deltaTime;
                yield return null;
            }

            // ���
            Idle();
            yield return new WaitForSeconds(2f);

            // �ڵ���
            Turn();
            elapsed = 0f;
        }
    }

    void Attack() // ����
    {
        enemyAttack.SetActive(true);
    }

    void Combat()
    {

    }
}
