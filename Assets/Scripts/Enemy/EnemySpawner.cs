using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
  public GameObject enemyPrefab;
  public GameObject playerObject;

  public Transform[] spawnPoints;
  public int maxEnemiesPerSpawnPoint = 1;
  private bool areEnemiesDefeated = false;
  private bool areSpawning = false;
  public int hordeNumber = 1;

  private void Start()
  {
    Debug.Log("Enemies spawned");
    SpawnInitialEnemies();
  }

  private void Update()
  {
    if (areEnemiesDefeated && !areSpawning)
    {
      areSpawning = true;
      StartCoroutine(SpawnNewEnemiesCoroutine());
      Debug.Log($"New horde #{hordeNumber}");
    }
    CheckIfEnemiesDefeated();
  }

  private void SpawnInitialEnemies()
  {
    foreach (Transform spawnPoint in spawnPoints)
    {
      for (int i = 0; i < maxEnemiesPerSpawnPoint; i++)
      {
        SpawnEnemy(spawnPoint);
      }
    }
  }

  private IEnumerator SpawnNewEnemiesCoroutine()
  {
    hordeNumber++;
    playerObject.GetComponent<CombatController>().playerStats.hordeNumber = hordeNumber;
    yield return new WaitForSeconds(3f); // Esperar un frame extra antes de verificar si los enemigos están derrotados

    foreach (Transform spawnPoint in spawnPoints)
    {
      SpawnEnemy(spawnPoint);
    }

    areSpawning = false;
    areEnemiesDefeated = false;


    CheckIfEnemiesDefeated();
  }

  private void SpawnEnemy(Transform spawnPoint)
  {
    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

    // Obtener el componente EnemyController del objeto enemigo
    EnemyController enemyController = enemy.GetComponent<EnemyController>();

    // Modificar la variable player
    enemyController.player = playerObject;
    enemyController.enemyStats.hordeNumber = hordeNumber;
  }

  private int GetEnemyCount()
  {
    EnemyController[] enemies = FindObjectsOfType<EnemyController>();
    return enemies.Length;
  }

  private void CheckIfEnemiesDefeated()
  {
    if (GetEnemyCount() == 0)
    {
      areEnemiesDefeated = true;
    }
  }

  private void OnDestroy()
  {
    StopAllCoroutines();
  }
}
