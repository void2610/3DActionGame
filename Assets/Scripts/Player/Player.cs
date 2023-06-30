using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	private GameObject camera;
	private Rigidbody rb;
	private float speed = 13f;
	public bool isGrounded = true;
	public float targetAngle;

	public float getCameraDirection()
	{
		return camera.transform.eulerAngles.y;
	}

	private Vector3 getMoveDirection()
	{
		Vector3 cameraForward = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetDirection = cameraForward * moveDirection.z + camera.transform.right * moveDirection.x;
		return Vector3.Lerp(rb.velocity / speed, targetDirection, 0.2f);
	}

	public void CheckInput()
	{
		int forward = 0;
		int right = 0;
		if (Input.GetKey(KeyCode.W))
			forward = 1;
		else if (Input.GetKey(KeyCode.S))
			forward = -1;
		if (Input.GetKey(KeyCode.A))
			right = -1;
		else if (Input.GetKey(KeyCode.D))
			right = 1;
		if (forward != 0 || right != 0)
		{
			targetAngle = camera.transform.rotation.eulerAngles.y + 90f - Mathf.Atan2(forward, right) * Mathf.Rad2Deg;
		}
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		CheckInput();
	}

	//地上ではキー操作方向を向く
	//空中ではカメラ方向を向く
	void FixedUpdate()
	{
		if (isGrounded)
		{
			rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), Time.deltaTime * 10f);

			rb.velocity = getMoveDirection() * speed;
			// // 移動方向を向く
			// if (rb.velocity.magnitude > 0.1f)
			// {
			// 	this.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.eulerAngles, getMoveDirection(), 0.2f));
			// 	this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
			// }
		}
	}
}
