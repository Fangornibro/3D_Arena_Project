using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRed : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    [SerializeField] private int curHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private float speed;


    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private Transform healthBarBack, healthBarFront;
    [SerializeField] private LayerMask wallLM, playerLM;
    private Rigidbody rb;
    private float healthBarMultiplier;
    private Player player;
    private Vector3 target;
    private bool isOnPlayer = false;


    private float maxUpValue;
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        maxUpValue = transform.position.y + 8;
        rb = GetComponent<Rigidbody>();

        healthBarMultiplier = healthBarFront.localScale.x / maxHealth;
        curHealth = maxHealth;
        UpdateBars();
    }


    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, 0.5f, wallLM))
        {
            Die(false);
        }
        else if (Physics.Raycast(transform.position, transform.forward, 0.5f, playerLM))
        {
            player.GetHit(15);
            Die(false);
        }
    }

    private void FixedUpdate()
    {
        if (isOnPlayer)
        {
            target = player.transform.position;
        }
        else
        {
            target = new Vector3(transform.position.x, maxUpValue, transform.position.z);
            if (Vector3.Distance(target, transform.position) <= 0.1f)
            {
                StartCoroutine(PauseOnTheTop(1));
            }
        }

        Vector3 difference = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(difference);
        rb.MoveRotation(rotation);

        rb.velocity = (transform.forward * speed);
    }


    public bool GetHit(int damage, bool isRicochet)
    {
        curHealth -= damage;
        UpdateBars();
        if (curHealth <= 0)
        {
            player.GetPower(15);
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

    private IEnumerator PauseOnTheTop(int time)
    {
        yield return new WaitForSeconds(time);
        isOnPlayer = true;
    }
}
