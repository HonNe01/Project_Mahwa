using UnityEngine;

public class PlayerHook : MonoBehaviour
{
    public LineRenderer line;
    public Transform hook;
    public Transform hookHand;

    Vector2 mouseDir;

    public bool isHook;
    public bool isLineMax;
    public bool isHang;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.03f;
        line.SetPosition(0, hookHand.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;

        isHang = false;
        hook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 훅 위치 갱신
        line.SetPosition(0, hookHand.position);
        line.SetPosition(1, hook.position);

        if (Input.GetMouseButtonDown(0))
        {
            // 로프 방향 계산
            rb.linearVelocity = Vector2.zero;
            hook.position = hookHand.position;
            mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - hookHand.position;

            isHook = true;
            hook.gameObject.SetActive(true);
        }

        if (isHook && !isLineMax && !isHang)
        {
            // 훅 회전

            // 로프 이동
            hook.Translate(mouseDir.normalized * Time.deltaTime * 15);

            if (Vector2.Distance(hookHand.position, hook.position) > 3.5f)
            {
                isLineMax = true;
            }
        }
        else if (isHook && isLineMax && !isHang)
        {
            // 로프 원위치
            hook.position = Vector2.MoveTowards(hook.position, hookHand.position, Time.deltaTime * 30);

            if (Vector2.Distance(hookHand.position, hook.position) < 0.1f)
            {
                isHook = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);
            }
        }
        else if (isHang)
        {
            // 로프 끊기
            if (Input.GetMouseButtonUp(0))
            {
                isHang = false;
                isHook = false;
                isLineMax = false;

                hook.GetComponent<HookScript>().joint.enabled = false;
                hook.gameObject.SetActive(false);
            }
        }
    }
}
