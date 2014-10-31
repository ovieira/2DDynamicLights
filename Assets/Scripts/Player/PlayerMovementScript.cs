using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour {
	public float speed;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate()
	{
		
		if (Application.platform != RuntimePlatform.Android)
		{
			if (Input.GetButton("Vertical"))
			{
				//transform.position += (Input.GetAxis("Vertical") * Vector3.up * speed * Time.deltaTime);
				rigidbody2D.AddForce(Vector3.up * Input.GetAxis("Vertical") * speed);
			}
			if (Input.GetButton("Horizontal"))
			{
				//transform.position += (Input.GetAxis("Horizontal") * Vector3.right * speed * Time.deltaTime);
				rigidbody2D.AddForce(Vector3.right * Input.GetAxis("Horizontal") * speed);
			}
		}
		else
		{
			if (Input.acceleration.y != 0)
			{
				//transform.position += (Input.GetAxis("Vertical") * Vector3.up * speed * Time.deltaTime);
				rigidbody2D.AddForce(Vector3.up * Input.acceleration.y * speed);

			}
			if (Input.acceleration.x != 0)
			{
				//transform.position += (Input.GetAxis("Horizontal") * Vector3.right * speed * Time.deltaTime);
				rigidbody2D.AddForce(Vector3.right * Input.acceleration.x * speed);

			}
		}
	}
}
