using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    ParticleSystem particleSystem;
    PlayerController player;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        player = GetComponent<PlayerController>();
        player = FindObjectOfType<PlayerController>();
        particleSystem.Stop();
    }

    void Update()
    {
        if (player.IsAttacking)
        {
            if (player.transform.localScale.x == 3)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (player.transform.localScale.x == -3)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            particleSystem.Play();
        }
        if (!player.IsAttacking)
        {
            particleSystem.Stop();
        }
    }
}
