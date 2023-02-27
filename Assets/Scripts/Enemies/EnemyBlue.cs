using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBlue : MonoBehaviour, IEnemy
{

    [Header("Stats")]
    [SerializeField] private int curHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private float timeBetweenAttacks;
    private float curTimeBetweenAttacks = 0;
    [SerializeField] private float attackRange;
    private bool isAtack = false, isPlayerInRange = false;

    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private Transform healthBarBack, healthBarFront;
    [SerializeField] private LayerMask playerLM, groundLM;
    [SerializeField] private EnemyBullet bullet;
    private Transform enemiesBullets;

    private bool onGround = false;

    private float healthBarMultiplier;
    private NavMeshAgent agent;
    private Player player;
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        enemiesBullets = GameObject.Find("EnemiesBullets").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        healthBarMultiplier = healthBarFront.localScale.x / maxHealth;
        curHealth = maxHealth;
        UpdateBars();
    }

    private void Update()
    {
        if (!agent.enabled)
        {
            onGround = Physics.Raycast(transform.position, Vector3.down, transform.lossyScale.y + 0.2f, groundLM);

            if (onGround)
            {
                agent.enabled = true;
            }
        }


        isPlayerInRange = Physics.CheckSphere(transform.position, attackRange, playerLM);

        if (agent.enabled)
        {
            if (isPlayerInRange)
            {
                Attack();
            }
            else
            {
                Move();
            }
        }
    }

    public bool GetHit(int damage, bool isRicochet)
    {
        curHealth -= damage;
        UpdateBars();
        if (curHealth <= 0)
        {
            player.GetPower(50);
            if (isRicochet)
            {
                if (Random.Range(0, 1) == 0)
                {
                    player.GetHeal(player.maxHealth / 2);
                }
                else
                {
                    player.GetPower(10);
                }
            }
            Die(true);
            return true;
        }
        return false;
    }

    private void UpdateBars()
    {
        healthBarFront.localScale = new Vector2((float)curHealth * healthBarMultiplier, healthBarFront.localScale.y);
    }

    public void Die(bool killed)
    {
        ParticleSystem part = Instantiate(deathParticles, transform.position, transform.rotation);
        part.GetComponent<ParticleSystemRenderer>().material = GetComponent<MeshRenderer>().materials[0];
        part.Play();
        if (killed)
        {
            player.PlusScore(1);
        }
        Destroy(gameObject);
    }

    public Transform GetEnemyTransform()
    {
        return transform;
    }

    public void Attack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform);

        if (!isAtack)
        {
            Instantiate(bullet, transform.position, Quaternion.identity, enemiesBullets);
            isAtack = true;
        }
        else
        {
            curTimeBetweenAttacks += Time.deltaTime;
            if (curTimeBetweenAttacks >= timeBetweenAttacks)
            {
                isAtack = false;
                curTimeBetweenAttacks = 0;
            }
        }
    }

    public void Move()
    {
        agent.SetDestination(player.transform.position);
    }
}
