using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using MyEnums;
public class QEnemyAgent : Agent
{
  public enum ActionType { ATTACK, BLOCK, MOVE, DODGE, DRINK_POTION, HURT }; // Actions for the file content
  public GameObject player;
  private EnemyController enemyController;
  private CombatController playerCombatController;

  private float[][] QTable; //QTable from the Agent
                            //QTable: filas (i) = estado, columnas (j) = acción
  private float learningRate = 0.1f; //Learning Rate
  private float discountFactor = 0.99f; //Discount Factor
  private int state;
  private int action;

  public override void Initialize()
  {
    enemyController = GetComponent<EnemyController>();
    playerCombatController = enemyController.player.GetComponent<CombatController>();

    // Inicializar la QTable con valores aleatorios
    int numStates = 5; // Número de estados
    int numActions = 2; // Número de acciones (moverse, atacar)
    QTable = new float[numStates][];
    for (int i = 0; i < numStates; i++)
    {
      QTable[i] = new float[numActions];
      for (int j = 0; j < numActions; j++)
      {
        QTable[i][j] = Random.Range(-1f, 1f);
      }
    }
  }

  public override void OnEpisodeBegin()
  {
    // Restablecer la salud del enemigo a 100
    enemyController.transform.position = new Vector3(0f, 0f, 10f);
    enemyController.enemyStats.health = 100;
    enemyController.enemyStats.accuracy = 0;
    enemyController.isAgent = true;
    enemyController.player = player;
    playerCombatController.playerStats.health = 100;
    playerCombatController.healthBar.value = 100;
    playerCombatController.isTraining = true;
  }

  public override void CollectObservations(VectorSensor sensor)
  {
    // Observaciones del entorno y del agente
    Vector3 playerPosition = playerCombatController.GetPosition();
    Vector3 enemyPosition = transform.position;

    // 1. Distancia entre el jugador y el enemigo
    float distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
    sensor.AddObservation(distanceToPlayer);

    // 2. Diferencia entre la vida del jugador y el enemigo
    sensor.AddObservation(playerCombatController.playerStats.health);
    sensor.AddObservation(enemyController.enemyStats.health);

    // 3. Puntaje del jugador
    float playerScore = playerCombatController.GetMainMetric();
    sensor.AddObservation(playerScore);

    // 4. Precisión del enemigo
    float enemyAccuracy = enemyController.GetMainMetric();
    sensor.AddObservation(enemyAccuracy);

    // 5. Posición del jugador
    sensor.AddObservation(playerPosition);
  }
  public override void OnActionReceived(ActionBuffers actions)
  {
    // Acciones discretas del agente
    int state = actions.DiscreteActions[0]; // State: isMoving, isAttacking, isIddle
    int attackRange = actions.DiscreteActions[1]; //5
    int chasingRange = actions.DiscreteActions[2]; //5
    int speedRange = actions.DiscreteActions[3]; //5
    int movementAction = actions.DiscreteActions[4]; // UP DOWN LEFT RIGHT

    Vector3 moveDirection = Vector3.zero;
    switch (movementAction)
    {
      case 0: // Mover hacia adelante
        moveDirection = transform.forward;
        break;
      case 1: // Mover hacia atrás
        moveDirection = -transform.forward;
        break;
      case 2: // Mover hacia la izquierda
        moveDirection = -transform.right;
        break;
      case 3: // Mover hacia la derecha
        moveDirection = transform.right;
        break;
    }

    switch (attackRange)
    {
      case 0:
        enemyController.attackRange = 1f;
        break;
      case 1:
        enemyController.attackRange = 1.5f;
        break;
      case 2:
        enemyController.attackRange = 2f;
        break;
      case 3:
        enemyController.attackRange = 2.5f;
        break;
      case 4:
        enemyController.attackRange = 3f;
        break;
    }

    switch (chasingRange)
    {
      case 0:
        enemyController.chasingRange = 8f;
        break;
      case 1:
        enemyController.chasingRange = 9f;
        break;
      case 2:
        enemyController.chasingRange = 10f;
        break;
      case 3:
        enemyController.chasingRange = 15f;
        break;
      case 4:
        enemyController.chasingRange = 20f;
        break;
    }

    switch (speedRange)
    {
      case 0:
        enemyController.speedRange = 0.006f;
        break;
      case 1:
        enemyController.speedRange = 0.008f;
        break;
      case 2:
        enemyController.speedRange = 0.009f;
        break;
      case 3:
        enemyController.speedRange = 0.011f;
        break;
      case 4:
        enemyController.speedRange = 0.013f;
        break;
    }

    if (state == 0)
    {
      enemyController.enemyAnimator.SetBool("isMoving", true);
      if (Vector3.Distance(transform.position, player.transform.position) >= chasingRange)
      {
        Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        transform.position += moveDirection * Time.deltaTime * 1f;
      }
    }
    else if (state == 1)
    {
      enemyController.enemyAnimator.SetBool("isMoving", false);
      enemyController.Attack();
    }

    // Actualizar el estado y la acción previa
    int previousState = state;
    int previousAction = action;
    state = CalculateState();

    // Calcular la recompensa en base al cambio de estado
    float reward = CalculateReward();
    SetReward(reward);
    // Actualizar la QTable
    QTable[previousState][previousAction] += learningRate * (reward + discountFactor * GetMaxQValue(state) - QTable[previousState][previousAction]);
  }

  private bool CheckMountainCollision()
  {
    Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
    foreach (Collider collider in colliders)
    {
      if (collider.CompareTag("Mountain"))
      {
        return true;
      }
    }
    return false;
  }

  private bool CheckPlayerCollision()
  {
    Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
    foreach (Collider collider in colliders)
    {
      if (collider.CompareTag("Player"))
      {
        return true;
      }
    }
    return false;
  }

  private int CalculateState()
  {
    if (enemyController.isAttacking) return 1;
    else return 0;
  }

  private float CalculateReward()
  {
    float reward = 0f;
    // Obtener la distancia entre el enemigo y el jugador
    Vector3 playerPosition = playerCombatController.GetPosition();
    Vector3 enemyPosition = transform.position;
    float distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);

    // Calcular la recompensa basada en la diferencia entre la distancia actual y la distancia objetivo
    if (distanceToPlayer < 15f)
    {
      reward += 10f;
    }
    else
    {
      // El enemigo está lejos del jugador (comportamiento no deseado)
      reward -= -1f;
    }

    // Verificar colisiones con montañas
    if (CheckMountainCollision())
    {
      // Aplicar recompensa negativa y finalizar el episodio
      SetReward(-10f);
      EndEpisode();
    }

    if (playerCombatController.playerStats.health <= 0)
    {
      SetReward(10f);
      EndEpisode();
    }

    if (enemyController.enemyStats.health <= 0)
    {
      SetReward(-10f);
      EndEpisode();
    }

    //Attack rewards
    switch (enemyController._attackState)
    {
      case AttackStates.SUCCESS:
        reward += 5f;
        break;
      case AttackStates.FAIL:
        reward -= 10f;
        break;
    }
    //Reset Triggers
    enemyController._attackState = AttackStates.NO_ATTACK;
    return reward;
  }

  private float GetMaxQValue(int state)
  {
    // Obtener el valor máximo de la QTable para el estado dado
    float maxQValue = Mathf.Max(QTable[state]);

    return maxQValue;
  }

  public override void Heuristic(in ActionBuffers actionsOut)
  {
    // No hacer nada
  }
}
