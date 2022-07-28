using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    [SerializeField] private float maxHealth, knockbackSpeedX, knockbackSpeedY, knockbackDuration, knockbackDeathSpeedX, knockbacDeathSpeedY, deathTorque;
    [SerializeField] private bool applyKnockBack;
    [SerializeField] private GameObject hitParticles;

    private float currentHealth, knockbackStart;

    private int playerFacingDirection;

    private bool playerOnLeft, knockback;

    private PlayerController pc;
    private GameObject aliveGO, brokenTopGO, brokenBotGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBot;
    private Animator aliveAnim;

    void Start()
    {
        currentHealth = maxHealth;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("Broken Top").gameObject;
        brokenBotGO = transform.Find("Broken Bottom").gameObject;

        aliveAnim = aliveGO.GetComponent<Animator>();
        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBot = brokenBotGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenBotGO.SetActive(false);
        brokenBotGO.SetActive(false);

    }

    void Update()
    {
        CheckKnockback();
    }
    private void Damage(float[] details)
    {
        currentHealth -= details[0];
        if (details[1] < aliveGO.transform.position.x)
        {
            playerFacingDirection = 1;
        }
        else
        {
            playerFacingDirection = -1;
        }

        Instantiate(hitParticles, aliveGO.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }
        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("damage");
        if (applyKnockBack && currentHealth > 0)
        {
            Knockback();
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        rbAlive.velocity = new Vector2(knockbackDeathSpeedX * playerFacingDirection, knockbacDeathSpeedY);
    }
    private void CheckKnockback()
    {
        if (Time.deltaTime >= knockbackStart + knockbackDuration && knockback)
        {
            knockback = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);
        }
    }
    private void Die()
    {
        aliveGO.SetActive(false);
        brokenBotGO.SetActive(true);
        brokenTopGO.SetActive(true);

        brokenBotGO.transform.position = aliveGO.transform.position;
        brokenTopGO.transform.position = aliveGO.transform.position;


        rbBrokenBot.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbacDeathSpeedY);
        rbBrokenTop.velocity = new Vector2(knockbackDeathSpeedX * playerFacingDirection, knockbacDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
    }
}
