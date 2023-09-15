using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyEnums;

public class QEnemyAgent : MonoBehaviour
{
  private EnemyController enemyController;
  private CombatController playerCombatController;

  private float[][] qTable;
  private float learningRate = 0.1f;
  private float discountFactor = 0.99f;

  public int current_state;
  private int state;
  private int action;
  float rewardPerEpisode = 0f;
  float reward = 0f;

  private float episodeTime = 0f;

  JointQTable JointQTable = new JointQTable();

  private float player_health;
  private int player_score;
  private int enemy_health;
  private float enemy_accuracy;

  private int currentEpisode = 0;
  private int maxEpisodes = 5;

  private int numStates = 3; // idle(0), atacando(1), chasing(2)
  private int numActions = 9; // stay(0), increase speed (1), decrease speed (2), 
                              //increase damage (3), decrease damage (4), 
                              //increase attack range (5), decrease attack range(6),
                              //increase vision range (7), decrease vision range(8) 

  public string QTableString()
  {
    string qTableString = "";

    for (int i = 0; i < this.numStates; i++)
    {
      for (int j = 0; j < this.numActions; j++)
      {
        qTableString += this.qTable[i][j].ToString();
        if (j < numActions - 1)
        {
          qTableString += ",";
        }
      }
      qTableString += "\n";
    }

    return qTableString;
  }

  public void InitializeResources()
  {
    enemyController.enemyStats.health = 100;
    enemyController.healthBar.value = 100;
    enemyController.enemyStats.accuracy = 0;
    enemyController.isAgent = true;

    //playerCombatController.playerStats.health = 100;
    //playerCombatController.healthBar.value = 100;
    //playerCombatController.isTraining = true;
  }

  public void Start()
  {
    enemyController = GetComponent<EnemyController>();
    playerCombatController = enemyController.player.GetComponent<CombatController>();

    episodeTime = 0f;

    qTable = new float[numStates][];
    for (int i = 0; i < numStates; i++)
    {
      qTable[i] = new float[numActions];
    }

    for (int i = 0; i < numStates; i++)
    {
      this.qTable[i] = new float[numActions];
      for (int j = 0; j < numActions; j++)
      {
        this.qTable[i][j] = 0;
      }
    }
  }

  private void StartNewEpisode()
  {
    Debug.Log($"AgentId {enemyController.enemyStats.id} rewardPerEpisode -> {rewardPerEpisode}");
    InitializeResources();
    episodeTime = 0f;
    rewardPerEpisode = 0f;
  }

  float CalculateReward(int state, int action, float playerHealth, float playerScore, float playerDistance, float enemyAccuracy, float enemyHealth)
  {
    float reward = 0f;

    if (state == 0) // Si el estado es "idle"
    {
      if (action == 1) // Si la acción es "increase speed"
      {
        //RECOMPENSA POR DISTANCIA
        if (playerDistance > 10f)
        {
          reward += 1f;
        }
        else
        {
          reward -= 1f;
        }
      }
    }
    else if (state == 1) // Si el estado es "atacando"
    {
      if (action == 3) // Si la acción es "increase damage"
      {
        // recompensa por jugador con baja vida
        if (playerHealth < 30f)
        {
          reward += 1f;
        }
        else
        {
          reward -= 1f;
        }
      }
    }
    else if (state == 2) // Si el estado es "chasing"
    {
      Debug.Log("yey");
      // etc...
    }

    return reward;
  }


  public int ChooseAction(float[] actionValues, float epsilon)
  {
    if (UnityEngine.Random.value < epsilon)
    {
      //Valor aleatorio
      return UnityEngine.Random.Range(0, actionValues.Length);
    }
    else
    {
      int bestAction = 0;
      float bestValue = actionValues[0];

      for (int i = 1; i < actionValues.Length; i++)
      {
        if (actionValues[i] > bestValue)
        {
          bestAction = i;
          bestValue = actionValues[i];
        }
      }

      return bestAction;
    }
  }

  private void Training()
  {
    episodeTime += Time.deltaTime;

    this.player_health = playerCombatController.healthBar.value;
    this.player_score = playerCombatController.playerStats.points;
    this.enemy_accuracy = enemyController.enemyStats.accuracy;
    this.enemy_health = enemyController.enemyStats.health;

    float[,] actionValues = new float[numStates, numActions];

    for (int state = 0; state < numStates; state++)
    {
      for (int action = 0; action < numActions; action++)
      {
        int player_score = 0;
        int player_distance = 0;
        int enemy_accuracy = 0;
        int enemy_health = 0;

        float reward = CalculateReward(state, action, player_health, player_score, player_distance, enemy_accuracy, enemy_health);
        actionValues[state, action] = reward;
      }
    }

    //chosen action

    this.qTable[state][action] += (1 - learningRate) * JointQTable.QTable[state][action] + learningRate * (reward + discountFactor * GetMaxQValue(state) - this.qTable[state][action]);
    JointQTable.FusionQTables(this.qTable);
    //ESPERE 5 SEGUNDOS 
    reward += GetMaxQValue(state);
    rewardPerEpisode += reward;
  }

  private float GetMaxQValue(int state)
  {
    float maxQValue = Mathf.Max(this.qTable[state]);
    Debug.Log(String.Format("MAX QVALUE: {0}", maxQValue));
    return maxQValue;
  }

  void Update()
  {
    this.state = this.enemyController.getStatus();
    Training();
  }

}
