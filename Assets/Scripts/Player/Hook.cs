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


	public static Hook CreateHook(GameObject owner, Vector3 position, bool isLeft)
	{
		GameObject obj = new GameObject();
		obj.transform.SetParent(owner.transform);
		Hook hook = obj.AddComponent<Hook>();
		hook.owner = owner;
		hook.position = position;
		hook.isLeft = isLeft;
		return hook;
	}

	void Start()
	{

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
