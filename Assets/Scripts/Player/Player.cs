using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool isGrounded = true;
	public float targetAngle;
	[SerializeField]
	private GameObject camera;
	private Rigidbody rb;
	private float speed = 13f;
	private int forward = 0;
	private int right = 0;


	public float getCameraDirection()
	{
		return camera.transform.eulerAngles.y;
	}

	public void CheckInput()
	{
		forward = 0;
		right = 0;
		if (Input.GetKey(KeyCode.W))
			forward = 1;
		else if (Input.GetKey(KeyCode.S))
			forward = -1;
		if (Input.GetKey(KeyCode.A))
			right = -1;
		else if (Input.GetKey(KeyCode.D))
			right = 1;
	}

	public float GetTargetAngle()
	{
		if (forward != 0 || right != 0)
		{
			return targetAngle = camera.transform.rotation.eulerAngles.y + 90f - Mathf.Atan2(forward, right) * Mathf.Rad2Deg;
		}
		else
		{
			return targetAngle = camera.transform.rotation.eulerAngles.y;
		}
	}

	public virtual RaycastHit? RaycastIgnoreTriggers(Vector3 origin, Vector3 direction, float distance)
	{
		var hits = Physics.RaycastAll(origin, direction, distance);
		foreach (var hit in hits)
		{
			if (!hit.collider.isTrigger)
				return hit;
		}
		return null;
	}

	public bool CheckRaycastIgnoreTriggers(Vector3 origin, Vector3 direction, float distance)
	{
		return RaycastIgnoreTriggers(origin, direction, distance).HasValue;
	}


	public bool CheckGround()
	{
		return CheckRaycastIgnoreTriggers(this.transform.position, this.transform.up * -1f, 1f);
	}


	public Vector3 getMoveDirection()
	{
		Vector3 cameraForward = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetDirection = cameraForward * moveDirection.z + camera.transform.right * moveDirection.x;
		return Vector3.Lerp(rb.velocity / speed, targetDirection, 0.2f);
	}

	private void Jump()
	{
		if (CheckGround())
		{
			rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
		}
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
	}

	void Update()
	{
		CheckInput();
		if (CheckGround())
		{
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}

	}



	//地上ではキー操作方向を向く
	//空中ではカメラ方向を向く
	void FixedUpdate()
	{
		if (isGrounded)
		{
			if (forward == 1)
			{
				rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, GetTargetAngle(), 0f), Time.deltaTime * 10f);
				rb.velocity = getMoveDirection() * speed;
			}
			else
			{
				rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 5f);
				rb.velocity = getMoveDirection() * speed * 0.85f;
			}



			if (Input.GetKey(KeyCode.Space))
			{
				Jump();
			}
		}
	}
}
