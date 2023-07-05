using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
	public GameObject player
	{
		get; private set;
	}
	public Vector3 targetPosition
	{
		get; private set;
	}
	public HookState state
	{
		get; private set;
	} = HookState.Disabled;

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
		joint.maxDistance = getHookLength();
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
		lineRenderer.positionCount = 0;
		Destroy(joint);
	}

	public float getHookLength()
	{
		return Vector3.Distance(player.transform.position, targetPosition);
	}

	public void setLineRenderer()
	{

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
				lineRenderer.SetPosition(0, player.transform.position);
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
