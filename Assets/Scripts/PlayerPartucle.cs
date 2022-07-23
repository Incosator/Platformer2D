using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartucle : MonoBehaviour
{
    ParticleSystem particle;
    PlayerController playerController;

    private Collider2D collider2D;
    private Rigidbody2D rigidbody2D;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController = FindObjectOfType<PlayerController>();
        particle = GetComponent<ParticleSystem>();

        collider2D = playerController.GetComponent<Collider2D>();
        rigidbody2D = playerController.GetComponent<Rigidbody2D>();

        particle.Stop();
    }

    void Update()
    {
        if (playerController != null)
        {
            if (playerController.IsDashing)
            {
                particle.Play();
                collider2D.enabled = false;
                rigidbody2D.gravityScale = 0;
            }
            else
            {
                particle.Stop();
                collider2D.enabled = true;
                rigidbody2D.gravityScale = 1;
            }
        }
    }
}
