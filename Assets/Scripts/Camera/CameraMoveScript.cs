using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveScript : MonoBehaviour
{
	public Transform target;
	public float distanceToPlayerM = 2f;    // カメラとプレイヤーとの距離[m]
	public float slideDistanceM = 0f;       // カメラを横にスライドさせる；プラスの時右へ，マイナスの時左へ[m]
	public float heightM = 1.2f;            // 注視点の高さ[m]
	public float rotationSensitivity = 100f;// 感度

	private float floorHeight = 0.0f;
	private float minDistance = 0.8f;

	private RaycastHit hit;

	private Vector3 position;

	private float distance;

	private int mask;

	void Start()
	{
		position = target.transform.position - this.transform.position;
		distance = Vector3.Distance(target.transform.position, transform.position);
		mask = ~(1 << LayerMask.NameToLayer("Player"));
	}

	void FixedUpdate()
	{
		floorHeight = target.position.y + 1;

		var rotX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSensitivity;
		var rotY = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSensitivity;

		var lookAt = target.position + Vector3.up * heightM;

		// 回転
		transform.RotateAround(lookAt, Vector3.up, rotX);
		// カメラがプレイヤーの真上や真下にあるときにそれ以上回転させないようにする
		if (transform.forward.y > 0.9f && rotY < 0)
		{
			rotY = 0;
		}
		if (transform.forward.y < -0.9f && rotY > 0)
		{
			rotY = 0;
		}
		transform.RotateAround(lookAt, transform.right, rotY);

		// カメラとプレイヤーとの間の距離を調整
		transform.position = lookAt - transform.forward * distanceToPlayerM;

		// 注視点の設定
		transform.LookAt(lookAt);

		// カメラを横にずらして中央を開ける
		transform.position = transform.position + transform.right * slideDistanceM;

		// カメラの高さを調整
		if (transform.position.y < floorHeight)
		{
			float diff = floorHeight - transform.position.y;

			if (diff > minDistance)
			{
				diff = minDistance;
			}
			Debug.Log(diff);
			//diffが大きくなればなるほどtargetに近づく
			float x = target.position.x + (transform.position.x - target.position.x) * (1 - diff);
			float z = target.position.z + (transform.position.z - target.position.z) * (1 - diff);

			transform.position = new Vector3(x, floorHeight, z);
		}



		// //めり込み防止
		// if (Physics.CheckSphere(target.transform.position, 0.3f, mask))
		// {
		// 	transform.position = Vector3.Lerp(transform.position, target.transform.position, 1);
		// }
		// else if (Physics.SphereCast(target.transform.position, 0.3f, (transform.position - target.transform.position).normalized, out hit, distance, mask))
		// {
		// 	transform.position = target.transform.position + (transform.position - target.transform.position).normalized * hit.distance;
		// }
		// else
		// {
		// 	transform.localPosition = Vector3.Lerp(transform.localPosition, position, 1);
		// }
	}
}
