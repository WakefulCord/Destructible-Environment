using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	#region Class References
	Rigidbody rb;
	Transform camTransform;
	#endregion

	#region Private Fields
	[Header("Movement Fields")]
	[SerializeField] private float movementSpeed;
    [SerializeField] private float normalSpeed = 10f;
	[SerializeField] private float sprintSpeed = 20f;


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

	}
	#endregion

	#region Class Methods
	private void HandleMovement(float h, float v, bool isSprinting)
	{
		Vector3 moveAmount = camTransform.forward * v;
		moveAmount += camTransform.right * h;

		movementSpeed = isSprinting ? sprintSpeed : normalSpeed;
		Vector3 movementVector = moveAmount * movementSpeed;

		rb.linearVelocity = new Vector3(movementVector.x, rb.linearVelocity.y, movementVector.z);

	}
	#endregion

	#region Update Methods
	public void OnUpdate(float h, float v, bool isSprinting)
	{
		HandleMovement(h, v, isSprinting);
	}
	#endregion
}
