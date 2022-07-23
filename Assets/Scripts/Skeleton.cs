using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    Rigidbody2D rigidbody;
    Animator animator;

    [Header("Move")]
    [SerializeField, Range(0, 10)] float speed;
    [SerializeField, Range(0, 100)] float health;

    [Header("Checkers")]
    [SerializeField] Transform wallChecker;
    [SerializeField] Transform edgeChecker;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask checkLayers;
    [SerializeField] LayerMask whatIsEnemy;

    [Header("Attack")]
    [SerializeField, Range(0, 10)] float attackRange;
    [SerializeField, Range(0, 100)] float damage;
    [SerializeField] float attackSpeed;

    private float timeToAttack = 0;
    private float direction = 1;
    private bool isAllive = true;
    private bool isAttack = false;

    PlayerController player;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (player.IsDead)
        {
            return;
        }

        Attack();

        if (isAllive && !isAttack)
        {
            if (CheckWalls() || !CheckEdge())
            {
                direction *= -1;
            }
            rigidbody.velocity = new Vector2(speed * direction, rigidbody.velocity.y);
            rigidbody.transform.localScale = new Vector3(2 * direction, 2, 2);
        }
        else if (isAllive && isAttack)
        {
            rigidbody.velocity = Vector2.zero;
        }
        else
        {
            rigidbody.velocity = Vector2.zero;
        }
    }

    void Animation()
    {
        float horizontal = rigidbody.velocity.x;
        animator.SetFloat("isMoveing", Mathf.Abs(horizontal));
    }

    public void TakeDamge(float damage)
    {
        health -= damage;
        if (!isAttack)
        {
            direction *= -1;
        }
        animator.SetTrigger("Hit");
        if (health <= 0)
        {
            Dead();
        }
    }
    public void Attack()
    {
        if (Physics2D.OverlapCircle(attackPoint.position, attackRange, whatIsEnemy))
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }

        if (isAttack && isAllive)
        {
            if (attackSpeed == 0)
                return;

            if (Time.time >= timeToAttack)
            {
                animator.SetTrigger("isAttack");

                Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemy);

                foreach (Collider2D enemy in hitEnemy)
                {
                    player.TakeDamage(damage);
                }
                timeToAttack = Time.time + 1 / attackSpeed;
            }
        }
    }

    public void Dead()
    {
        isAllive = false;
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
        rigidbody.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        Animation();
    }
    bool CheckWalls()
    {
        return Physics2D.OverlapCircle(wallChecker.position, 0.2f, checkLayers);
    }

    bool CheckEdge()
    {
        return Physics2D.OverlapCircle(edgeChecker.position, 0.2f, checkLayers);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallChecker.position, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(edgeChecker.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
