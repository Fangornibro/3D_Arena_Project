using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;
    public int curPower, maxPower;
    public int maxHealth, curHealth;
    [SerializeField] private float timeBetweenShots;
    private float curTimeBetweenShots;
    private bool isShoot;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float groundDrag;
    private int score = 0;


    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private CameraScript mainCamera;
    [SerializeField] private Bullet bulletPrefab;
    public Transform head;
    [SerializeField] private Joystick joystick;
    [SerializeField] private RectTransform powerBar, healthBar;
    [SerializeField] private TextMeshProUGUI powerText, healthText;
    [SerializeField] private Button ultiButton;
    [SerializeField] private Transform allEnemies;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform enemiesBullets;


    [Space]
    [Space]
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI looseText;
    [SerializeField] private Retry retryUI;

    float healthBarMultiplier, powerBarMultiplier;

    private Rigidbody rb;
    private Vector3 direction;
    private bool onGround;

    private float horizontalInput, verticalInput;
    private void Start()
    {
        ultiButton.interactable = false;


        healthBarMultiplier = healthBar.sizeDelta.x / maxHealth;
        powerBarMultiplier = powerBar.sizeDelta.x / maxPower;
        curHealth = maxHealth;
        curPower = maxPower / 2;
        UpdateBars();


        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        if (isShoot)
        {
            curTimeBetweenShots += Time.deltaTime;
            if (curTimeBetweenShots >= timeBetweenShots)
            {
                isShoot = false;
                curTimeBetweenShots = 0;
            }
        }


        horizontalInput = joystick.Horizontal;
        verticalInput = joystick.Vertical;


        onGround = Physics.Raycast(transform.position, Vector3.down, transform.lossyScale.y + 0.2f, ground);

        if (onGround)
        {
            rb.drag = groundDrag;
        }
        else
        {
            TelepotToRandomPos();
        }


        Vector3 xzOnlyVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (xzOnlyVelocity.magnitude > speed)
        {
            Vector3 limitVelocity = xzOnlyVelocity.normalized * speed;
            rb.velocity = new Vector3(limitVelocity.x, 0, limitVelocity.z);
        }
    }
    private void FixedUpdate()
    {
        direction = head.transform.forward * verticalInput + head.transform.right * horizontalInput;
        rb.AddForce(direction.normalized * speed, ForceMode.Force);
    }
    public void Shot()
    {
        if (!isShoot)
        {
            Instantiate(bulletPrefab, head.position + head.forward / 4, head.transform.rotation, transform);
            isShoot = true;
        }
    }

    private void UpdateBars()
    {
        healthBar.sizeDelta = new Vector2((float)curHealth * healthBarMultiplier, healthBar.sizeDelta.y);
        healthText.SetText("Health:" + curHealth);
        powerBar.sizeDelta = new Vector2 ((float)curPower * powerBarMultiplier, powerBar.sizeDelta.y);
        powerText.SetText("Power:" + curPower);
    }

    public void GetHit(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            curHealth = 0;
            retryUI.Open();
            looseText.SetText("You died. Score: " + score);
        }
        UpdateBars();
    }

    public void GetHeal(int health)
    {
        curHealth += health;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
        UpdateBars();
    }

    public void GetPower(int power)
    {
        curPower += power;
        if (curPower >= maxPower)
        {
            curPower = maxPower;
            ultiButton.interactable = true;
        }
        UpdateBars();
    }

    public void MinusPower(int power)
    {
        curPower -= power;
        if (curPower < 0)
        {
            curPower = 0;
            ultiButton.interactable = false;
        }
        UpdateBars();
    }

    public void Ulti()
    {
        MinusPower(100);
        for (int i = 0; i < allEnemies.childCount; i++)
        {
            allEnemies.GetChild(i).GetComponent<IEnemy>().Die(true);
        }
    }

    public void PlusScore(int plusScore)
    {

        score += plusScore;
    }

    private void TelepotToRandomPos()
    {
        for (int i = 0; i < enemiesBullets.childCount; i++)
        {
            try
            {
                EnemyBullet curEnemyBullet = enemiesBullets.GetChild(i).GetComponent<EnemyBullet>();
                curEnemyBullet.player = null;
            }
            catch { }
        }
        float curDist, minDist;
        Vector3 pos = Vector3.zero;
        for (int j = 0; j < 50; j++)
        {
            minDist = 9999;
            pos = Random.insideUnitCircle * floor.lossyScale.x / 2;
            pos = new Vector3(pos.x, 0, pos.y);
            for (int i = 0; i < allEnemies.childCount; i++)
            {
                curDist = Vector3.Distance(allEnemies.GetChild(i).position, pos);
                if (curDist < minDist)
                {
                    minDist = curDist;
                }
            }
            if (minDist >= 20)
            {
                break;
            }
        }
        transform.position = pos;
    }
}
