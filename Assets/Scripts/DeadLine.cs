using UnityEngine;

public class DeadLine : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ������ �Ա�
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            player.TakeDamage(1);

            float distanceLeft = Vector2.Distance(collision.transform.position, leftPoint.position);
            float distanceRight = Vector2.Distance(collision.transform.position, rightPoint.position);

            // ������
            if (distanceLeft < distanceRight)
                collision.transform.position = leftPoint.position;
            else
                collision.transform.position = rightPoint.position;
        }
    }
}
