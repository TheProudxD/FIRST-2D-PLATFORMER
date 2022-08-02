using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private float respawnTime;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI health;

    [HideInInspector] public float scorePoint;

    private float respawnTimeStart;

    private bool respawn;

    private PlayerController pc;
    private PlayerStats ps;
    private ItemCollector itemCollector;

    private CinemachineVirtualCamera CVC;

    private void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        ps = GameObject.Find("Player").GetComponent<PlayerStats>();
        itemCollector = GameObject.Find("Player").GetComponent<ItemCollector>();

        CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        CheckRespawn();

        score.SetText("Score: " + scorePoint.ToString());
        health.SetText("Health: " + ps.currentHealth.ToString());
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRespawn()
    {
        if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            //var playerTemp = Instantiate(player, respawnPoint);
            //CVC.m_Follow = playerTemp.transform;
            respawn = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
