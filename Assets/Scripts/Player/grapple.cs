using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapple : MonoBehaviour
{
    public GameObject camera;
    public GameObject player;
    public SpringJoint joint;
    public LineRenderer lineRenderer;

    private Vector3 grapplePoint;
    private void SetGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, camera.transform.forward, out hit, 1000f))
        {
            grapplePoint = hit.point;

            lineRenderer = this.gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            joint = this.gameObject.AddComponent<SpringJoint>();
            joint.connectedBody = hit.rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            joint.maxDistance = Vector3.Distance(this.transform.position, grapplePoint) * 0.01f;
            //joint.minDistance = Vector3.Distance(this.transform.position, grapplePoint) * 0.025f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }

    private void RemoveGrapple()
    {
        Destroy(joint);
        Destroy(lineRenderer);
    }

    void Awake()
    {
        player = this.gameObject;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            RemoveGrapple();
        }
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }
}
