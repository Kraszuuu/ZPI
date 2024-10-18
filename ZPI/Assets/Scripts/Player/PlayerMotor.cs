using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool crouching;
    private bool lerpCrouch;
    private bool sprinting;
    public float crouchTimer = 1f;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        if (lerpCrouch)
        {
            LerpCrouch();
        }
    }

    //receive the inputs from out InputManager.cs and apply them to our character controller.
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(speed * Time.deltaTime * transform.TransformDirection(moveDirection));
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    // slowly change camera height while in process of crouching/standing up
    private void LerpCrouch()
    {
        crouchTimer += Time.deltaTime;
        float p = crouchTimer / 1;
        p *= p;
        if (crouching)
        {
            controller.height = Mathf.Lerp(controller.height, 1, p);
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, 2, p);
        }

        if (p > 1)
        {
            lerpCrouch = false;
            crouchTimer = 0f;
        }
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
        {
            speed = 8f;
        }
        else
        {
            speed = 5f;
        }
    }
}
