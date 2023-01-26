using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerCamera))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerActions();
    public static event PlayerActions OnInventoryToggle, OnInteraction, OnUIRightClick;

    //References
    PlayerInputs playerInputs;
    PlayerMotor playerMotor;
    PlayerCamera playerCamera;
    
    //Singleton Reference
    public static PlayerController Instance;

    //Variables
    bool fireKeyHeld = false, sprintKeyHeld = false;

    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }

        //Reference init
        playerInputs = new PlayerInputs();
    }

    void Start() { 
        //Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Reference init
        playerMotor = GetComponent<PlayerMotor>();
        playerCamera = GetComponent<PlayerCamera>();
    }

    void OnEnable()
    {
        playerInputs.Player.Enable();
        playerInputs.Player.SpeedUp.started += SpeedUp;
        playerInputs.Player.SpeedUp.canceled += SpeedUp;
        playerInputs.Player.ToggleInventory.performed += ToggleInventory;
        playerInputs.Player.Interaction.performed += Interact;
        //playerInputs.Player.Fire.started += UseItem;
        //playerInputs.Player.Fire.canceled += UseItem;

        playerInputs.UI.Enable();
        playerInputs.UI.RightClick.performed += RightClickUI;
    }
    void OnDisable()
    {
        //Unsubscribe methods to inputs
        playerInputs.Player.Disable();
        playerInputs.Player.SpeedUp.started -= SpeedUp;
        playerInputs.Player.SpeedUp.canceled -= SpeedUp;
        playerInputs.Player.ToggleInventory.performed -= ToggleInventory;
        playerInputs.Player.Interaction.performed -= Interact;
        //playerInputs.Player.Fire.started -= UseItem;
        //playerInputs.Player.Fire.canceled -= UseItem;

        playerInputs.UI.Enable();
        playerInputs.UI.RightClick.performed -= RightClickUI;
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Transform GetOrientation()
    {
        return playerMotor.GetOrientation();
    }

    void Update()
    {
        Movement();
        VerticalMovement();
        Rotation();

        //if (fireKeyHeld) Use();
    }

    /// <summary>
    /// Calculate movement inputs
    /// </summary>
    void Movement()
    {
        Vector2 _inputVector = playerInputs.Player.Move.ReadValue<Vector2>();
        //Calculate direction based of current orientation
        Vector3 _moveDirection = (playerMotor.GetOrientation().forward * _inputVector.y) + (playerMotor.GetOrientation().right * _inputVector.x);
        playerMotor.SetDirection(_moveDirection);
    }

    /// <summary>
    /// Calculate vertical movement inputs
    /// </summary>
    void VerticalMovement()
    {
        float _inputVector = playerInputs.Player.VerticalMove.ReadValue<float>();
        playerMotor.SetVerticalDirection(_inputVector);
    }

    /// <summary>
    /// Calculate player rotations with mouse inputs
    /// </summary>
    void Rotation()
    {
        Vector2 _inputVector = playerInputs.Player.Look.ReadValue<Vector2>();
        playerCamera.SetRotation(_inputVector);
    }

    void SpeedUp(InputAction.CallbackContext context)
    {
        if (context.started)
            playerMotor.SpeedUp(true);
        if (context.canceled)
            playerMotor.SpeedUp(false);
    }

    void ToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed) {
            OnInventoryToggle?.Invoke();
        }
    }
    
    void Interact(InputAction.CallbackContext context)
    {
        if (context.performed) {
            OnInteraction?.Invoke();
        }
    }

    void RightClickUI(InputAction.CallbackContext context)
    { 
        if (context.performed) {
            OnUIRightClick?.Invoke();
        }
    }

    /*
    void UseItem(InputAction.CallbackContext context)
    {
        if (context.started)
            fireKeyHeld = true;
        if (context.canceled)
            fireKeyHeld = false;
    }
    */
}
