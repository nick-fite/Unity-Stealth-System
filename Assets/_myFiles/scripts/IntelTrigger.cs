using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController controller;
        if (other.tag == "Player" && other.gameObject.TryGetComponent(out controller))
        {
            controller.SetHasIntel(true);
            Destroy(gameObject);
        }
    }
}
