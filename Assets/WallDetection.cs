using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{
    ThirdPersonController _input;

    private void Start()
    {
        StartCoroutine(FindInputs());
    }

    private void Update()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider collider in nearbyColliders)
        {
            switch (collider.tag) 
            {
                case "StickableWallX":
                    _input.SetStuckToWallX(true);
                    break;
                case "StickableWallY":
                    _input.SetStuckToWallY(true);
                    break;
                case "StickableWallZ":
                    _input.SetStuckToWallZ(true);
                    break;
            }
            break;
        }
    }

    IEnumerator FindInputs() 
    {
        bool hasInput = false;
        while (!hasInput) 
        {
            if (TryGetComponent(out _input))
            {
                _input.GetComponent<ThirdPersonController>();
                hasInput = true;
            }
            yield return null;
        }
    }
}
