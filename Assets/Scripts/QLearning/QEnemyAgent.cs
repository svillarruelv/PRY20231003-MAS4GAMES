using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class QEnemyAgent : Agent
{
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
  }

  public override void OnEpisodeBegin()
  {
    // Restablecer la salud del enemigo a 100
    enemyController.enemyStats.health = 100;
    enemyController.enemyStats.accuracy = 0;

    // Establecer al jugador como objetivo del enemigo
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    if (players.Length > 0)
    {
      enemyController.player = players[0];
    }
  }

  public override void CollectObservations(VectorSensor sensor)
  {
    // Observaciones del entorno y del agente

    // 1. Distancia entre el jugador y el enemigo
    Vector3 playerPosition = playerCombatController.GetPosition();
    Vector3 enemyPosition = transform.position;
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
    sensor.AddObservation(enemyPosition);
  }
  public override void OnActionReceived(ActionBuffers actions)
  {
    // Acciones recibidas del agente

    // Ejemplo de acciones basadas en los valores de vectorAction
    float moveX = actions.ContinuousActions[0];
    float moveZ = actions.ContinuousActions[1];
    bool attack = actions.ContinuousActions[2] > 0f;
    float movementSpeed = 5f;

    transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * movementSpeed;

    // Realizar acciones en función de los valores recibidos
    // Por ejemplo, moverse en una dirección, atacar al jugador, etc.

    // Actualizar las estadísticas y la recompensa según sea necesario

    // Finalizar el episodio si se cumple alguna condición (por ejemplo, si el enemigo muere)

    // Otorgar una recompensa al agente según su desempeño
    float reward = 0f;
    if (enemyController.enemyStats.health <= 0)
      reward += 100f; // Recompensa positiva si el enemigo muere
    else if (playerCombatController.playerStats.health <= 0)
      reward -= 100f; // Recompensa negativa si el jugador muere
    else
      reward -= 0.01f; // Penalización por cada paso de tiempo sin terminar el episodio

    // Asignar la recompensa al agente
    AddReward(reward);
  }

  public override void Heuristic(in ActionBuffers actionsOut)
  {
    // Acciones para el modo Heurístico (control manual)
    ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    continuousActions[0] = Input.GetAxisRaw("Horizontal");
    continuousActions[1] = Input.GetAxisRaw("Vertical");
    // Por ejemplo, asignar valores directamente a actionsOut para controlar el enemigo manualmente
  }
}
