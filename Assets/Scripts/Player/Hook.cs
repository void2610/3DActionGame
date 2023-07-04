using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
	public GameObject owner
	{
		get; private set;
	}
	public Vector3 position
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

	public LineRenderer lineRenderer;


	public static Hook CreateHook(GameObject owner, bool isLeft)
	{
		GameObject obj = new GameObject();
		obj.transform.SetParent(owner.transform);
		Hook hook = obj.AddComponent<Hook>();

		hook.lineRenderer = obj.AddComponent<LineRenderer>();
		hook.lineRenderer.startWidth = 0.1f;
		hook.lineRenderer.endWidth = 0.1f;
		hook.lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		hook.lineRenderer.startColor = Color.black;
		hook.lineRenderer.endColor = Color.black;

		hook.owner = owner;
		hook.isLeft = isLeft;
		return hook;
	}

	public void SetHook(Vector3 position)
	{
		this.position = position;
		state = HookState.Hooked;
	}

	void Start()
	{

	}

	void Update()
	{
		switch (state)
		{
			case HookState.Disabled:
				lineRenderer.SetPosition(0, owner.transform.position);
				lineRenderer.SetPosition(1, owner.transform.position);
				break;
			case HookState.Hooking:
				break;
			case HookState.Hooked:
				lineRenderer.SetPosition(0, owner.transform.position);
				lineRenderer.SetPosition(1, this.position);
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
