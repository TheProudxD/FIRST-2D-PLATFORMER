using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float damage;
    private PlayerStats playerStats;
    private void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            playerStats.currentHealth -= damage;
        }
    }
}
