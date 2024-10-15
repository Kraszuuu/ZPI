using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    public float sprintSpeed = 9f;
    public float defaultSpeed = 5f;
    public float crouchSpeed = 3f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    private float speed = 5f;
    private float prevSpeed = 5f;
    private bool isGrounded;
    private bool crouching;
    private bool sprinting;
    private bool lerpCrouch;
    private float crouchTimer;

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
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
<<<<<<< Updated upstream
            {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }
=======
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2, p);
>>>>>>> Stashed changes

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        if (isGrounded)
        {
            controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
            prevSpeed = speed;
        }
        else
        {
            if (moveDirection.x < 0)
            {
                prevSpeed = crouchSpeed;
            }
            controller.Move(transform.TransformDirection(moveDirection) * prevSpeed * Time.deltaTime);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        controller.Move(playerVelocity * Time.deltaTime);
        //Debug.Log(playerVelocity.y);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
<<<<<<< Updated upstream
    }

    public void Spring()
=======
        if (crouching)
            speed = crouchSpeed;
        else speed = defaultSpeed;
    }

    public void Sprint()
>>>>>>> Stashed changes
    {
        sprinting = !sprinting;
        if (sprinting)
            speed = sprintSpeed;
        else speed = defaultSpeed;
    }
}
