using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{



    //This was the first way I thought to stick a player to a wall, it kinda worked but I think I can do better.
    /*
    ThirdPersonController _input;

    private void Start()
    {
        StartCoroutine(FindInputs());
    }

    private void Update()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 0.5f);
        bool wallFound = false;
        
        foreach (Collider collider in nearbyColliders)
        {
            switch (collider.tag) 
            {
                case "StickableWallX":
                    _input.SetStuckToWallX(true);
                    wallFound = true;
                    break;
                case "StickableWallY":
                    _input.SetStuckToWallY(true);
                    wallFound = true;
                    break;
                case "StickableWallZ":
                    _input.SetStuckToWallZ(true);
                    wallFound = true;
                    break;
            }
            break;
        }

        if (!wallFound) 
        {
            _input.SetStuckToWallX(false);
            _input.SetStuckToWallY(false);
            _input.SetStuckToWallZ(false);
        }
    }

    private void RaycastToWall() 
    {
        
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
    }*/
}
