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
  private Mutex fileMutex;

  public enum ActionType { ATTACK, BLOCK, MOVE, DODGE, DRINK_POTION, HURT };
  public enum ActionResult { SUCCESS, FAIL, DEAD }
  public enum CharacterType { ENEMY, PLAYER };

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
    FileName = string.Format("LOG_{0:D2}_{1:D2}_{2:D2}{3:D2}.txt", now.Day, now.Month, now.Hour, now.Minute);
    FilePath = Path.Combine(FilePathIncomplete, FileName);
    Debug.Log("LOG at " + FilePath);
  }


  public void WriteFile(string content)
  {
    fileMutex.WaitOne();
    try
    {
      using (StreamWriter writer = new StreamWriter(FilePath, true))
      {
        writer.WriteLine(content);
      }
    }
    finally
    {
      fileMutex.ReleaseMutex();
    }
  }

  public void WriteAction(ActionType _action, ActionResult _outcome, CharacterType _type, IStatsDataProvider character, IStatsDataProvider opponent)
  {
    string content;
    StatsData characterData = character.GetStatsData();
    StatsData opponentData = opponent.GetStatsData();
    DateTime now = DateTime.Now;
    int _health_diff = characterData.health - opponentData.health;
    int _hordeNumber = characterData.hordeNumber;

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

}