using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
  public static CanvasManager instance;

  [SerializeField]
  private GameObject image;

  void Awake()
  {
    if (instance)
    {
      Destroy(instance);
    }
    else
    {
      instance = this;
    }
  }

  public void Wasted()
  {
    image.SetActive(true);

    Time.timeScale = 0.2f;

    StartCoroutine(Utility.TimedEvent(() =>
    {
      Time.timeScale = 1f;
      image.SetActive(false);
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }, 0.8f));
  }

  public void SceneLoading(int index)
  {
    SceneManager.LoadScene(index, LoadSceneMode.Single);
  }
}
