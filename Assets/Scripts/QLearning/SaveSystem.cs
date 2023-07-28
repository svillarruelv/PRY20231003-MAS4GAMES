using UnityEngine;
using System.IO; //Librería para los archivos
using System.Runtime.Serialization.Formatters.Binary; //Librería para convertir los archivos en binario

public class SaveSystem
{

  //Función para guardar la QTable
  /*
  public static void SaveQTable (Joint_QTable joint_QTable) {
      BinaryFormatter formatter = new BinaryFormatter(); //Formateador del archivo a binario
      string path = Application.persistentDataPath + "/training.txt"; //Dirección donde se almacenará el archivo
      FileStream stream = new FileStream(path, FileMode.Create); //Crear el stream 

      JointQData data = new JointQData(joint_QTable); //Agregar los datos a la variable Data = datos a guardar

      formatter.Serialize(stream, data); //Escribir y codificar en el archivo los valores del stream y la data
      stream.Close(); //Cerrar el stream una vez que se terminó de escribir en el archivo
  }
  */

  //Función para cargar los datos de la QTable
  public static JointQData LoadQTable()
  {
    string path = Application.persistentDataPath + "/savedata.txt"; //Dirección donde se obtendrá el archivo
    Debug.Log(path);
    if (File.Exists(path)) //Revisar que el archivo existe
    {
      BinaryFormatter formatter = new BinaryFormatter(); //Formateador del archivo a binario
      FileStream stream = new FileStream(path, FileMode.Open); //Crear el stream que abrirá/leerá el archivo

      JointQData data = formatter.Deserialize(stream) as JointQData; //Decodificar y leer la data del archivo
      stream.Close(); //Cerrar el stream

      return data;
    }
    else //Si el archivo no existe
    {
      return null;
    }

  }

  //Función de prueba
  public static void SaveQTable(int score)
  {
    BinaryFormatter formatter = new BinaryFormatter(); //Formateador del archivo a binario
    string path = Application.persistentDataPath + "/savedata.txt"; //Dirección donde se almacenará el archivo
    FileStream stream = new FileStream(path, FileMode.Create); //Crear el stream 

    JointQData data = new JointQData(score); //Agregar los datos a la variable Data = datos a guardar

    formatter.Serialize(stream, data); //Escribir en el archivo los valores del stream y la data
    stream.Close(); //Cerrar el stream una vez que se terminó de escribir en el archivo
  }
}