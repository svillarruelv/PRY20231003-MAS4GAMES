using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyEnums;

public class EnemyController : MonoBehaviour, IStatsDataProvider
{
  public AttackStates _attackState; //Outcome
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
  private Coroutine attackCoroutine;
  public bool isAgent = false;

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
    // Small values for easier testing
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

    healthBar.gameObject.SetActive(true);
    healthBar.transform.LookAt(new Vector3(player.transform.position.x, healthBar.transform.position.y, player.transform.position.z));
    if (Vector3.Distance(transform.position, player.transform.position) < chasingRange)
    {
      transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
    }

    if (Vector3.Distance(transform.position, player.transform.position) > attackRange && Vector3.Distance(transform.position, player.transform.position) < chasingRange && !isAttacking)
    {
      // BUSCA AL JUGADOR
      enemyAnimator.SetBool("isMoving", true);
      transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), speedRange);

#if UNITY_EDITOR
        // Record that the enemy is moving
        FileManager.Instance.WriteAction(FileManager.ActionType.MOVE,
                                            FileManager.ActionResult.SUCCESS,
                                            FileManager.CharacterType.ENEMY,
                                            this.GetComponent<IStatsDataProvider>(),
                                            player.GetComponent<IStatsDataProvider>());
#endif
    }
    else if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
    {
      enemyAnimator.SetBool("isMoving", false);
      Attack();
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
#if UNITY_EDITOR
      FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                      FileManager.ActionResult.SUCCESS,
                                      FileManager.CharacterType.ENEMY,
                                      this.GetComponent<IStatsDataProvider>(),
                                      playerToHit.GetComponent<IStatsDataProvider>());
#endif
      _attackState = AttackStates.SUCCESS;

      playerToHit.GetComponent<CombatController>().TakeDamage(damage, this);
    }
#if UNITY_EDITOR
    else
    {
      FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                      FileManager.ActionResult.FAIL,
                                      FileManager.CharacterType.ENEMY,
                                      this.GetComponent<IStatsDataProvider>(),
                                      playerToHit.GetComponent<IStatsDataProvider>());
      _attackState = AttackStates.FAIL;
    }
#endif

  }

  public void TakeDamage(int damage)
  {
    healthBar.value -= damage;
    enemyStats.health -= damage;
    enemyAnimator.SetBool("isHit", true);

    if (isAttacking) StopCoroutine(attackCoroutine);
    player.GetComponent<CombatController>().playerStats.points += 10; // Player gets 10 points for attacking the enemy
    player.GetComponent<CombatController>().UpdateScoreText();
    audioSource.PlayOneShot(hurtSound);
#if UNITY_EDITOR
    // Record that PLAYER attacked the ENEMY
    FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                        FileManager.ActionResult.SUCCESS,
                                        FileManager.CharacterType.PLAYER,
                                        player.GetComponent<IStatsDataProvider>(),
                                        this.GetComponent<IStatsDataProvider>());
    // Record that the enemy was attacked
    FileManager.Instance.WriteAction(FileManager.ActionType.HURT,
                                        FileManager.ActionResult.SUCCESS,
                                        FileManager.CharacterType.ENEMY,
                                        this.GetComponent<IStatsDataProvider>(),
                                        player.GetComponent<IStatsDataProvider>());
#endif
    if (enemyStats.health <= 0)
    {
      player.GetComponent<CombatController>().playerStats.points += 100; // Player gets 100 points for killing the enemy
      player.GetComponent<CombatController>().UpdateScoreText();
#if UNITY_EDITOR
      // Record that the enemy was attacked and killed
       FileManager.Instance.WriteAction(FileManager.ActionType.HURT,
                                          FileManager.ActionResult.DEAD,
                                          FileManager.CharacterType.ENEMY,
                                          this.GetComponent<IStatsDataProvider>(),
                                          player.GetComponent<IStatsDataProvider>());
#endif
      audioSource.PlayOneShot(deathSound);
      if (!isAgent)
      {
        enemyAnimator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        player = null;
        Destroy(gameObject, 4f);
      }
    }
  }
}
