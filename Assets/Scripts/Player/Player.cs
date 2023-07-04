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
	private const float SPEED = 13f;
	private const float AIRSPEED = 8;
	private int forward = 0;
	private int right = 0;
	private bool rightHookInput = false;
	private bool leftHookInput = false;
	private Hook leftHook;
	private Hook rightHook;

	public float getCameraDirection()
	{
		return camera.transform.eulerAngles.y;
	}

	public Vector3 getHookPoint()
	{
		RaycastHit hit;
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100f))
		{
			return hit.point;
		}
		else
		{
			return camera.transform.position + camera.transform.forward * 100f;
		}
	}

	public void CheckMoveInput()
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

	public void CheckHookInput()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			leftHookInput = true;
		}
		else
		{
			leftHookInput = false;
		}
		if (Input.GetKey(KeyCode.E))
		{
			rightHookInput = true;
		}
		else
		{
			rightHookInput = false;
		}
	}

	public float GetTargetAngle()
	{
		if (forward != 0 || right != 0)
		{
			return targetAngle = getCameraDirection() + 90f - Mathf.Atan2(forward, right) * Mathf.Rad2Deg;
		}
		else
		{
			return targetAngle = getCameraDirection();
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
		return Vector3.Lerp(rb.velocity / SPEED, targetDirection, 0.2f);
	}

	private void InAirMovement()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			rb.AddForce(Vector3.up * AIRSPEED * 1.6f);
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			rb.AddForce(Vector3.down * AIRSPEED);
		}
		if (Input.GetKey(KeyCode.W))
		{
			rb.AddForce(this.transform.forward * AIRSPEED);
		}
		if (Input.GetKey(KeyCode.S))
		{
			rb.AddForce(this.transform.forward * -AIRSPEED);
		}
		if (Input.GetKey(KeyCode.A))
		{
			rb.AddForce(this.transform.right * -AIRSPEED);
		}
		if (Input.GetKey(KeyCode.D))
		{
			rb.AddForce(this.transform.right * AIRSPEED);
		}
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
		leftHook = Hook.CreateHook(this.gameObject, true);
		rightHook = Hook.CreateHook(this.gameObject, false);

		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
	}

	void Update()
	{
		CheckMoveInput();
		CheckHookInput();
		if (CheckGround())
		{
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}

		if (leftHookInput)
		{
			leftHook.SetHook(getHookPoint());
		}
		if (rightHookInput)
		{
			rightHook.SetHook(getHookPoint());
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
				rb.velocity = getMoveDirection() * SPEED;
			}
			else
			{
				rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, getCameraDirection(), 0f), Time.deltaTime * 5f);
				rb.velocity = getMoveDirection() * SPEED * 0.85f;
			}


			if (Input.GetKey(KeyCode.Space))
			{
				Jump();
			}
		}
		else
		{
			rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, getCameraDirection(), 0f), Time.deltaTime * 10f);
			InAirMovement();
		}
	}
}
