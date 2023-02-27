using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float SpawnTime = 5;
    private float NumberOfEnemies = 1;
    [SerializeField] private List<GameObject> EnemyTypesRatio;


    [Space]
    [Space]
    [Header("Initialisation")]
    [SerializeField] private Transform allEnemies;
    [SerializeField] private Transform floor;
    void Start()
    {
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        Vector3 pos;
        while (true)
        {
            for (int i = 0; i < NumberOfEnemies; i++)
            {
                foreach (GameObject enemy in EnemyTypesRatio)
                {
                    pos = Random.insideUnitCircle * floor.lossyScale.x / 2;
                    pos = new Vector3(pos.x, 20, pos.y);
                    Instantiate(enemy, pos, Quaternion.identity, allEnemies);
                }
            }
            if (SpawnTime > 2)
            {
                SpawnTime -= 0.5f;
            }
            else
            {
                NumberOfEnemies++;
            }
            yield return new WaitForSeconds(SpawnTime);
        }
    }
}
