using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rigidBody;
    HealthBar healthBar;
    attackAnim mouseButton;

    enum attackAnim { Light, Hard }

    [Header("Moveing")]
    [SerializeField] float runSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float doubleJumpPower;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] public float damage;

    [Header("Checkers")]
    [SerializeField] Transform checkGround;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask checkLayers;
    [SerializeField] LayerMask whatIsEnemy;


    private bool canDash = true;
    private bool isDashing;
    private bool doubleJump;
    private bool isAllive = true;
    private bool isAttack = false;
    private float direction;
    private float timeToAttack = 0;

    [Header("Dashing")]
    [SerializeField] float dashingPower;
    [SerializeField] float dashingTime;
    [SerializeField] float dashCD;

    public bool IsDashing { get => isDashing; set => isDashing = value; }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = FindObjectOfType<HealthBar>();
        healthBar = GetComponent<HealthBar>();

        switch (mouseButton)
        {
            case attackAnim.Light:
                animator.SetTrigger("isAttack");
                break;
            case attackAnim.Hard:
                animator.SetTrigger("HardAttack");
                break;
        }
    }
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Move();
        
    }
    private void FixedUpdate()
    {
        Animation();

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
    void Animation()
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
            animator.SetFloat("Running", Mathf.Abs(direction));
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
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        animator.SetTrigger("Hit");
        if (currentHealth <= 0)
        {
            Dead();
        }
    }
    void Dead()
    {
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
        rigidBody.gravityScale = 0;
    }

    public void Healing(float heal)
    {
        currentHealth += heal;
        healthBar.SetHealth(currentHealth);
    }
    IEnumerator Attack()
    {
        isAttack = true;
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
            enemy.GetComponent<Skeleton>().TakeDamge(damage);
        }
        //   yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(attackSpeed);
        isAttack = false;
    }

    void Move()
    {
        if (isAllive && !isAttack)
        {
            direction = Input.GetAxis("Horizontal");
            rigidBody.velocity = new Vector2(direction * runSpeed, rigidBody.velocity.y);

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
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine("Dash");
            }
        }
        else if (isAllive && isAttack)
        {
            rigidBody.velocity = Vector2.zero;
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && isGround())
        {
            StartCoroutine("Attack");
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
}
