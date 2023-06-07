using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class Damageable_Enemy : Damageable
{
    public override void DealDamage(float damageAmount, Vector3? hitPosition = null, Vector3? hitNormal = null, bool reactToHit = true, GameObject sender = null, GameObject receiver = null) {
        EnemyController _enemycontroller = GetComponent<EnemyController>();
        _enemycontroller.TakeDamage((int)damageAmount);
    }
}
