using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public WeaponData weaponData;

    [SerializeField]
    private Image press;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateUI();
        }
    }

    public void ActivateUI()
    {
        press.gameObject.SetActive(!press.gameObject.activeSelf);
    }
}
