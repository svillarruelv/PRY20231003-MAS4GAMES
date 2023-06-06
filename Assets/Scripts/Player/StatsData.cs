using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsData
{
  public int id;
  public int health;
  public float accuracy;
  public int hit_success;
  public int hit_attempts;
  public StatsData()
  {
    System.Random random = new System.Random();
    health = 100;
    id = random.Next();
    hit_success = 0;
    hit_attempts = 0;
  }
  public float GetAccuracy()
  {
    if (hit_attempts == 0) return 1f;
    accuracy = (float)hit_success / (float)hit_attempts * 100f;
    return accuracy;
  }
  public void HitAttempt()
  {
    hit_attempts++;
  }
  public void HitSuccess()
  {
    hit_success++;
  }
}
