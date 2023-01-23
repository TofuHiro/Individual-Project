using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Tooltip("Transform storing the orientation of the player")]
    [SerializeField] Transform orientation;

    [Header("Walking Movement")]
    [Tooltip("The base walking acceleration of the player")]
    [SerializeField] float walkingSpeed = 40f;
    [Tooltip("The multiplier applied to the player's speed when jumping while not in floating mode")]
    [SerializeField] float walkingAirSpeedMult = .1f;
    [Tooltip("The maximum speed the player can reach while walking")]
    [SerializeField] float maxWalkingSpeed = 6f;
    [Tooltip("The multiplier applied to the player's speed when sprinting")]
    [SerializeField] float walkingSpeedUpMult = 4f;
    [Tooltip("The physical drag applied while the player is walking")]
    [SerializeField] float groundDrag = 7f;
    [Tooltip("The upwards force applied to the player to jump")]
    [SerializeField] float jumpForce = 5f;
    [Tooltip("The time delay between jumps")]
    [SerializeField] float jumpRate = .1f;
    [Tooltip("The radius of the sphere that is cast to check for ground objects")]
    [SerializeField] float groundCheckRadius = .5f;
    [Tooltip("The layer to check if objects are considered the ground")]
    [SerializeField] LayerMask groundLayer;
    [Tooltip("The position to cast the sphere to chek for ground")]
    [SerializeField] Transform groundCheckTransform;

    [Header("Floating Movement")]
    [Tooltip("The base floating acceleration of the player")]
    [SerializeField] float floatingSpeed = 50f;
    [Tooltip("The multiplier applied to the player's speed when floating/boosting fast")]
    [SerializeField] float floatingSpeedUpMult = 4f;
    [Tooltip("The maximum speed the player can reach while floating")]
    [SerializeField] float maxFloatingSpeed = 6f;
    [Tooltip("The physical drag applied while the player is floating")]
    [SerializeField] float floatingDrag = 5f;

    //References
    Rigidbody rb;

    //Variables
    Vector3 moveDir;
    float verticalDir;
    float nextTimeToJump = 0f;
    bool isGrounded;
    bool isSpeedingUp;

    public bool IsFloating { get { return isFloating; }
        private set {
            isFloating = value;
            rb.useGravity = !value;
        }
    }
    private bool isFloating = false;

    /// <summary>
    /// Returns the current orientation of the player
    /// </summary>
    /// <returns>The orientation transform of this player</returns>
    public Transform GetOrientation()
    {
        return orientation;
    }

    /// <summary>
    /// Set a direction for the player to move towards
    /// </summary>
    /// <param name="_dir">A normalized direction for the player to move towards</param>
    public void SetDirection(Vector3 _dir)
    {
        moveDir = _dir;
    }

    /// <summary>
    /// Sets a vertical direction for the player to move towards
    /// </summary>
    /// <param name="_vertDir">The direction to move towards on the y-axis</param>
    public void SetVerticalDirection(float _vertDir)
    {
        verticalDir = _vertDir;
    }

    /// <summary>
    /// Toggle the player's movement mode between walking and floating
    /// </summary>
    /// <param name="_state">If true, the player is set to floating mode to move in a 3D axis</param>
    public void SetFloatingMode(bool _state)
    {
        IsFloating = _state;
    }

    /// <summary>
    /// Change the maximum floating speed of the player
    /// </summary>
    /// <param name="_value">The maximum speed the player may reach</param>
    public void SetMaxFloatingSpeed(float _value)
    {
        maxFloatingSpeed = _value;
    }

    /// <summary>
    /// Change the base acceleration of the player while floating
    /// </summary>
    /// <param name="_value">The new floating acceleration of the player</param>
    public void SetFloatingSpeed(float _value)
    {
        floatingSpeed = _value;
    }

    /// <summary>
    /// Change the drag constant for floating movements
    /// </summary>
    /// <param name="_value">The new drag value to apply to the player when floating</param>
    public void SetFloatingDampen(float _value)
    {
        floatingDrag = _value;
    }

    /// <summary>
    /// Speeds up the player's movement with their set multipliers
    /// </summary>
    /// <param name="_state">If true, the player will speed up by their multiplier</param>
    public void SpeedUp(bool _state)
    {
        isSpeedingUp = _state;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = groundDrag;

        nextTimeToJump = jumpRate;
    }

    void FixedUpdate()
    {
        if (isFloating) {
            FloatingMovement();
            LimitFloatingMovement();
        }
        else {
            Movement();
            LimitWalkingMovement();
        }
    }

    void Update()
    {
        if (isFloating) {
            FloatingDrag();
        }
        else {
            WalkingDrag();
            GroundCheck();
            Jump();
        }
    }

    /// <summary>
    /// Moves the player towards the set direction with a speed
    /// </summary>
    void Movement()
    {
        //If walking
        if (isGrounded) {
            Vector3 _vel = moveDir.normalized * walkingSpeed * rb.mass;
            //Holding sprint key
            if (isSpeedingUp) {
                _vel *= walkingSpeedUpMult;
            }
            rb.AddForce(_vel, ForceMode.Force);
        }
        //If midair
        else {
            rb.AddForce(moveDir.normalized * walkingSpeed * rb.mass * walkingAirSpeedMult, ForceMode.Force);
        }
    }

    /// <summary>
    /// Limits the player's speed when walking if exceeds the maximum speed
    /// </summary>
    void LimitWalkingMovement()
    {
        Vector3 _horiVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (_horiVel.magnitude > maxWalkingSpeed) {
            //Apply max speed to the current direction
            Vector3 _limitedVel = _horiVel.normalized * maxWalkingSpeed;
            rb.velocity = new Vector3(_limitedVel.x, rb.velocity.y, _limitedVel.z);
        }
    }

    /// <summary>
    /// Sets the player's drags accordingly between jumping
    /// </summary>
    void WalkingDrag()
    {
        if (isGrounded) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0f;
        }
    }

    /// <summary>
    /// Checks if the player is touching the ground
    /// </summary>
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Applies an upward force on the player to make them jump up
    /// </summary>
    void Jump()
    {
        if (isGrounded && (verticalDir > 0) && (Time.time > nextTimeToJump)) {
            rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
            nextTimeToJump = Time.time + jumpRate;
            isGrounded = false;
        }
    }

    /// <summary>
    /// Moves the player in a 3D axis
    /// </summary>
    void FloatingMovement()
    {
        //If any key pressed
        if (moveDir.magnitude > 0f) {
            Vector3 _horiDir = moveDir.normalized * floatingSpeed * rb.mass;
            //Holding sprint key
            if (isSpeedingUp) {
                _horiDir *= floatingSpeedUpMult;
            }
            rb.AddForce(_horiDir, ForceMode.Force);
        }
        if (verticalDir != 0f) {
            Vector3 _vertDir = Vector3.up * verticalDir * floatingSpeed * rb.mass;
            if (isSpeedingUp) {
                _vertDir *= floatingSpeedUpMult;
            }
            rb.AddForce(_vertDir, ForceMode.Force);
        }
    }

    /// <summary>
    /// Limits the player's speed when floating if exceeds the maximum speed
    /// </summary>
    void LimitFloatingMovement()
    {
        if (rb.velocity.magnitude > maxFloatingSpeed) {
            //Apply max speed to the current direction
            Vector3 _limitedVel = rb.velocity.normalized * maxWalkingSpeed;
            rb.velocity = new Vector3(_limitedVel.x, _limitedVel.y, _limitedVel.z);
        }
    }

    /// <summary>
    /// Sets the player's drag when floating
    /// </summary>
    void FloatingDrag()
    {
        if (rb.drag != floatingDrag)
            rb.drag = floatingDrag;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
