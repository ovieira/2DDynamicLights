using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform target;
	
	public float speed;
	// Use this for initialization
	void Start () {
	}
	void Update(){

        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
 
	}
}
