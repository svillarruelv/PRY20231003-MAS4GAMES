using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class Damageable_Enemy : Damageable
{
  public override void DealDamage(float damageAmount, Vector3? hitPosition = null, Vector3? hitNormal = null, bool reactToHit = true, GameObject sender = null, GameObject receiver = null)
  {
    EnemyController _enemycontroller = GetComponent<EnemyController>();
    _enemycontroller.TakeDamage((int)damageAmount);

    BoxCollider senderCollider = sender.GetComponent<BoxCollider>();
    if (senderCollider != null)
    {
      senderCollider.enabled = false;
      StartCoroutine(ReactivateCollider(senderCollider));
    }
  }

  private IEnumerator ReactivateCollider(BoxCollider collider)
  {
    yield return new WaitForSeconds(1f);
    collider.enabled = true;
  }
}