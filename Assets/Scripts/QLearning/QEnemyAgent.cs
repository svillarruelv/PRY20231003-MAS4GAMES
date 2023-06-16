using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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
    float healthDifference = playerCombatController.playerStats.health - enemyController.enemyStats.health;
    sensor.AddObservation(healthDifference);

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
    // Acciones recibidas del agente

    // Ejemplo de acciones basadas en los valores de vectorAction
    float moveX = actions.ContinuousActions[0];
    float moveZ = actions.ContinuousActions[1];
    bool attack = actions.DiscreteActions[0] == 1;
    int prevPlayerHealth = player.GetComponent<CombatController>().playerStats.health;
    float movementSpeed = 5f;


    if (attack)
    {
      enemyController.enemyAnimator.SetBool("isMoving", false);
      enemyController.Attack();
    }
    else
    {
      enemyController.enemyAnimator.SetBool("isMoving", true);
      transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * movementSpeed;
    }

    // Actualizar el estado y la acción previa
    int previousState = state;
    int previousAction = action;
    state = CalculateState();
    action = attack ? 1 : 0;

    // Calcular la recompensa en base al cambio de estado
    float reward = CalculateReward();

    // Actualizar la QTable
    QTable[previousState][previousAction] += learningRate * (reward + discountFactor * GetMaxQValue(state) - QTable[previousState][previousAction]);

    // Obtener la distancia entre el enemigo y el jugador
    Vector3 playerPosition = playerCombatController.GetPosition();
    Vector3 enemyPosition = transform.position;
    float distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
    // Calcular la recompensa basada en la diferencia entre la distancia actual y la distancia objetivo
    if (distanceToPlayer < 15f)
    {
      // El enemigo está cerca del jugador (comportamiento deseado)
      AddReward(10f);
    }
    else
    {
      // El enemigo está lejos del jugador (comportamiento no deseado)
      AddReward(-1f);
    }

    // Verificar colisiones con montañas
    if (CheckMountainCollision())
    {
      // Aplicar recompensa negativa y finalizar el episodio
      SetReward(-10f);
      EndEpisode();
      return;
    }

    if (playerCombatController.playerStats.health <= 0)
    {
      SetReward(10f);
      EndEpisode();
      return;
    }

    if (enemyController.enemyStats.health <= 0)
    {
      SetReward(-10f);
      EndEpisode();
      return;
    }
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
    // Lógica para calcular la recompensa
    // ...

    return 1;
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
