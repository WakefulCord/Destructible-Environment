using UnityEngine;

public class PlayerMovementVoxel : MonoBehaviour
{
    public float speed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;

    CharacterController controller;
    Vector3 velocity;
    float xRotation;
    Camera cam;

    [SerializeField] WorldManager worldManager;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

   

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;// * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;// * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = -2f;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlaceBlock();
        }



    }

    private void DestroyBlock()
    {
        
        if (cam == null) 
        {
            return; //safety check to make sure screen to point world cast isn't done on a different angle camera
        }

        // Ray from screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        RaycastHit hit;   

        if (Physics.Raycast(ray, out hit, 10f))
        {           
            Vector3 removePos = hit.point - hit.normal * 0.01f;
            worldManager.DestroyMultiple(removePos, 2);
        }
    }

    private void PlaceBlock()
    {
        if (cam == null)
        {
            return; //safety check to make sure screen to point world cast isn't done on a different angle camera
        }

        // Ray from screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            Vector3 addPos = hit.point + hit.normal * 0.01f;
            worldManager.CreateBlock(addPos, 1);
        }
    }
}
