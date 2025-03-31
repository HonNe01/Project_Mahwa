using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHp;
    public int maxEnemyHp = 3;

    public bool isDead = false;
    public bool isDamaged = false;


    Animator anim;
    SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHp = maxEnemyHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(int damage)
    {
        enemyHp -= damage;

        // 피격 이벤트
        StartCoroutine(DamageEffect());


        if (enemyHp <= 0)
        {
            Dead();
        }
    }

    IEnumerator DamageEffect()
    {
        isDamaged = true;
        //anim.SetBool("IsDamaged", true);

        for (int i = 0; i < 3; i++)
        {
            sprite.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);

            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.1f);
            //anim.SetBool("IsDamaged", false);
            isDamaged = false;
        }
        sprite.color = new Color(1, 1, 1, 0.3f);
        yield return new WaitForSeconds(0.5f);

        sprite.color = new Color(1, 1, 1, 1);
    }

    void Dead()
    {
        isDead = true;
        //anim.SetTrigger("IsDead");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            TakeDamage(1);
        }
    }
}
