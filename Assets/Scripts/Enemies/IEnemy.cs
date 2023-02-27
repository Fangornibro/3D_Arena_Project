using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    bool GetHit(int damage, bool isRicochet);
    void Die(bool killed);
    Transform GetEnemyTransform();
}
