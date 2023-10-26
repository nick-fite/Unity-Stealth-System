using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerControl : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    [SerializeField] CharacterController playerController;
    Vector3 playerVelocity;
    bool isGrounded;

    [SerializeField]private bool isMoving;
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = playerController.isGrounded;
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    public void ProcessMovement() 
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 MovementDirection = new Vector3(inputVector.x, 0, inputVector.y);
        playerController.Move(Time.deltaTime * transform.TransformDirection(MovementDirection) * speed);

        playerVelocity.y += Time.deltaTime * gravity;
        if (isGrounded && playerVelocity.y < 0) { playerVelocity.y = -2f; }

        playerController.Move(playerVelocity * Time.deltaTime);
    }

    public void checkMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMoving = true;
        }
        if (context.canceled)
        {
            isMoving = false;
        }

        Debug.Log(isMoving);
    }
}
