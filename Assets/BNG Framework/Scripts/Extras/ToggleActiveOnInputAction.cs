using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNG
{

    /// <summary>
    /// This script will toggle a GameObject whenever the provided InputAction is executed
    /// </summary>
    public class ToggleActiveOnInputAction : MonoBehaviour
    {

        public InputActionReference InputAction = default;
        public GameObject Canvas;
        public GameObject ToggleObject = default;
        public bool Paused = false;

        private void OnEnable()
        {
            InputAction.action.performed += ToggleActive;
        }

        private void OnDisable()
        {
            InputAction.action.performed -= ToggleActive;
        }

        public void ToggleActive(InputAction.CallbackContext context)
        {
            if (ToggleObject)
            {
                //ToggleObject.SetActive(!ToggleObject.activeSelf);
                if (Paused == true)
                {
                    Time.timeScale = 1.0f;
                    Paused = false;
                }
                else
                {
                    Time.timeScale = 0.0f;
                    Paused = true;
                }
            }
        }
    }
}

