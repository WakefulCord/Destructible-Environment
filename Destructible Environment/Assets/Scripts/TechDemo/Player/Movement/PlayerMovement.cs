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

    #endregion

    #region Properties

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

    }
    #endregion

    #region Class Methods


    public void OnUpdate(float h, float v, bool isSprinting)
    {
        HandleMovement(h, v, isSprinting);
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
