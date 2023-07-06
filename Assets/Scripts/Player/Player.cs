using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable IDE0044

public class Player : MonoBehaviour
{
	[SerializeField]
	private GameObject camera;
	[SerializeField]
	private ParticleSystem particleSystem;
	[SerializeField]
	private Hook leftHook;
	[SerializeField]
	private Hook rightHook;
	[SerializeField]
	private LayerMask hookableLayer;

	private Rigidbody rb;
	private const float SPEED = 13f;
	private const float AIRSPEED = 9;
	private const float MAXDISTANCE = 150f;
	private bool isGrounded = true;
	private float targetAngle;
	private int forward = 0;
	private int right = 0;
	private readonly bool rightHookInputDown = false;
	private readonly bool leftHookInputDown = false;
	private readonly bool rightHookInput = false;
	private readonly bool leftHookInput = false;
	private readonly bool leftHookInputUp = false;
	private readonly bool rightHookInputUp = false;


	public float GetCameraDirection()
	{
		return camera.transform.eulerAngles.y;
	}

	public Vector3 GetHookPoint()
	{
		if (Physics.Raycast(this.transform.position, camera.transform.forward, out RaycastHit hit, MAXDISTANCE, hookableLayer))
		{
			return hit.point;
		}
		else
		{
			return Vector3.zero;
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

	private void CheckHookInput()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			leftHook.SetHook(GetHookPoint(), this.gameObject);
		}
		else if (Input.GetKey(KeyCode.Q))
		{
		}
		else if (Input.GetKeyUp(KeyCode.Q))
		{
			leftHook.DisableHook();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			rightHook.SetHook(GetHookPoint(), this.gameObject);
		}
		else if (Input.GetKey(KeyCode.E))
		{
		}
		else if (Input.GetKeyUp(KeyCode.E))
		{
			rightHook.DisableHook();
		}

		//マウスホイールでのフックの長さ調整
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			leftHook.LilleWire(10f);
			rightHook.LilleWire(10f);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			leftHook.LilleWire(-10f);
			rightHook.LilleWire(-10f);
		}
	}

	public float GetTargetAngle()
	{
		if (forward != 0 || right != 0)
		{
			return targetAngle = GetCameraDirection() + 90f - Mathf.Atan2(forward, right) * Mathf.Rad2Deg;
		}
		else
		{
			return targetAngle = GetCameraDirection();
		}
	}

	public virtual RaycastHit? RaycastIgnoreTriggers(Vector3 origin, Vector3 direction, float distance)
	{
		var hits = Physics.RaycastAll(origin, direction, distance);
		foreach (var hit in hits)
		{
			if (!hit.collider.isTrigger)
			{
				return hit;
			}
		}
		return null;
	}

	public bool CheckRaycastIgnoreTriggers(Vector3 origin, Vector3 direction, float distance)
	{
		return RaycastIgnoreTriggers(origin, direction, distance).HasValue;
	}


	public bool CheckGround()
	{
		return CheckRaycastIgnoreTriggers(this.transform.position, this.transform.up * -1f, 1.1f);
	}


	public Vector3 GetMoveDirection()
	{
		Vector3 cameraForward = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetDirection = cameraForward * moveDirection.z + camera.transform.right * moveDirection.x;
		return Vector3.Lerp(rb.velocity / SPEED, targetDirection, 0.2f);
	}

	public Vector3 GetGasDirection(){
		Vector3 direction = Vector3.zero;
		if (Input.GetKey(KeyCode.Space))
		{
			direction += Vector3.up;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			direction += Vector3.down;
		}
		if (Input.GetKey(KeyCode.W))
		{
			direction += this.transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			direction += this.transform.forward * -1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			direction += this.transform.right * -1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			direction += this.transform.right;
		}

		return direction;
	}

	private void InAirMovement()
	{
		Vector3 d = GetGasDirection();
		if(Input.GetKey(KeyCode.LeftShift)){
			GasMovement();
		}

		if (d != Vector3.zero)
		{
			particleSystem.enableEmission = true;
			particleSystem.transform.rotation = Quaternion.LookRotation(d);
		}
		else
		{
			particleSystem.enableEmission = false;
		}
	}

	private void Jump()
	{
		if (CheckGround())
		{
			rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
		}
	}

	private void GasMovement()
	{
		if(leftHook.state == Hook.HookState.Disabled && rightHook.state == Hook.HookState.Disabled){
			return;
		}

		rb.velocity = GetGasDirection() * AIRSPEED * 10f;
		rightHook.SetWireLengthToPlayerDistance();
		leftHook.SetWireLengthToPlayerDistance();
	}

	void Start()
	{
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
	}



	//地上ではキー操作方向を向く
	//空中ではカメラ方向を向く
	void FixedUpdate()
	{
		if (isGrounded)
		{
			particleSystem.enableEmission = false;
			if (forward == 1)
			{
				rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, GetTargetAngle(), 0f), Time.deltaTime * 10f);
				rb.velocity = GetMoveDirection() * SPEED;
			}
			else
			{
				rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, GetCameraDirection(), 0f), Time.deltaTime * 5f);
				rb.velocity = GetMoveDirection() * SPEED * 0.85f;
			}

			if (Input.GetKey(KeyCode.Space))
			{
				Jump();
			}
		}
		else
		{
			rb.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0f, GetCameraDirection(), 0f), Time.deltaTime * 10f);
			InAirMovement();
		}
	}
}
