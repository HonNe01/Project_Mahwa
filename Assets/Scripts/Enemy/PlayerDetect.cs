using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    EnemyMove enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitscan"))
        {
            Debug.Log("플레이어 발견!");

            enemy.isDetected = true;
        }
        if (collision.CompareTag("PlayerHide"))
        {
            Debug.Log("플레이어 놓침");

            enemy.isDetected = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitscan"))
        {
            Debug.Log("플레이어 놓침");

            enemy.isDetected = false;
        }
    }
}
