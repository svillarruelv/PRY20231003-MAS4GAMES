using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IStatsDataProvider
{
  public StatsData enemyStats = new StatsData();

  private Animator enemyAnimator;

  [SerializeField]
  private GameObject weapon;
  private float chasingRange = 8f;
  private float attackRange;
  private int damage;

  public GameObject player;
  private bool isAttacking = false;

  [SerializeField]
  private Slider healthBar;

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

    //small values for eisier testing
    enemyStats.health = 100;
    damage = 10;

    healthBar.maxValue = enemyStats.health;
    healthBar.value = enemyStats.health;

    attackRange = weapon.GetComponent<WeaponController>().weaponData.range;
    //damage = weapon.GetComponent<WeaponController>().weaponData.damage;
  }

  void Update()
  {
    if (enemyStats.health <= 0) return;

    healthBar.gameObject.SetActive(true);
    healthBar.transform.LookAt(new Vector3(player.transform.position.x, healthBar.transform.position.y, player.transform.position.z));

    if (Vector3.Distance(transform.position, player.transform.position) > attackRange && Vector3.Distance(transform.position, player.transform.position) < chasingRange)
    {
      //BUSCA AL JUGADOR
      transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
      transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), 0.003f);

      //Record that the enemy is moving
      FileManager.Instance.WriteAction(FileManager.ActionType.MOVE,
                                            FileManager.ActionResult.SUCCESS,
                                            FileManager.CharacterType.ENEMY,
                                            this.GetComponent<IStatsDataProvider>(),
                                            player.GetComponent<IStatsDataProvider>());
    }
    else if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
    {
      Attack();
    }
  }

  private void Attack()
  {
    var playerToHit = player;

    isAttacking = true;
    this.enemyStats.HitAttempt();
    enemyAnimator.SetBool("isAttacking", true);

    StartCoroutine(Utility.TimedEvent(() =>
    {
      if (Vector3.Distance(transform.position, player.transform.position) <= attackRange + 1)
      {
        this.enemyStats.HitSuccess();
        FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                          FileManager.ActionResult.SUCCESS,
                                          FileManager.CharacterType.ENEMY,
                                          this.GetComponent<IStatsDataProvider>(),
                                          playerToHit.GetComponent<IStatsDataProvider>());

        playerToHit.GetComponent<CombatController>().TakeDamage(damage, this);
      }
      else
      {
        FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                            FileManager.ActionResult.FAIL,
                                            FileManager.CharacterType.ENEMY,
                                            this.GetComponent<IStatsDataProvider>(),
                                            playerToHit.GetComponent<IStatsDataProvider>());
      }
    }, 1f));

    StartCoroutine(Utility.TimedEvent(() =>
    {
      isAttacking = false;
    }, 2.5f));
  }

  public void TakeDamage(int damage)
  {
    healthBar.value -= damage;
    enemyStats.health -= damage;
    enemyAnimator.SetBool("isHit", true);
    player.GetComponent<CombatController>().playerStats.points += 10; //Player gets 10 points for attacking the enemy

    //Record that PLAYER attacked the ENEMY
    FileManager.Instance.WriteAction(FileManager.ActionType.ATTACK,
                                            FileManager.ActionResult.SUCCESS,
                                            FileManager.CharacterType.PLAYER,
                                            player.GetComponent<IStatsDataProvider>(),
                                            this.GetComponent<IStatsDataProvider>());

    if (enemyStats.health <= 0)
    {
      player.GetComponent<CombatController>().playerStats.points += 100; //Player gets 100 points for attacking the enemy

      //Record that the enemy was attacked and killed
      FileManager.Instance.WriteAction(FileManager.ActionType.HURT,
                                            FileManager.ActionResult.DEAD,
                                            FileManager.CharacterType.ENEMY,
                                            this.GetComponent<IStatsDataProvider>(),
                                            player.GetComponent<IStatsDataProvider>());

      GetComponent<Collider>().enabled = false;
      enemyAnimator.SetBool("isDead", true);
      player = null;

      Destroy(gameObject, 4f);
    }

    //Record that the enemy was attacked
    FileManager.Instance.WriteAction(FileManager.ActionType.HURT,
                                            FileManager.ActionResult.SUCCESS,
                                            FileManager.CharacterType.ENEMY,
                                            this.GetComponent<IStatsDataProvider>(),
                                            player.GetComponent<IStatsDataProvider>());
  }
}
