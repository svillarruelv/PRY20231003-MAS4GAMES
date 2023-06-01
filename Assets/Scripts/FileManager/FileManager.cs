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
    private string FilePathIncomplete = @"\PRY20231003-MAS4GAMES\Assets\MAS4GAMES\Logs";

    private Mutex fileMutex;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        fileMutex = new Mutex();
    }

    public void CreateFile()
    {
        DateTime datetime = DateTime.Now;
        string name = "Partida " + datetime.ToString() + ".txt";
        FileName = name;
        string a = "Archivo se creó con el nombre: " + FileName;
        Debug.Log(a);

        FilePath = Path.Combine(FilePathIncomplete, FileName);
        a = "Filepath: " + FilePath;
        Debug.Log(a);
    }

    public void WriteFile(string content)
    {
        fileMutex.WaitOne(); // Espera hasta que se obtenga el bloqueo
        try
        {
            Directory.CreateDirectory(FilePath);
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                writer.Write(content);
            }
        }
        finally
        {
            fileMutex.ReleaseMutex(); // Libera el bloqueo
        }
    }
}
