using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

  public static JointQData LoadQTable()
  {
    string path = Application.persistentDataPath + "/savedata.txt";
    if (File.Exists(path))
    {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream stream = new FileStream(path, FileMode.Open);

      JointQData data = formatter.Deserialize(stream) as JointQData;
      stream.Close();

      return data;
    }
    else
    {
      return null;
    }

  }

  public static void SaveQTable(int score)
  {
    BinaryFormatter formatter = new BinaryFormatter();
    string path = Application.persistentDataPath + "/savedata.txt";
    FileStream stream = new FileStream(path, FileMode.Create);

    JointQData data = new JointQData(score, path);

    formatter.Serialize(stream, data);
    stream.Close();
  }
}