using UnityEngine;
using System.Collections;

public class JointScript : MonoBehaviour {

    Transform attached;

	// Use this for initialization
	void Start () {
        attached = GetComponent<SpringJoint2D>().connectedBody.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(transform.position, attached.position, Color.green);
	}

}
