using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] float inputValue;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float shortJump = 0.5f;

    [Header("Wall Jump")]
    [SerializeField] float wallJumpForce = 10f;
    [SerializeField] float slidingSpeed = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCool = 1f;

    [Header("Attack")]
    public GameObject attackRange;


    [Header("Check Setting")]
    int isRight;
    bool isGrounded = true;
    bool isWallJumping = false;
    bool isWallSliding = false;
    bool isDash = false;
    bool isAttack = false;
    bool canSlding = true;
    bool canDash = true;
    bool canAttack = true;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    float rayDistance = 0.1f;
    Vector2 groundCheckPos;
    Vector2 wallCheckPos;

    Animator anim;
    PlayerHook hook;
    PlayerHealth hp;
    Collider2D coll;
    Rigidbody2D rb;
    SpriteRenderer sprite;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponent<PlayerHealth>();
        coll = GetComponentInChildren<Collider2D>();
        hook = GetComponent<PlayerHook>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("SpeedX", Mathf.Abs(inputValue));
        anim.SetFloat("SpeedY", rb.linearVelocityY);

        if (inputValue != 0 && !isWallJumping && !hp.isDead)
        {
            isRight = inputValue > 0 ? 1 : -1;
        }

        // ���� üũ
        groundCheckPos = new Vector2(coll.bounds.center.x, coll.bounds.min.y);
        isGrounded = Physics2D.Raycast(groundCheckPos, Vector2.down, rayDistance, groundLayer);

        // �� üũ
        wallCheckPos = new Vector2(isRight > 0 ? coll.bounds.max.x : coll.bounds.min.x, coll.bounds.center.y);
        isWallSliding = canSlding && Physics2D.Raycast(wallCheckPos, Vector2.right * isRight, rayDistance, wallLayer);

        // ����
        if (Input.GetButtonDown("Jump") && !hp.isDead)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (isWallSliding) // �� ����
            {
                StartCoroutine(WallJump());
            }
        }

        // ª�� ����
        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0 && !hp.isDead)
        {
            rb.linearVelocityY = rb.linearVelocityY * shortJump;
        }

        // �� �����̵�
        if (isWallSliding && rb.linearVelocityY < 0)
        {
            anim.SetTrigger("IsWallSliding");
            rb.linearVelocityY = rb.linearVelocityY * slidingSpeed;
        }

        // �뽬
        if (Input.GetButtonDown("Dash") && canDash && isGrounded && !hp.isDead)
        {
            StartCoroutine(Dash());
        }

        // ��� ����
        if (Input.GetButtonDown("Attack") && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        // �¿� �̵�
        if (!isWallJumping && !isAttack)
        {
            if (hook.isHang)
            {
                // ������ �Ŵ޷� ���� ��� AddForce�� ����
                rb.AddForce(new Vector2(inputValue * moveSpeed, 0));
            }
            else if (!isDash && !hp.isDead)
            {
                rb.linearVelocityX = inputValue * moveSpeed;
            }
        }
    }

    private void LateUpdate()
    {
        if (inputValue != 0)
        {
            sprite.flipX = isRight < 0;

            hook.hookHand.localPosition = new Vector3(isRight < 0 ? -1.62f : 1.62f, 6, 0);
            attackRange.transform.localPosition = new Vector3(isRight < 0 ? -2.69f : 2.69f, 4.33f, 0);
        }

        anim.SetBool("IsHang", hook.isHang);
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

    private void OnDrawGizmosSelected()
    {
        // �� üũ
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector2.down * rayDistance);

        // �� üũ
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + Vector2.right * rayDistance * isRight);
    }
}
