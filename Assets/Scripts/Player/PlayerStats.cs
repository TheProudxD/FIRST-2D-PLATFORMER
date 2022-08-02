using UnityEngine;

public class PlayerStats : MonoBehaviour
{
   
    [SerializeField] private GameObject deathChunkParticle, deathBloodParticle;

    public float currentHealth;

    private GameManager gameManager;

    public float maxHealth;
    void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        CheckDie();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
    }
    private void CheckDie()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        gameManager.Respawn();
        currentHealth = maxHealth;
        Destroy(gameObject);
    }
}
