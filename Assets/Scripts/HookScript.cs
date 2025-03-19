using UnityEngine;

public class HookScript : MonoBehaviour
{
    PlayerHook hook;
    public DistanceJoint2D joint;

    void Start()
    {
        hook = GameObject.Find("Player").GetComponent<PlayerHook>();
        joint = GetComponent<DistanceJoint2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HookPoint"))
        {
            joint.enabled = true;
            hook.isHang = true;
        }
    }
}
