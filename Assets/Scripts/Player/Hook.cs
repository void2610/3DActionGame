using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
	public GameObject player;
	public Vector3 targetPosition;
	public HookState state = HookState.Disabled;

	public SpringJoint joint;
	public LineRenderer lineRenderer;

	public void SetHook(Vector3 target, GameObject player)
	{
		if (target == Vector3.zero)
		{
			return;
		}

		this.player = player;
		this.targetPosition = target;
		joint = player.gameObject.AddComponent<SpringJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.connectedAnchor = targetPosition;
		joint.maxDistance = GetWireLength();
		joint.minDistance = 0;
		joint.spring = 4.5f;
		joint.damper = 7f;
		joint.massScale = 4.5f;
		lineRenderer.positionCount = 2;
		state = HookState.Hooked;
	}

	public void DisableHook()
	{
		state = HookState.Disabled;
		targetPosition = Vector3.zero;
		lineRenderer.positionCount = 0;
		Destroy(joint);
	}

	public Vector3 GetTargetPosition()
	{
		return targetPosition;
	}

	public void ReelWire(float reelLength)
	{
		if (joint != null && joint.maxDistance - reelLength > 0)
		{
			joint.maxDistance -= reelLength;
		}
	}

	public float GetWireLength()
	{
		return Vector3.Distance(player.transform.position, targetPosition);
	}

	public void SetWireLength(float length)
	{
		if (joint != null)
		{
			joint.maxDistance = length;
		}
	}

	void Start()
	{
		lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = Color.black;
		lineRenderer.endColor = Color.black;
		DisableHook();
	}

	void Update()
	{
		switch (state)
		{
			case HookState.Disabled:
			break;
			case HookState.Hooking:
			break;
			case HookState.Hooked:
			lineRenderer.SetPosition(0, this.transform.position);
			lineRenderer.SetPosition(1, targetPosition);
			break;
			default:
			break;
		}
	}

	void FixedUpdate()
	{
		switch (state)
		{
			case HookState.Disabled:
			break;
			case HookState.Hooking:
			break;
			case HookState.Hooked:
			break;
			default:
			break;
		}
	}

	public enum HookState
	{
		Hooking,
		Hooked,
		Disabled
	}
}
