using System.Collections;
using System.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    Vector2 moveInput;
    Vector2 lookInput;

    Camera cam;
    float camRot;
    [SerializeField] float minLookX = -80f;
    [SerializeField] float maxLookX = 80f;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float mouseSensVert;
    [SerializeField] float mouseSensHor;
    [SerializeField] float hitDistance;

    [SerializeField] float hitCooldown;
    bool canHit = true;

    [SerializeField] float exForce;
    [SerializeField] float exRadius;

    [SerializeField] float damageRadius;
    [SerializeField] float spread;

    LayerMask brokenVoxMask;

    private void Start()
    {
        brokenVoxMask = ~(1 << LayerMask.NameToLayer("BrokenVox"));   //creates a layermask to ignore the 'BrokenVox' layer
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnHit(InputAction.CallbackContext context)
    {
        if (context.performed && canHit)
        {
            canHit = false;

            CheckWalls();
            StartCoroutine(HitCooldown());
        }
    }

    IEnumerator HitCooldown()
    {
        float timer = 0f;
        while (timer < hitCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        canHit = true;
    }

    void HandleMovement()   //handles the players movement
    {
        Vector3 movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
    }

    void HandleLook()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, lookInput.x * mouseSensHor * Time.fixedDeltaTime, 0f);

        camRot -= lookInput.y * mouseSensVert;
        camRot = Mathf.Clamp(camRot, minLookX, maxLookX);
        cam.transform.localRotation = Quaternion.Euler(camRot, 0, 0);
    }

    void CheckWalls()   //checks for walls in front of the player
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, hitDistance, brokenVoxMask))
        {
            if (hit.collider.CompareTag("Wall"))    //if hit wall of switch type
            {
                hit.collider.GetComponentInParent<WallManager>().breakWall();

                Vector3 dirToPlayer = (gameObject.transform.position - hit.point).normalized;
                Vector3 exPos = (hit.point + 1 * dirToPlayer);

                Transform Parent = hit.collider.transform.parent;   //adds explosion force to knock wall pieces back
                foreach (Transform child in Parent) //finds broken wall
                {
                    if (child.tag == "BrokenWall")
                        foreach (Transform children in child)    //finds each wall piece
                            children.GetComponentInChildren<Rigidbody>().AddExplosionForce(exForce, exPos, exRadius);
                }

            }

            else //hit wall of voxel type
            {
                float  radiusOverDistance = damageRadius + Vector3.Distance(hit.point, transform.position) * spread; //adds shotgun style spread
                foreach (Collider collider in Physics.OverlapSphere(hit.point, radiusOverDistance))    //breaks each voxel in a radius
                {
                    if (collider.GetComponent<WallVoxel>() != null && Vector3.Distance(hit.point, transform.position) <= hitDistance)
                        collider.GetComponent<WallVoxel>().breakVoxel();
                }
            }

        }
    }
}