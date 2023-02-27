using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private int damage;
    [SerializeField] private int startChanceToRicochet, startChanceToGoThrough;
    private int chanceToRicochet, chanceToGoThrough;


    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private ParticleSystem destroyParticles;
    [SerializeField] private LayerMask enemy, wall;
    private Transform allEnemies;
    private Rigidbody rb;
    private IEnemy NearestEnemy = null;
    private Player player;

    List<IEnemy> nonHitableEnemies = new List<IEnemy>();

    private bool isRicochet = false;

    private void Start()
    {
        allEnemies = GameObject.Find("AllEnemies").transform;
        player = GameObject.Find("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            DestroyBullet();
        }


        if (Physics.Raycast(transform.position, transform.forward, 0.5f, wall))
        {
            DestroyBullet();
        }
        else if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 0.5f, enemy))
        {
            EnemyHit(hitInfo.collider.GetComponent<IEnemy>());
        }


        chanceToGoThrough = (int)(100 - ((double)player.curHealth / (double)player.maxHealth) * 100);
        chanceToRicochet = (int)(100 - ((double)player.curHealth / (double)player.maxHealth) * 100);
    }
    private void FixedUpdate()
    {
        if (NearestEnemy != null)
        {
            Vector3 difference = NearestEnemy.GetEnemyTransform().position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(difference);
            rb.MoveRotation(rotation);
        }
        rb.velocity = (transform.forward * speed);
    }

    private void EnemyHit(IEnemy enemy)
    {
        bool hitable = true;
        if (nonHitableEnemies.Count > 0)
        {
            foreach (IEnemy en in nonHitableEnemies)
            {
                if (en == enemy)
                {
                    hitable = false;
                }
            }
        }
        if (hitable)
        {
            if(enemy.GetHit(damage, isRicochet))
            {
                int r = Random.Range(0, 100);
                if (r <= chanceToRicochet + startChanceToRicochet)
                {
                    isRicochet = true;
                    NearestEnemy = SelectNearestEnemy(enemy);
                }
                else
                {
                    isRicochet = false;
                    r = Random.Range(0, 100);
                    if (r <= chanceToGoThrough + startChanceToGoThrough)
                    {
                        NearestEnemy = null;
                        nonHitableEnemies.Add(enemy);
                    }
                    else
                    {
                        DestroyBullet();
                    }
                }
            }
            else
            {
                DestroyBullet();
            }
        }
    }

    private void DestroyBullet()
    {
        ParticleSystem curentDestroyParticles = Instantiate(destroyParticles, transform.position, transform.rotation);
        curentDestroyParticles.GetComponent<ParticleSystemRenderer>().material = GetComponent<MeshRenderer>().materials[0];
        curentDestroyParticles.Play();
        Destroy(gameObject);
    }

    private IEnemy SelectNearestEnemy(IEnemy curEnemy)
    {
        float minDist = 999999;
        Transform minDistEnemyTransform = null;
        for (int i = 0; i < allEnemies.childCount; i++)
        {
            if (curEnemy != allEnemies.GetChild(i).GetComponent<IEnemy>())
            {
                float curDist = Vector3.Distance(allEnemies.GetChild(i).position, curEnemy.GetEnemyTransform().position);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    minDistEnemyTransform = allEnemies.GetChild(i);
                }
            }
            
        }
        if (minDistEnemyTransform != null)
        {
            return minDistEnemyTransform.GetComponent<IEnemy>();
        }
        else { return null; }
    }
}
