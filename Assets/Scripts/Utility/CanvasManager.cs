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

  private void Update()
  {
    // Asegurarse de que el objeto del HUD esté siguiendo la cámara
    if (Camera.main != null)
    {
      // Obtener la posición y rotación de la cámara principal
      Vector3 cameraPosition = Camera.main.transform.position;
      Quaternion cameraRotation = Camera.main.transform.rotation;

      Vector3 offsetVector = cameraRotation * new Vector3(0f, 1.3f, 3f);
      Vector3 hudPosition = cameraPosition + offsetVector;

      // Asignar la posición y rotación de la cámara al objeto del HUD
      transform.position = Vector3.Lerp(transform.position, hudPosition, Time.deltaTime * 5f);
      transform.rotation = cameraRotation;
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
