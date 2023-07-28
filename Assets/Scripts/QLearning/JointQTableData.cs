using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class JointQData
{
  public float[][] QTable; //Variable QTable
  float n; //filas
  float m; //columnas
  public int score; //Variable prueba

  public JointQData(int score)
  {
    this.score = score;
  }

  //Constructor para llenar los datos de la JointQTable
  /*
  public JointQData (Joint_QTable joint_QTable) {
      this->n = joint_QTable.Length; //Tamaño filas
      this->m = joint_QTable[0].Length; //Tamaño columnas
      //this->QTable = new float[n][m]; //Crear la matriz con las dimensiones
      this->QTable = joint_QTable; //copiar los datos de la jointQTable obtenida por parámetros
  }
  */
}