using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveScript : MonoBehaviour
{
	public GameObject target;
	public float distanceToPlayerM = 2f;    // カメラとプレイヤーとの距離[m]
	public float slideDistanceM = 0f;       // カメラを横にスライドさせる；プラスの時右へ，マイナスの時左へ[m]
	public float heightM = 1.2f;            // 注視点の高さ[m]
	public float rotationSensitivity = 100f;// 感度

	private float floorHeight = 0.0f;
	private float minDistance = 3.8f;

	private RaycastHit hit;
	private RaycastHit[] hitList;
	private RaycastHit[] oldHitList = { new RaycastHit() };

	private Vector3 position;

	private float distance;

	private int playerMask;
	private Color playerColor;

	void Start()
	{
		position = target.transform.position - this.transform.position;
		distance = Vector3.Distance(target.transform.position, transform.position);
		playerMask = ~(1 << LayerMask.NameToLayer("Player"));
		playerColor = target.GetComponent<Renderer>().material.color;
	}

	void FixedUpdate()
	{
		floorHeight = target.transform.position.y + 1;

		var rotX = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSensitivity;
		var rotY = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSensitivity;

		var lookAt = target.transform.position + Vector3.up * heightM;

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

		// //めり込んだオブジェクトを透過する
		// foreach (RaycastHit hit in oldHitList)
		// {
		// 	if (hit.collider != null)
		// 	{
		// 		hit.collider.gameObject.GetComponent<Renderer>().enabled = true;
		// 	}
		// }

		// hitList = Physics.RaycastAll(lookAt, transform.position - lookAt, distanceToPlayerM, playerMask);
		// foreach (RaycastHit hit in hitList)
		// {
		// 	hit.collider.gameObject.GetComponent<Renderer>().enabled = false;
		// }


		//めり込みそうなオブジェクトの手前にカメラを移動させる
		if (Physics.Linecast(lookAt, transform.position, out hit, playerMask))
		{
			//lookAtに近づきすぎたらtargetを半透明にする
			if (Vector3.Distance(lookAt, hit.point) < minDistance)
			{
				Debug.Log("近づきすぎ");
				target.GetComponent<Renderer>().material.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.2f);
			}
			else
			{
				target.GetComponent<Renderer>().material.color = playerColor;
			}
			transform.position = hit.point;
		}
		oldHitList = hitList;
	}
}
