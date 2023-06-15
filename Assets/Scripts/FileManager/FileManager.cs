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
  public enum ActionType { ATTACK, BLOCK, MOVE, DODGE, DRINK_POTION, HURT }; // Actions for the file content
  public enum ActionResult { SUCCESS, FAIL, DEAD } //Action outcome/result
  public enum CharacterType { ENEMY, PLAYER }; //Who made the action

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
#if UNITY_EDITOR
      instance.CreateFile();
#endif
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
    string fileName = string.Format("replay_{0:D2}_{1:D2}_{2:D2}{3:D2}.txt", now.Day, now.Month, now.Hour, now.Minute);
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

  //UTITILY WRITER FUNCTIONS
  public void WriteAction(ActionType _action, ActionResult _outcome, CharacterType _type, IStatsDataProvider character, IStatsDataProvider opponent)
  {
    /*int _opp_health;
    int _opp_accuracy;

    if (_type == CharacterType.PLAYER) {
      _opp_health = avg_health;
      _opp_accuracy = avg_accuracy;
    }*/
    //Variables
    string content;
    StatsData characterData = character.GetStatsData();
    StatsData opponentData = opponent.GetStatsData();
    DateTime now = DateTime.Now;
    int _health_diff = characterData.health - opponentData.health;
    int _hordeNumber = characterData.hordeNumber;

    //String variables
    string action = _action.ToString();
    string outcome = _outcome.ToString();
    string type = _type.ToString();
    string time = now.ToString("HH:mm:ss");
    string health = characterData.health.ToString();
    string id = characterData.id.ToString();
    string opp_id = opponentData.id.ToString();
    string health_diff = _health_diff.ToString();
    string mainMetric = character.GetMainMetric().ToString();
    string opp_mainMetric = opponent.GetMainMetric().ToString();
    string hordeNumber = _hordeNumber.ToString();

    content = $"{time},{id},{hordeNumber},{type},{action},{outcome},{opp_id},{health},{health_diff},{mainMetric},{opp_mainMetric},{character.GetPosition()},{opponent.GetPosition()}";

    WriteFile(content);
  }
  /*
  //Ejemplo para el enemigo:
    string _action = Actions.ATTACK.ToString();
    string _outcome = Action_Result.SUCCESS.ToString();
    string _character = Character.ENEMY.ToString();
    string _id = "1";
    int h = 10; string health = h.ToString();
    int accuracy = 50; string a = accuracy.ToString();
    int op_accuracy = 60; string op_a = op_accuracy.ToString();
    int health_diff = h - 8; string h_diff = health_diff.ToString();
    DateTime now = DateTime.Now; string time = now.ToString("HH:mm:ss");
    
    content = _id + ',' + _character + ',' + _action + ',' + _outcome + ',' + health + ',' + a + ',' + op_a + ',' + h_diff + ',' + time;
  */

}
