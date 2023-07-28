using System;
public class JointQTable
{
  public float[][] QTable; //Common knowledge table for all agents

  //Function to load the data
  /*   public void LoadData()
    {
      JointQData data = SaveSystem.LoadQTable(); //Load the data
      this.QTable = data.QTable; //Use the loaded data
    }

    //Function to save the data
    public void SaveData()
    {
      SaveSystem.SaveQTable(this.QTable); //Save the current data
    }

    //Function for the agents to access to the common knowledge table
    public float GetQTable()
    {
      return this.QTable;
    } */

  float Median(float a, float b)
  {
    float median = 0;

    float[] values = new float[] { a, b };
    Array.Sort(values);

    median = (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2f;

    return median;
  }

  //Function to fusion the QTables from the agents to the QTable 
  public void FusionQTables(float[][] table)
  {
    //Get the dimnsion of both matrices
    int rowsTable = table.Length;
    int rowsQTable = this.QTable.Length;
    int columnsTable = table[0].Length;
    int columnsQTable = this.QTable[0].Length;

    //Create temporary new matrix/Table
    float[][] updated_QTable = new float[rowsTable + rowsQTable][];

    //Add the data of table A in the temporary table
    for (int i = 0; i < rowsTable; i++)
    {
      updated_QTable[i] = new float[columnsTable];
      Array.Copy(table[i], updated_QTable[i], columnsTable);
    }

    //Add the data of QTable to the temporary table
    for (int i = 0; i < columnsQTable; i++)
    {
      updated_QTable[columnsTable + i] = new float[columnsQTable];
      Array.Copy(QTable[i], updated_QTable[columnsTable + i], columnsQTable);
    }

    this.QTable = updated_QTable;
  }
}