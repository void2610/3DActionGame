using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
	public GameObject player
	{
		get; private set;
	}
	public Vector3 position
	{
		get; private set;
	}
	public Vector3 targetPosition
	{
		get; private set;
	}
	public bool isLeft
	{
		get; private set;
	}
	public HookState state
	{
		get; private set;
	} = HookState.Disabled;
	public const float tension = 1f;
	public const float maxDistance = 10f;

	public SpringJoint joint;
	public LineRenderer lineRenderer;

	public void SetHook(Vector3 target)
	{
		targetPosition = target;

		lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = Color.black;
		lineRenderer.endColor = Color.black;


		joint = this.gameObject.AddComponent<SpringJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.spring = 4.5f;
		joint.damper = 7f;

		// joint.connectedBody = player.GetComponent<Rigidbody>();
		// joint.connectedAnchor = Vector3.zero;

		joint.maxDistance = getHookLength() * 0.08f;
		joint.minDistance = getHookLength() * 0.25f;

		state = HookState.Hooked;
	}

	public void DisableHook()
	{
		state = HookState.Disabled;
		joint.connectedBody = null;
		Destroy(joint);
		Destroy(lineRenderer);
	}

	public float getHookLength()
	{
		return Vector3.Distance(player.transform.position, targetPosition);
	}

	void Start()
	{

	}

	void Update()
	{
		switch (state)
		{
			case HookState.Disabled:
				lineRenderer.SetPosition(0, player.transform.position);
				lineRenderer.SetPosition(1, player.transform.position);
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
