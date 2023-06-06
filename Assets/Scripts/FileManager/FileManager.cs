using System;
using System.IO;
using UnityEngine;
using System.Threading;

public class FileManager : MonoBehaviour
{
  private static FileManager instance;
  public static FileManager Instance => instance;

  public string FileName;
  public string FilePath;
  private string FilePathIncomplete = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MAS_LOGS");

  //Actions
  public enum Actions { ATTACK, BLOCK, MOVE, DODGE, DRINK_POTION, HURT}; // Actions for the file content
  public enum Action_Result {SUCCESS, FAIL, DEAD} //Action outcome/result
  public enum Character { ENEMY, PLAYER }; //Who made the action

  private Mutex fileMutex;

  private void Awake()
  {
    if (instance)
    {
      Destroy(gameObject);
    }
    else
    {
      instance = this;
      instance.CreateFile();
      DontDestroyOnLoad(gameObject);
    }

    fileMutex = new Mutex();
  }


  public static void Initialize()
  {
    if (instance == null)
    {
      GameObject fileManagerObject = new GameObject("FileManager");
      instance = fileManagerObject.AddComponent<FileManager>();
    }
  }


  public void CreateFile()
  {
    DateTime now = DateTime.Now;
    string fileName = string.Format("replay_{0:D2}_{1:D2}_{2:D2}{3:D2}.json", now.Day, now.Month, now.Hour, now.Minute);
    FileName = fileName;
    string logMessage = "Archivo se creó con el nombre: " + FileName;
    Debug.Log(logMessage);

    FilePath = Path.Combine(FilePathIncomplete, FileName);
    logMessage = "Filepath: " + FilePath;
    Debug.Log(logMessage);
  }


  public void WriteFile(string content)
  {
    fileMutex.WaitOne();
    try
    {
      using (StreamWriter writer = new StreamWriter(FilePath, true)) // Utiliza el parámetro 'append' en 'true' para agregar al archivo existente
      {
        writer.WriteLine(content); //Add a new line to the file
      }
    }
    finally
    {
      fileMutex.ReleaseMutex();
    }
  }
}
