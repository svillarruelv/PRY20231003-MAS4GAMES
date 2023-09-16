using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyEnums;

public class QEnemyAgent : MonoBehaviour
{
  private EnemyController enemyController;
  private CombatController playerCombatController;

  private float learningRate = 0.1f;
  private float discountFactor = 0.99f;

  public int current_state;
  private int state;
  private int action;

  private float player_health;
  private int player_score;
  private int enemy_health;
  private float enemy_accuracy;
  private float player_distance;

  private float[,] qTable;
  private float episodeTime = 0f;
  private float totalTime = 0f;
  private float rewardPerEpisode = 0f;
  private float currentReward = 0f;
  private int currentEpisode = 0;
  private int maxEpisodes = 5;
  private int currentStep = 0;
  private int totalSteps = 0;
  private int maxStep = 10000;
  private int numStates = 3; // idle(0), atacando(1), chasing(2)
  private int numActions = 9; // stay(0), increase speed (1), decrease speed (2), 
                              //increase damage (3), decrease damage (4), 
                              //increase attack range (5), decrease attack range(6),
                              //increase vision range (7), decrease vision range(8) 
  private float maxQValue = 0f;
  private bool flagTriggerDead = true;
  public string QTableString()
  {
    string qTableString = "";

    for (int i = 0; i < this.numStates; i++)
    {
      for (int j = 0; j < this.numActions; j++)
      {
        qTableString += this.qTable[i, j].ToString();
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
    this.flagTriggerDead = true;
    //playerCombatController.playerStats.health = 100;
    //playerCombatController.healthBar.value = 100;
    //playerCombatController.isTraining = true;
  }

  public void Start()
  {
    enemyController = GetComponent<EnemyController>();
    playerCombatController = enemyController.player.GetComponent<CombatController>();

    episodeTime = 0f;

    enemyController.isAgent = true;

    qTable = new float[numStates, numActions];

    for (int i = 0; i < numStates; i++)
    {
      for (int j = 0; j < numActions; j++)
      {
        this.qTable[i, j] = 0;
      }
    }
  }

  private void StartNewEpisode()
  {
    Debug.Log($"AgentId {enemyController.enemyStats.id} rewardPerEpisode -> {rewardPerEpisode}");
    Debug.Log($"FILE UPDATED!");
    totalTime += episodeTime;
    totalSteps += currentStep;
#if UNITY_EDITOR
    FileManager.Instance.WriteStep(totalSteps, enemyController.GetComponent<IStatsDataProvider>(), rewardPerEpisode, totalTime);
#endif
    InitializeResources();
    episodeTime = 0f;
    currentStep = 0;
    rewardPerEpisode = 0f;
  }

  float CalculateReward(int state, int action, float playerDistance, float playerHealth, float playerScore, float enemyAccuracy, float enemyHealth)
  {
    float reward = 0f;

    switch (state)
    {
      case 0: // Estado "idle"
        switch (action)
        {
          case 1: // Acción "increase speed"
                  // Recompensa por distancia: Cuanto más cerca del jugador, mejor
            reward = Mathf.Clamp(10f - playerDistance, -1f, 1f);
            break;
          case 2: // Acción "decrease speed"
                  // Recompensa por distancia: Cuanto más lejos del jugador, mejor
            reward = Mathf.Clamp(playerDistance - 10f, -1f, 1f);
            break;
          case 3: // Acción "increase damage"
                  // Recompensa por puntaje del jugador: Cuanto más alto, mejor
            reward = Mathf.Clamp((playerScore + 1f) / 100f, -1f, 1f);
            break;
          case 4: // Acción "decrease damage"
                  // Recompensa por puntaje del jugador: Cuanto más bajo, mejor
            reward = Mathf.Clamp(1f - (playerScore + 1f) / 100f, -1f, 1f);
            break;
          default:
            // Otras acciones en estado "idle" no tienen recompensa
            reward = 0f;
            break;
        }
        break;

      case 1: // Estado "atacando"
        switch (action)
        {
          case 5: // Acción "increase attack range"
                  // Recompensa por precisión del enemigo: Cuanto más precisa, mejor
            reward = Mathf.Clamp(enemyAccuracy, -1f, 1f);
            break;
          case 6: // Acción "decrease attack range"
                  // Recompensa por precisión del enemigo: Cuanto menos precisa, mejor
            reward = Mathf.Clamp(1f - enemyAccuracy, -1f, 1f);
            break;
          default:
            // Otras acciones en estado "atacando" no tienen recompensa
            reward = 0f;
            break;
        }
        break;

      case 2: // Estado "chasing"
        switch (action)
        {
          case 7: // Acción "increase vision range"
                  // Recompensa por salud del enemigo: Cuanto más salud tenga el enemigo, mejor
            reward = Mathf.Clamp((enemyHealth + 1f) / 100f, -1f, 1f);
            break;
          case 8: // Acción "decrease vision range"
                  // Recompensa por salud del enemigo: Cuanto menos salud tenga el enemigo, mejor
            reward = Mathf.Clamp(1f - (enemyHealth + 1f) / 100f, -1f, 1f);
            break;
          default:
            // Otras acciones en estado "chasing" no tienen recompensa
            reward = 0f;
            break;
        }
        break;

      default:
        // Otros estados no tienen recompensa
        reward = 0f;
        break;
    }

    return reward;
  }



  public int ChooseAction(float[,] qTable, int state, float epsilon)
  {
    int numActions = qTable.GetLength(1);

    if (UnityEngine.Random.value < epsilon)
    {
      return UnityEngine.Random.Range(0, numActions);
    }
    else
    {
      int bestAction = 0;
      float bestValue = qTable[state, 0];

      for (int action = 1; action < numActions; action++)
      {
        if (qTable[state, action] > bestValue)
        {
          bestAction = action;
          bestValue = qTable[state, action];
          this.maxQValue = bestValue;
        }
      }

      return bestAction;
    }
  }


  private void Training()
  {
    episodeTime += Time.deltaTime;
    if (this.currentStep == maxStep)
    {
      StartNewEpisode();
      return;
    }

    this.currentStep += 1;

    if (this.enemy_health > 0)
    {

      for (int state = 0; state < numStates; state++)
      {
        for (int action = 0; action < numActions; action++)
        {
          float reward = CalculateReward(state, action, this.player_health, this.player_score, this.player_distance, this.enemy_accuracy, this.enemy_health);

          float newQValue = reward + learningRate * (reward + discountFactor * this.maxQValue - JointQTable.Instance.GetQValue(state, action));

          // Aplicar la normalización
          newQValue = Mathf.Clamp(newQValue, -1f, 1f);

          // Actualizar la tabla Q con el nuevo valor normalizado
          this.qTable[state, action] = newQValue;
        }
      }

      int bestAction = ChooseAction(this.qTable, this.state, 0.5f);

      JointQTable.Instance.FusionQTables(this.qTable);
      //ESPERE 5 SEGUNDOS 
      Debug.Log($"MAX QVALUE: {this.maxQValue} ACTION: {bestAction} REWARD: {currentReward} EPISODE REWARD {rewardPerEpisode}");
      currentReward += this.maxQValue;
      rewardPerEpisode += this.maxQValue;
    }
    else
    {
      if (this.flagTriggerDead)
      {
        this.maxQValue = -10f;
        currentReward += this.maxQValue;
        rewardPerEpisode += this.maxQValue;
        this.flagTriggerDead = false;
      }
    }

  }

  private void GetObservations()
  {
    this.player_health = playerCombatController.healthBar.value;
    this.player_score = playerCombatController.playerStats.points;
    this.enemy_accuracy = enemyController.enemyStats.accuracy;
    this.enemy_health = enemyController.enemyStats.health;
    this.player_distance = enemyController.playerDistance;
    this.state = this.enemyController.getState();
  }

  void Update()
  {
    if (JointQTable.Instance.IsTableInitialized())
    {
      GetObservations();
      Training();
    }
  }

}
