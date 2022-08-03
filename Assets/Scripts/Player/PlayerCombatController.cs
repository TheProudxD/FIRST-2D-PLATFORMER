using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    [SerializeField] private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField] private Transform attack1HitBoxPos;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private AudioSource fight1SoundEffect;
    [SerializeField] private AudioSource fight2SoundEffect;
    [SerializeField] private AudioSource punchSoundEffect;

    private bool gotInput, isAttacking, isFirstAttack;
    private float lastInputTime = Mathf.NegativeInfinity;
    private float[] attackDetails = new float[2];
    private Animator anim;
    private PlayerController PC;
    private PlayerStats PS;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStats>();
    }


    void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }
    private void CheckCombatInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }
    private void CheckAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
                if (Random.Range(0,2)==0)
                    fight1SoundEffect.Play();
                else
                    fight2SoundEffect.Play();
            }
        }
        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }

    }
    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;
        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            punchSoundEffect.Play();
        }
    }
    private void Damage(float[] attackDetails)
    {
        if (!PC.GetDashStation())
        {
            int direction;

            PS.DecreaseHealth(attackDetails[0]);

            if (attackDetails[1] < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            PC.Knockback(direction);
        }
    }
    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
