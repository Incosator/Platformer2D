using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rigidBody;

    [Header("Moveing")]
    [SerializeField] float runSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float doubleJumpPower;
    [SerializeField] Vector2 spawnPositin;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float timeBtwAttacks;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] float damage;
    [SerializeField] int lives = 3;

    [Header("Checkers")]
    [SerializeField] Transform checkGround;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask checkLayers;
    [SerializeField] LayerMask whatIsEnemy;

    [Header("Dashing")]
    [SerializeField] float dashingPower;
    [SerializeField] float dashingTime;
    [SerializeField] float dashCD;

    public HealthBar healthBar;
    public Hearts hearts;

    private bool canDash = true;
    private bool isDashing;
    private bool doubleJump;
    private bool isAttacking;
    private bool canAttack = true;
    private bool isDead = false;
    private float horizontal;

    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public int Lives { get => lives; set => lives = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        healthBar = GetComponent<HealthBar>();
        healthBar = FindObjectOfType<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        spawnPositin = transform.position;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        animator.SetTrigger("Hit");
        if (currentHealth <= 0)
        {
            lives -= 1;
            Healing(maxHealth);
            if (lives < 0)
            {
                Dead();
            }
        }
    }
    void Dead()
    {
        animator.SetBool("isDead", true);
        isDead = true;
        GetComponent<Collider2D>().enabled = false;
        rigidBody.velocity = Vector2.zero;
        healthBar.SetHealth(0);
    }

    public void Healing(float heal)
    {
        currentHealth += heal;
        healthBar.SetHealth(currentHealth);
    }

    void Moveing()
    {
        if (!isAttacking)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            rigidBody.velocity = new Vector2(horizontal * runSpeed, rigidBody.velocity.y);

            if (rigidBody.velocity.x > 0)
            {
                transform.localScale = new Vector3(3, 3, 3);
            }
            if (rigidBody.velocity.x < 0)
            {
                transform.localScale = new Vector3(-3, 3, 3);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (isGround())
                {
                    doubleJump = true;
                    Jump();
                }
                else if (doubleJump)
                {
                    Jump();
                    doubleJump = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine("Dash");
        }
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && isGround() && canAttack)
        {
            StartCoroutine("Attack");
        }
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        horizontal = 0;
        rigidBody.velocity = Vector2.zero;
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isAttack");
        }
        else if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("HardAttack");
        }
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemy);
        foreach (Collider2D enemy in hitEnemy)
        {
            Debug.Log("Melee hit: " + enemy.name);
            if (enemy is BoxCollider2D)
            {
                if (enemy.tag == "Bone")
                    enemy.GetComponent<Skeleton>().TakeDamge(damage);
                else if (enemy.tag == "Bow")
                    enemy.GetComponent<BowMan>().TakeDamge(damage);
            }
        }
        yield return new WaitForSeconds(timeBtwAttacks);
        isAttacking = false;
        canAttack = true;
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rigidBody.velocity = new Vector2(transform.localScale.x * dashingPower, 0);
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }

    private void FixedUpdate()
    {
        if (isDead == true)
        {
            return;
        }
        moveAnimation();
    }

    void Update()
    {
        if (isDashing || isDead == true)
        {
            return;
        }
        Moveing();
    }

    void moveAnimation()
    {
        if (!isGround())
        {
            animator.SetInteger("Jump", 1);
            if (!isGround() && rigidBody.velocity.y < 0)
            {
                animator.SetInteger("Jump", 2);
            }
        }
        if (isGround())
        {
            animator.SetInteger("Jump", 0);
            animator.SetFloat("Running", Mathf.Abs(horizontal));
        }

        if (isDashing)
        {
            animator.SetBool("Dash", true);
        }
        else if (!isDashing)
        {
            animator.SetBool("Dash", false);
        }
    }
    private bool isGround()
    {
        return Physics2D.OverlapCircle(checkGround.position, 0.2f, checkLayers);
    }
    void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, doubleJump ? jumpPower : doubleJumpPower);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.tag == "CheckPoint")
        {
            spawnPositin = transform.position;
        }
        else if (collision.tag == "Falling")
        {
            transform.position = spawnPositin;
            lives -= 1;
        }
    }
}