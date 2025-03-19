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

    [Header("Check Setting")]
    [SerializeField] int isRight;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isWallJumping = false;
    [SerializeField] bool isWallSliding;
    [SerializeField] bool canSlding = true;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float rayDistance = 0.1f;
    Vector2 groundCheckPos;
    Vector2 wallCheckPos;

    Animator anim;
    PlayerHook hook;
    Collider2D coll;
    Rigidbody2D rb;
    SpriteRenderer sprite;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        hook = GetComponent<PlayerHook>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(inputValue));

        if (inputValue != 0 && !isWallJumping)
        {
            isRight = inputValue > 0 ? 1 : -1;
        }

        // 지면 체크
        groundCheckPos = new Vector2(coll.bounds.center.x, coll.bounds.min.y);
        isGrounded = Physics2D.Raycast(groundCheckPos, Vector2.down, rayDistance, groundLayer);

        // 벽 체크
        wallCheckPos = new Vector2(isRight > 0 ? coll.bounds.max.x : coll.bounds.min.x, coll.bounds.center.y);
        isWallSliding = canSlding && Physics2D.Raycast(wallCheckPos, Vector2.right * isRight, rayDistance, wallLayer);

        // 점프
        if (Input.GetButtonDown("Jump"))
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
        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY = rb.linearVelocityY * shortJump;
        }

        // 벽 슬라이딩
        if (isWallSliding && rb.linearVelocityY < 0)
        {
            rb.linearVelocityY = rb.linearVelocityY * slidingSpeed;
        }
    }

    private void FixedUpdate()
    {
        // 좌우 이동
        if (!isWallJumping)
        {
            if (hook.isHang)
            {
                // 로프에 매달려 있을 경우 AddForce로 변경
                rb.AddForce(new Vector2(inputValue * moveSpeed, 0));
            }
            else
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
        }
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

    private void OnDrawGizmosSelected()
    {
        // 땅 체크
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector2.down * rayDistance);

        // 벽 체크
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + Vector2.right * rayDistance * isRight);
    }
}
