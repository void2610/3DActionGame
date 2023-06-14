using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
	private GameObject player;
	[SerializeField]
	private GameObject camera;
	private Rigidbody rb;

	[SerializeField]
	private float speed = 10f;
	private Vector3 latestPos;
	private Vector3 moving;
	private float direction;


	void MovementControll()
	{
		//斜め移動と縦横の移動を同じ速度にするためにVector3をNormalize()する。
		moving = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moving.Normalize();
		moving = moving * speed;
		//カメラの向きに合わせて移動方向を変える
		moving = Quaternion.Euler(0, direction, 0) * moving;
	}

	void Movement()
	{
		rb.velocity = moving;
	}


	void Start()
	{
		player = this.gameObject;
		rb = player.GetComponent<Rigidbody>();
	}

	void Update()
	{
		//カメラの向きを取得
		direction = camera.transform.eulerAngles.y;
	}

	void FixedUpdate()
	{
		MovementControll();
		Movement();
	}
}
