using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyEnums;

public class EnemyController : MonoBehaviour, IStatsDataProvider
{
  public AttackStates _attackState;
  private AudioSource audioSource;
  public AudioClip hurtSound;
  public AudioClip deathSound;

  public StatsData enemyStats = new StatsData();

  public Animator enemyAnimator;

  [SerializeField]
  private GameObject weapon;
  public float chasingRange = 10f;
  public float attackRange = 3f;
  public float speedRange = 0.009f;
  private int damage;

  public GameObject player;
  public bool isAttacking = false;
  public bool isChasing = false;
  private Coroutine attackCoroutine;
  public bool isAgent = false;
  public float playerDistance = 0f;

  [SerializeField]
  public Slider healthBar;
  public StatsData GetStatsData()
  {
    return enemyStats;
  }

  public Vector3 GetPosition()
  {
    return transform.position;
  }

  public float GetMainMetric()
  {
    return enemyStats.GetAccuracy();
  }

  void Start()
  {
    enemyAnimator = GetComponent<Animator>();

    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      audioSource = gameObject.AddComponent<AudioSource>();
    }

    enemyStats.health = 100;
    damage = 10;

    healthBar.maxValue = enemyStats.health;
    healthBar.value = enemyStats.health;

    attackRange = weapon.GetComponent<WeaponController>().weaponData.range;
    _attackState = AttackStates.NO_ATTACK;
  }

  void Update()
  {
    if (enemyStats.health <= 0) return;
    playerDistance = Vector3.Distance(transform.position, player.transform.position);
    healthBar.gameObject.SetActive(true);
    healthBar.transform.LookAt(new Vector3(player.transform.position.x, healthBar.transform.position.y, player.transform.position.z));
    if (Vector3.Distance(transform.position, player.transform.position) < chasingRange)
    {
      transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
    }

    if (Vector3.Distance(transform.position, player.transform.position) > attackRange && Vector3.Distance(transform.position, player.transform.position) < chasingRange && !isAttacking)
    {
      enemyAnimator.SetBool("isMoving", true);
      this.isChasing = true;
      transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), speedRange);
    }
    else if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
    {
      enemyAnimator.SetBool("isMoving", false);
      this.isChasing = false;
      Attack();
    }


  }

  public int getState()
  {
    if (isAttacking)
    {
      return 2;
    }
    else if (enemyAnimator.GetBool("isMoving"))
    {
      return 1;
    }
    else
    {
      return 0; //IDLE
    }
  }

  public void Attack()
  {
    var playerToHit = player;

    if (!isAttacking)
    {
      isAttacking = true;
      enemyStats.HitAttempt();
      enemyAnimator.SetBool("isAttacking", true);

      attackCoroutine = StartCoroutine(AttackCoroutine(playerToHit));

      StartCoroutine(Utility.TimedEvent(() =>
      {
        isAttacking = false;
        StopCoroutine(attackCoroutine);
      }, 2.5f));
    }
  }

  private IEnumerator AttackCoroutine(GameObject playerToHit)
  {
    yield return new WaitForSeconds(0.9f);

    if (Vector3.Distance(transform.position, player.transform.position) <= attackRange + 1)
    {
      this.enemyStats.HitSuccess();
      _attackState = AttackStates.SUCCESS;

      playerToHit.GetComponent<CombatController>().TakeDamage(damage, this);
    }
    else
    {
      _attackState = AttackStates.FAIL;
    }


  }

  public void TakeDamage(int damage)
  {
    healthBar.value -= damage;
    enemyStats.health -= damage;
    enemyAnimator.SetBool("isHit", true);

    if (isAttacking) StopCoroutine(attackCoroutine);
    player.GetComponent<CombatController>().playerStats.points += 10;
    player.GetComponent<CombatController>().UpdateScoreText();
    audioSource.PlayOneShot(hurtSound);

    if (enemyStats.health <= 0)
    {
      player.GetComponent<CombatController>().playerStats.points += 100;
      player.GetComponent<CombatController>().UpdateScoreText();

      audioSource.PlayOneShot(deathSound);
      if (!isAgent)
      {
        enemyAnimator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        player = null;
        Destroy(gameObject, 4f);
      }
    }
  }
}
