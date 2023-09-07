using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyEnums;

public class QEnemyAgent : MonoBehaviour
{
  public GameObject player;
  private EnemyController enemyController;
  private CombatController playerCombatController;

  private float[][] QTable; // QTable from the Agent
                            // QTable: filas (i) = estado, columnas (j) = acción
  private float learningRate = 0.1f; // Learning Rate
  private float discountFactor = 0.99f; // Discount Factor

  public int current_state; //Estado actual
  private int state; //Estado
  private int action; //Acción
  float rewardPerEpisode = 0f; //Recompensa
  float reward = 0f; //Recompensa

  private float episodeTime = 0f; //Tiempo del episodio de entrenamiento

  JointQTable JointQTable = new JointQTable(); //Joint QTable: tabla con la información/conocimiento de todos los agentes

  //Metrics:
  private float player_health;
  private int player_score;
  private int enemy_health;
  private float enemy_accuracy;

  public void Start()
  {
    enemyController = GetComponent<EnemyController>();
    playerCombatController = enemyController.player.GetComponent<CombatController>();

    episodeTime = 0f;

    // Inicializar la QTable con valores aleatorios
    int numStates = 3; // idle(0), atacando(1), moviendo(2)
    int numActions = 3; // variar vel(0), daño atk(1),  chasing range(2)
    QTable = new float[numStates][];

    for (int i = 0; i < numStates; i++)
    {
      this.QTable[i] = new float[numActions];
      for (int j = 0; j < numActions; j++)
      {
        //QTable[i][j] = Random.Range(-1f, 1f);
        //Valores: 0 = inicial, != 0 = Recompensa de la acción
        this.QTable[i][j] = 0;
        //QTable[i][j] = JointQTable[i][j];
      }
    }
  }

  void Update()
  {
    while (playerCombatController.playerStats.health != 0)
    {
      if (episodeTime == 0f)
      { //1st iteration
        OnEpisodeBegin();
      }
      else
      { //2nd and/or + iterations
        this.state = this.enemyController.getStatus(); //Estado actual
        Training();
      }
    }
  }

  private void OnEpisodeBegin()
  {
    //Reset enemies vales
    enemyController.enemyStats.health = 100;
    enemyController.healthBar.value = 100;
    enemyController.enemyStats.accuracy = 0;
    enemyController.isAgent = true;
    enemyController.player = player;

    // RESET PLAYER ATTRS
    playerCombatController.playerStats.health = 100;
    playerCombatController.healthBar.value = 100;
    playerCombatController.isTraining = true;

    //Reset training values
    episodeTime = Time.deltaTime;
    rewardPerEpisode = 0f;

    //Log - Debug
    Debug.Log($"AgentId {enemyController.enemyStats.id} rewardPerEpisode -> {rewardPerEpisode}");
  }

  private void Training()
  {
    episodeTime += Time.deltaTime; //Incrementar tiempo del episodio

    //Update/Check Player Metrics
    this.player_health = playerCombatController.healthBar.value;
    this.player_score = playerCombatController.playerStats.points;
    this.enemy_accuracy = enemyController.enemyStats.accuracy;
    this.enemy_health = enemyController.enemyStats.health;

    //Actualizar la QTable considerando los valores de la tabla JointQ Table
    this.QTable[state][action] += (1 - learningRate) * JointQTable.QTable[state][action] + learningRate * (reward + discountFactor * GetMaxQValue(state) - QTable[state][action]);
    JointQTable.FusionQTables(QTable); //Fusion Agent QTable with the Global/Joint QTable

    reward += GetMaxQValue(state); //Update reward
    rewardPerEpisode += reward; //Increase reward per episode
  }




  private float GetMaxQValue(int state)
  {
    // Obtener el valor máximo de la QTable para el estado dado
    float maxQValue = Mathf.Max(QTable[state]);
    return maxQValue;
  }

}
