using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionSpawner : MonoBehaviour
{
  public GameObject potionPrefab;
  public Transform[] spawnPoints;
  public int maxPotionsPerSpawnPoint = 1;

  private void Start()
  {
    SpawnPotions();
  }

  private void Update() { }

  public void SpawnPotions()
  {
    foreach (Transform spawnPoint in spawnPoints)
    {
      for (int i = 0; i < maxPotionsPerSpawnPoint; i++)
      {
        SpawnPotion(spawnPoint);
      }
    }
  }

  private void SpawnPotion(Transform spawnPoint)
  {
    GameObject potion = Instantiate(potionPrefab, spawnPoint.position, spawnPoint.rotation);
  }

  private void OnDestroy()
  {
    StopAllCoroutines();
  }
}
