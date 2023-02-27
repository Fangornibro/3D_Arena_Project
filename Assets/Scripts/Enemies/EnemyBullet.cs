using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private int damage;


    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private ParticleSystem destroyParticles;
    [SerializeField] private LayerMask wallLM, playerLM;

    [HideInInspector] public Player player;
    private Rigidbody rb;

    Vector3 curPlayerPos;
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, 0.5f, wallLM))
        {
            DestroyBullet();
        }
        else if (Physics.Raycast(transform.position, transform.forward, 0.5f, playerLM))
        {
            player.MinusPower(damage);
            DestroyBullet();
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            curPlayerPos = player.transform.position;
        }
        else
        {
            if (Vector3.Distance(curPlayerPos, transform.position) <= 0.1f)
            {
                DestroyBullet();
            }
        }
        Vector3 difference = curPlayerPos - transform.position;
        Quaternion rotation = Quaternion.LookRotation(difference);
        rb.MoveRotation(rotation);

        rb.velocity = (transform.forward * speed);
    }

    private void DestroyBullet()
    {
        ParticleSystem curentDestroyParticles = Instantiate(destroyParticles, transform.position, transform.rotation);
        curentDestroyParticles.GetComponent<ParticleSystemRenderer>().material = GetComponent<MeshRenderer>().materials[0];
        curentDestroyParticles.Play();
        Destroy(gameObject);
    }
}
