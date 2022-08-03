using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private PlayerController pc;
    private PlayerStats ps;
    private GameManager gameManager;

    [SerializeField] private AudioSource collectSoundEffect;
    [SerializeField] private AudioSource collectScoreSoundEffect;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        ps = GetComponent<PlayerStats>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Heal"))
        {
            ps.currentHealth = ps.maxHealth;
            PlaySound();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Score"))
        {
            gameManager.scorePoint++;
            collectScoreSoundEffect.Play();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Speed"))
        {
            pc.upSpeed = true;
            PlaySound();
            Destroy(collision.gameObject);
        }
    }
    private void PlaySound()
    {
        collectSoundEffect.Play();
    }
}
