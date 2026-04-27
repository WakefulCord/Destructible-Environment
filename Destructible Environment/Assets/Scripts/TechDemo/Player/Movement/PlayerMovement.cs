using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Class References
    Transform camTransform;
    Rigidbody rb;
    #endregion

    #region Private Fields
    [Header("Movement Fields")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;


    private float movementSpeed;


    [Header("Grounded Fields")]
    [SerializeField] private float groundRayOffset;
    [SerializeField] private float groundRayDist;

    [SerializeField] private LayerMask ignoreForGroundCheck;
    [SerializeField] private bool isGrounded;

    [Header("Jump Fields")]
    [SerializeField] private float jumpForce;

    [SerializeField] private float jumpTimer;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private bool canJump;

    #endregion

    #region Properties
    public bool IsGrounded => isGrounded;

    
    #endregion

    #region Start Up


    public void OnAwake()
    {
        rb = GetComponent<Rigidbody>();
        camTransform = Camera.main.transform;
    }
    public void OnStart()
    {
        movementSpeed = walkSpeed;

        canJump = true;
        jumpTimer = 0;

    }
    #endregion

    #region Class Methods


    public void OnUpdate(float h, float v, bool isSprinting)
    {
        isGrounded = HandleGroundCheck();


        if (isGrounded)
            HandleMovement(h, v, isSprinting);


        if (!canJump)
        {
            jumpTimer += Time.deltaTime;

            if (jumpTimer >= jumpCooldown)
            {
                canJump = true;
                jumpTimer = 0;
            }
        }
    }

    private bool HandleGroundCheck()
    {
        Vector3 origin = transform.position;
        origin.y += groundRayOffset;

        if (Physics.Raycast(origin, -Vector3.up, groundRayDist, ~ignoreForGroundCheck))
        {
            return true;
        }
        return false;
    }

    public void HandleJump()
    {
        if (canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
            jumpTimer = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin = transform.position;
        origin.y += groundRayOffset;
        Gizmos.DrawRay(origin, -Vector3.up * groundRayDist);
    }
    private void HandleMovement(float hor, float vert, bool sprint)
    {

        //direction of movement
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * vert + right * hor;

        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        //apply speed 

        movementSpeed = sprint ? sprintSpeed : walkSpeed;

        Vector3 newVel = new Vector3(moveDir.x * movementSpeed, rb.linearVelocity.y, moveDir.z * movementSpeed );
        //apply new velocity
        rb.linearVelocity = newVel;

    }
    #endregion
}
