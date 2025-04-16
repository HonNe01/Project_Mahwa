using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    public float curSpeed;
    private float moveSpeed = 5f;
    private float sitSpeed = 3f;
    private float inputValue;

    public GameObject hitscan;
    public Collider2D standHitscan;
    public Collider2D sitHitscan;

    public Collider2D stand_coll;
    public Collider2D sit_coll;

    public Collider2D coll;

    // 점프
    private float jumpForce = 10f;
    private float shortJump = 0.3f;

    // 벽점프
    private float wallJumpForce = 9f;
    private float slidingSpeed = 0.5f;

    // 대쉬
    private float dashForce = 20f;
    private float dashTime = 0.2f;
    private float dashCool = 1f;

    [Header("Attack")]
    public GameObject attackRange;

    [Header("Stealth")]
    public bool isSit = false;
    public bool isHide;
    public bool isSturn;


    // 상태
    public int isRight;
    bool isGrounded = true;

    bool isWallJumping = false;
    bool isWallSliding = false;
    bool canSlding = true;

    bool isDash = false;
    bool canDash = true;

    bool isAttack = false;
    bool canAttack = true;

    Vector2 groundCheckPos;
    Vector2 wallCheckPos;

    float rayDistance = 0.1f;

    [Header("Layer Setting")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;


    Animator anim;

    Rigidbody2D rb;
    SpriteRenderer sprite;


    PlayerHook hook;
    PlayerSturn sturn;
    PlayerHealth hp;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();


        hp = GetComponent<PlayerHealth>();
        hook = GetComponent<PlayerHook>();
        sturn = GetComponentInChildren<PlayerSturn>();
    }

    void Start()
    {
        curSpeed = moveSpeed;
        sitHitscan.enabled = false;
        standHitscan.enabled = true;

        sit_coll.enabled = false;

        attackRange.SetActive(false);
        
        sturn.sturnUI.SetActive(false);
        sturn.curEnemy = null;
        sturn.canSturn = false;
    }

    void Update()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("SpeedX", Mathf.Abs(inputValue));
        anim.SetFloat("SpeedY", rb.linearVelocityY);

        // 좌우 체크
        if (inputValue != 0 && !isWallJumping && !hp.isDead && !isSturn)
        {
            isRight = inputValue > 0 ? 1 : -1;
        }

        // 지면 체크
        groundCheckPos = new Vector2(coll.bounds.center.x, coll.bounds.min.y);
        isGrounded = Physics2D.Raycast(groundCheckPos, Vector2.down, rayDistance, groundLayer);

        // 벽 체크
        wallCheckPos = new Vector2(isRight > 0 ? coll.bounds.max.x : coll.bounds.min.x, coll.bounds.center.y);
        isWallSliding = canSlding && Physics2D.Raycast(wallCheckPos, Vector2.right * isRight, rayDistance, wallLayer);


        // 앉기
        if (Input.GetButton("Sit") && !hook.isHang && !hp.isDead && !hp.isDamaged && !isSturn)
        {
            Sit();
        }
        else
        {
            Stand();
        }

        // 점프
        if (Input.GetButtonDown("Jump") && !hp.isDead && !isSturn)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (isWallSliding) // 벽 점프
            {
                StartCoroutine(WallJump());
            }
        }

        // 짧은 점프
        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0 && !hp.isDead && !isSturn)
        {
            rb.linearVelocityY = rb.linearVelocityY * shortJump;
        }

        // 로프 점프
        if (Input.GetButtonDown("Jump") && hook.isHang && !isSturn)
        {
            hook.RopeRelease();

            Jump();
        }

        // 벽 슬라이딩
        if (isWallSliding && rb.linearVelocityY < 0)
        {
            anim.SetTrigger("IsWallSliding");
            rb.linearVelocityY = rb.linearVelocityY * slidingSpeed;
        }

        // 대쉬
        if (Input.GetButtonDown("Dash") && canDash && isGrounded && !hp.isDead && !isSturn)
        {
            StartCoroutine(Dash());
        }

        // 통상 공격
        if (Input.GetButtonDown("Attack") && canAttack)
        {
            if (GameManager.instance.isBattle)
            {
                StartCoroutine(Attack());
            }
            else if (!GameManager.instance.isBattle && sturn.canSturn)
            {
                StartCoroutine(Sturn());
            }
        }
    }

    private void FixedUpdate()
    {
        // 좌우 이동
        if (!isWallJumping && !isAttack && !isSturn)
        {
            if (hook.isHang)
            {
                // 로프에 매달려 있을 경우 AddForce로 변경
                rb.AddForce(new Vector2(inputValue * moveSpeed, 0));
            }
            else if (!isDash && !hp.isDead)
            {
                rb.linearVelocityX = inputValue * curSpeed;
            }
        }
    }

    private void LateUpdate()
    {
        if (inputValue != 0 && !isSturn)
        {
            sprite.flipX = isRight < 0;

            hook.hookHand.localPosition = new Vector3(isRight < 0 ? -1.62f : 1.62f, 6, 0);
            attackRange.transform.localPosition = new Vector3(isRight < 0 ? -5f : 5f, 3f, 0);
        }

        anim.SetBool("IsHang", hook.isHang);
    }

    public void Sit()
    {
        isSit = true;
        sitHitscan.enabled = true;
        stand_coll.enabled = false;
        sit_coll.enabled = true;

        if (isHide)
        {
            hitscan.tag = "PlayerHide";
        }
        else
        {
            hitscan.tag = "PlayerHitscan";
        }

        standHitscan.enabled = false;
        sprite.sortingOrder = 4;

        curSpeed = sitSpeed;
        anim.SetBool("IsSit", true);
    }

    public void Stand()
    {
        isSit = false;
        hitscan.tag = "PlayerHitscan";
        sit_coll.enabled = false;
        stand_coll.enabled = true;
        
        sitHitscan.enabled = false;
        standHitscan.enabled = true;
        sprite.sortingOrder = 8;

        curSpeed = moveSpeed;
        anim.SetBool("IsSit", false);
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    IEnumerator WallJump()
    {
        canSlding = false;
        isWallJumping = true;

        isRight = -isRight;
        sprite.flipX = !sprite.flipX;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(isRight * wallJumpForce * 0.8f, wallJumpForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        canSlding = true;
        isWallJumping = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDash = true;
        rb.linearVelocityX = isRight * dashForce;

        yield return new WaitForSeconds(dashTime);
        isDash = false;

        yield return new WaitForSeconds(dashCool);
        canDash = true;
    }

    IEnumerator Attack()
    {
        isAttack = true;
        canAttack = false;
        anim.SetTrigger("IsAttack");
        attackRange.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    public void endAttack()
    {
        isAttack = false;
        attackRange.SetActive(false);
    }

    IEnumerator Sturn()
    {
        isHide = false;
        isSturn = true;
        sprite.enabled = false;
        sturn.curEnemy.SturnAnim(!isGrounded);

        yield return new WaitForSeconds(1f);

        isSturn = false;
        sprite.enabled = true;
        sturn.sturnUI.SetActive(false);

        sturn.curEnemy.Sturn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Objects"))
        {
            if (isSit)
            {
                hitscan.tag = "PlayerHide";
                isHide = true;
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Objects"))
        {
            hitscan.tag = "Player";
            isHide = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 땅 체크
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector2.down * rayDistance);

        // 벽 체크
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + Vector2.right * rayDistance * isRight);
    }
}
