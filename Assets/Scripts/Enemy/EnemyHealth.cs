using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHp;
    public int maxEnemyHp = 3;

    public bool isDead = false;
    public bool isSturn = false;
    public bool isDamaged = false;

    public GameObject view;
    public EnemyMove move;

    Animator anim;
    SpriteRenderer sprite;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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

    public void SturnAnim(bool isJump)
    {
        move.StopAllCoroutines();
        view.SetActive(false);

        anim.SetBool("IsWalk", false);

        if (isJump)
        {
            anim.SetTrigger("SturnAnim_Jump");
        }
        else
        {
            string triggerName = Random.Range(0, 2) == 0 ? "SturnAnim_Stand" : "SturnAnim_Jump";
            anim.SetTrigger(triggerName);
        }
    }

    public void Sturn()
    {
        isSturn = true;

        anim.SetBool("IsSturn", true);
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

        sprite.color = new Color(1, 1, 1, 0.3f);
        yield return new WaitForSeconds(0.2f);

        sprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.2f);
        //anim.SetBool("IsDamaged", false);
        isDamaged = false;
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
