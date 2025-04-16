using UnityEngine;

public class PlayerSturn : MonoBehaviour
{
    public bool canSturn;


    public GameObject sturnUI;
    public EnemyHealth curEnemy;

    PlayerMove player;
    Collider2D coll;

    private void Awake()
    {
        player = GetComponentInParent<PlayerMove>();
        coll = GetComponent<Collider2D>();
    }

    private void LateUpdate()
    {
        coll.offset = new Vector2(player.isRight < 0 ? -3f : 3f, 3.4f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !GameManager.instance.isBattle)
        {
            curEnemy = collision.GetComponent<EnemyHealth>();

            if (curEnemy != null && !curEnemy.isSturn)
            {
                canSturn = true;
                sturnUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !GameManager.instance.isBattle)
        {
            if (collision.GetComponent<EnemyHealth>() == curEnemy)
            {
                canSturn = false;
                curEnemy = null;
                sturnUI.SetActive(false);
            }
        }
    }
}
