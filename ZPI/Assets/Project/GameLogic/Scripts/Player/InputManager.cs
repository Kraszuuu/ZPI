using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    private PauseManager pauseManager;
    private PrimaryAttack primaryAttack;
    private SmoothFollowPoint smoothFollowPoint;
    private Vector2 currentMoveInput;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        pauseManager = FindObjectOfType<PauseManager>();
        primaryAttack = GetComponent<PrimaryAttack>();
        smoothFollowPoint = GetComponentInChildren<SmoothFollowPoint>();

        onFoot.Jump.performed += ctx => motor.Jump();

        onFoot.Crouch.performed += ctx => motor.Crouch();
        // onFoot.Crouch.canceled += ctx => motor.Crouch();

        onFoot.Sprint.performed += ctx => motor.StartSprint();
        onFoot.Sprint.canceled += ctx => motor.StopSprint();

        onFoot.Dash.performed += OnDashPerformed;

        onFoot.Pause.performed += ctx => TogglePause();

        onFoot.BasicAttack.performed += ctx => primaryAttack.ShootProjectile();

        onFoot.HideWand.performed += ctx => smoothFollowPoint.ToggleWand();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tell the playermotor to move using the value from our movement action
        currentMoveInput = onFoot.Movement.ReadValue<Vector2>();
        motor.ProcessMove(currentMoveInput);
    }

    private void LateUpdate()
    {
        if (!GameState.Instance.IsSpellCasting)
        {
            look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
        }
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        Vector3 dashDirection = new Vector3(currentMoveInput.x, 0, currentMoveInput.y).normalized;

        // Jeśli nie ma kierunku, nie dashuj
        if (dashDirection != Vector3.zero)
        {
            motor.StartDash(dashDirection);
        }
    }
    private void TogglePause()
    {
        if (pauseManager != null && !GameState.Instance.IsGamePaused)
        {
            pauseManager.PauseGame();
        }
    }
}
