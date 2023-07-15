using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable IDE0044

public class Player : MonoBehaviour
{
	[SerializeField]
	private new GameObject camera;
	[SerializeField]
	private new ParticleSystem particleSystem;
	[SerializeField]
	private Hook leftHook;
	[SerializeField]
	private Hook rightHook;
	[SerializeField]
	private LayerMask hookableLayer;

	private Rigidbody rb;
	[SerializeField]
	private float SPEED = 13f;
	[SerializeField]
	private float AIRSPEED = 9;
	[SerializeField]
	private float JUMPSPEED = 10f;
	private const float MAXDISTANCE = 150f;
	private const float MAXSPEED = 100f;
	private bool isGrounded = true;
	private float targetAngle;
	private int forward = 0;
	private int right = 0;
	private Vector3 gasTargetPosition;
	private bool isUsingGas = false;
	private bool oldIsUsingGas = false;


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
			leftHook.ReelWire(10f);
			rightHook.ReelWire(10f);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			leftHook.ReelWire(-10f);
			rightHook.ReelWire(-10f);
		}
	}

	public void CheckGasInput()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			isUsingGas = true;
		}
		else if (Input.GetKey(KeyCode.LeftShift))
		{
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			isUsingGas = false;
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

	public Vector3 GetGasDirection()
	{
		Vector3 direction = Vector3.zero;
		if (Input.GetKey(KeyCode.Space))
		{
			direction += Vector3.up;
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
		if (d != Vector3.zero)
		{
			//particleSystem.enableEmission = true;
			particleSystem.transform.rotation = Quaternion.LookRotation(d);
			rb.AddForce(d * 1f * AIRSPEED);
		}
		else
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
	}

	private void Jump()
	{
		if (CheckGround())
		{
			rb.AddForce(Vector3.up * JUMPSPEED, ForceMode.Impulse);
		}
	}

	private void SetGasTargetPosition()
	{
		if (leftHook.state == Hook.HookState.Disabled && rightHook.state == Hook.HookState.Disabled)
		{
			return;
		}

		Vector3 d = Vector3.zero;
		if (leftHook.state == Hook.HookState.Hooked)
		{
			d += leftHook.GetTargetPosition() - this.transform.position;
			leftHook.SetWireLength(0f);
		}
		if (rightHook.state == Hook.HookState.Hooked)
		{
			d += rightHook.GetTargetPosition() - this.transform.position;
			rightHook.SetWireLength(0f);
		}

		gasTargetPosition = d;
	}

	private void ResetGasTargetPosition()
	{
		gasTargetPosition = Vector3.zero;
		if (leftHook.state == Hook.HookState.Hooked)
		{
			leftHook.SetWireLength(leftHook.GetWireLength());
		}
		if (rightHook.state == Hook.HookState.Hooked)
		{
			rightHook.SetWireLength(rightHook.GetWireLength());
		}
	}

	private void StrongReelWire()
	{
		if (leftHook.state == Hook.HookState.Hooked)
		{
			leftHook.ReelWire(100f);
		}
		if (rightHook.state == Hook.HookState.Hooked)
		{
			rightHook.ReelWire(100f);
		}
	}

	private void GasMovement()
	{
		if (leftHook.state == Hook.HookState.Disabled && rightHook.state == Hook.HookState.Disabled)
		{
			return;
		}

		//Vector3.Lerp(rb.velocity, rb.velocity + gasTargetPosition, 100f);
		rb.AddForce(gasTargetPosition * 10);
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = true;
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
		CheckGasInput();
		if (CheckGround())
		{
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}
	}

	void FixedUpdate()
	{
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = false;
		if (isGrounded)
		{
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

		if (isUsingGas && !oldIsUsingGas)
		{
			SetGasTargetPosition();
		}
		if (isUsingGas)
		{
			GasMovement();
		}
		if (!isUsingGas && oldIsUsingGas)
		{
			ResetGasTargetPosition();
		}

		oldIsUsingGas = isUsingGas;
	}
}
