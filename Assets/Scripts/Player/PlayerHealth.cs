using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDamaged = false;
    public bool isDead = false;

    public int maxHp = 5;
    public int curHp = 5;


    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        curHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;
        rb.linearVelocity = Vector3.zero;

        // 피격 이벤트
        StartCoroutine(DamageEffect());

        
        if (curHp <= 0)
        {
            Dead();
        }
    }

    IEnumerator DamageEffect()
    {
        isDamaged = true;
        anim.SetBool("IsDamaged", true);
        
        for (int i = 0; i < 3; i++)
        {
            sprite.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);

            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("IsDamaged", false);
            isDamaged = false;
        }
        sprite.color = new Color(1, 1, 1, 0.3f);
        yield return new WaitForSeconds(0.5f);

        sprite.color = new Color(1, 1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                
            }
            TakeDamage(1);
        }
    }

    void Dead()
    {
        isDead = true;
        anim.SetTrigger("IsDead");
    }
}
