using System;
using System.IO;
using UnityEngine;
using System.Threading;

public class JointQTable : MonoBehaviour
{
  private static JointQTable instance;
  public static JointQTable Instance => instance;

  private float[,] qTable; // Common knowledge table for all agents
  private bool isInitialized = false;

  private void Awake()
  {
    if (instance != null && instance != this)
    {
      Destroy(this.gameObject);
    }
    else
    {
      instance = this;
      DontDestroyOnLoad(gameObject);

      // Initialize qTable if it's not already initialized
      if (!isInitialized)
      {
        InitializeQTable();
        isInitialized = true;
      }
    }
  }

  public static void Initialize()
  {
    if (instance == null)
    {
      GameObject jointQTableObject = new GameObject("JointQTable");
      instance = jointQTableObject.AddComponent<JointQTable>();
    }
  }

  private void InitializeQTable()
  {
    int numStates = 3;
    int numActions = 9;
    qTable = new float[numStates, numActions];

    for (int i = 0; i < numStates; i++)
    {
      for (int j = 0; j < numActions; j++)
      {
        qTable[i, j] = 0;
      }
    }
  }

  public float GetQValue(int state, int action)
  {
    return qTable[state, action];
  }

  public void FusionQTables(float[,] newTable)
  {
    int rows = newTable.GetLength(0);
    int cols = newTable.GetLength(1);

    float[,] temp = new float[rows, cols];

    for (int i = 0; i < cols; i++)
    {
      for (int j = 0; j < rows; j++)
      {
        float mean = (newTable[j, i] + qTable[j, i]) / 2;
        temp[j, i] = Mathf.Clamp(mean, -1f, 1f);
      }
    }

    qTable = temp;
  }

  public bool IsTableInitialized()
  {
    return isInitialized;
  }
}
