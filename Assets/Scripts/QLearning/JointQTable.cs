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
    //Get dimensions of the matrix
    int rows = table.Length;
    int cols = table[0].Length;

    float[][] temp = new float[cols][]; //Temporary matrix that will store the modified values
	
    for (int i = 0; i < cols; i++) {
        temp[i] = new float[rows]; //For each column create a row
		
        for (int j = 0; j < rows; j++) {
            float median = Median(table[j][i], QTable[j][i]); //Get the median value of both tables
            temp[i][j] = median; //Update the temporary matrix
        }
    }

    this.QTable = temp; //QTable = Temporary matrix
  }
}