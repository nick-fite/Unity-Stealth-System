using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController controller;
        if (other.tag == "Player" && other.gameObject.TryGetComponent(out controller))
        {
            if (controller.GetHasIntel())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
        }
    }
}
