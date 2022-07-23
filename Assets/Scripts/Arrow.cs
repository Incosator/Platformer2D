using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float speed;

    Rigidbody2D rigidbody2D;
    BowMan bowMan;
    PlayerController player;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        bowMan = FindObjectOfType<BowMan>();
        player = FindObjectOfType<PlayerController>();
        if (bowMan.transform.localScale.x == -2)
        {
            speed *= -1;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void Update()
    {
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(bowMan.damage);
            Destroy(gameObject);
        }
    }
}
