using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Android.LowLevel;
using UnityEngine.InputSystem.XR;

public class WallDetection : MonoBehaviour
{
    private LayerMask whatIsWall;
    private StarterAssetsInputs input;
    [Range(0.0f, 1.0f)] [SerializeField] private float wallCheckDistance;
    private RaycastHit leftWall;
    private RaycastHit rightWall;
    private bool leftWallHit;
    private bool rightWallHit;
    private GameObject objectHit;
    private CharacterController controller;
    public Transform objectRotation;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        whatIsWall = LayerMask.NameToLayer("Wall");
    }

    private void FixedUpdate()
    {
        if (input.crouch)
        {
            CheckForOpenSpace();
            CheckForWall();

            if (leftWallHit)
            {
                objectHit = leftWall.collider.gameObject;
                objectRotation = leftWall.collider.gameObject.transform;
                //PushPlayerTowardsWall();
            }
            if (rightWallHit)
            {
                objectHit = rightWall.collider.gameObject;
                objectRotation = rightWall.collider.gameObject.transform;
                //PushPlayerTowardsWall();
            }
        }
        else
        {
            objectHit = null;
            leftWallHit = false;
            rightWallHit = false;
        }
    }

    private void CheckForWall() 
    {

        Ray rayLeft = new Ray(new Vector3(transform.position.x, transform.position.y + (controller.height/2), transform.position.z), -transform.right);
        Ray rayRight = new Ray(new Vector3(transform.position.x, transform.position.y + (controller.height/2), transform.position.z), transform.right);
        Debug.DrawRay(rayLeft.origin, rayLeft.direction * wallCheckDistance);
        Debug.DrawRay(rayRight.origin, rayRight.direction * wallCheckDistance);

        leftWallHit = Physics.Raycast(rayLeft, out leftWall, whatIsWall);
        rightWallHit = Physics.Raycast(rayRight, out rightWall, whatIsWall);
    }

    private void CheckForOpenSpace() 
    {
        Ray upLeft = new Ray(new Vector3(transform.position.x, transform.position.y + (controller.height + 2.3f), transform.position.z), -transform.right);
        Ray upRight = new Ray(new Vector3(transform.position.x, transform.position.y + (controller.height + 2.3f), transform.position.z), transform.right);
        Debug.DrawRay(upLeft.origin, upLeft.direction * wallCheckDistance);
        Debug.DrawRay(upRight.origin, upRight.direction * wallCheckDistance);

        Ray side1Left = new Ray(new Vector3(transform.position.x + 1, transform.position.y + (controller.height / 2), transform.position.z), -transform.right);
        Ray side1Right = new Ray(new Vector3(transform.position.x + 1, transform.position.y + (controller.height / 2), transform.position.z), transform.right);
        Debug.DrawRay(side1Left.origin, side1Left.direction * wallCheckDistance);
        Debug.DrawRay(side1Right.origin, side1Right.direction * wallCheckDistance);

        Ray side2Left = new Ray(new Vector3(transform.position.x - 1, transform.position.y + (controller.height / 2), transform.position.z), -transform.right);
        Ray side2Right = new Ray(new Vector3(transform.position.x - 1, transform.position.y + (controller.height / 2), transform.position.z), transform.right);
        Debug.DrawRay(side2Left.origin, side2Left.direction * wallCheckDistance);
        Debug.DrawRay(side2Right.origin, side2Right.direction * wallCheckDistance);
    }

    public bool GetLeftHit() { return leftWallHit; }
    public bool GetRightHit() { return rightWallHit; }
    //public bool GetStuckToWall() { return stuckToWall; }
    //public void SetStuckToWall(bool newState) { stuckToWall = newState; }

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
