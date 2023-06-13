using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
	[SerializeField]
	private GameObject player;
	private Rigidbody rb;

	[SerializeField]
	private float speed = 10f;
	void Start()
	{
		player = this.gameObject;
		rb = player.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		rb.velocity = Vector3.zero;

		if (Input.GetKey(KeyCode.W))
		{
			rb.velocity += Vector3.forward * speed;
		}
		if (Input.GetKey(KeyCode.S))
		{
			rb.velocity += Vector3.back * speed;
		}
		if (Input.GetKey(KeyCode.A))
		{
			rb.velocity += Vector3.left * speed;
		}
		if (Input.GetKey(KeyCode.D))
		{
			rb.velocity += Vector3.right * speed;
		}
	}
}