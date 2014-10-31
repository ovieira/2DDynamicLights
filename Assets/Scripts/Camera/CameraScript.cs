using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	GameObject player;
	Transform player_transform;
	public float horizontal_factor = 10f;
	public float vertical_factor = 3.6f;
	public float raycast_dist_horizontal,raycast_dist_vertical, w, h;
	public float speed;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		player_transform = player.transform;
		w = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
		h = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
		raycast_dist_horizontal = w * 0.75f;
		raycast_dist_vertical = h * 0.75f;
		transform.position = new Vector3(player_transform.position.x, player_transform.position.y, transform.position.z);
		checkInitialPosition();
	}
	void Update(){

	}
	// Update is called once per frame
	void FixedUpdate () {

		Vector3 new_pos = new Vector3 (player_transform.position.x, player_transform.position.y, transform.position.z);

		RaycastHit2D rightHit = Physics2D.Raycast (player_transform.position, Vector2.right, raycast_dist_horizontal, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D leftHit = Physics2D.Raycast (player_transform.position, -Vector2.right, raycast_dist_horizontal, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D upHit = Physics2D.Raycast (player_transform.position, Vector2.up, raycast_dist_vertical, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D downHit = Physics2D.Raycast (player_transform.position, -Vector2.up, raycast_dist_vertical, 1 << LayerMask.NameToLayer("Border"));

		Debug.DrawRay(player_transform.position, Vector2.right * raycast_dist_horizontal, Color.blue);
		Debug.DrawRay(player_transform.position, -Vector2.right * raycast_dist_horizontal, Color.blue);
		Debug.DrawRay(player_transform.position, Vector2.up * raycast_dist_vertical , Color.blue);
		Debug.DrawRay(player_transform.position, -Vector2.up * raycast_dist_vertical , Color.blue);

		if (rightHit.collider != null && rightHit.collider.tag == "Border") {
			new_pos.x = transform.position.x;	
		}
		if (leftHit.collider != null && leftHit.collider.tag == "Border") {
			new_pos.x = transform.position.x;	
		}
		if (upHit.collider != null && upHit.collider.tag == "Border") {
			new_pos.y = transform.position.y;	
		}
		if (downHit.collider != null && downHit.collider.tag == "Border") {
			new_pos.y = transform.position.y;	
		}

		//transform.position = new Vector3(player_transform.position.x, player_transform.position.y, transform.position.z);
		//transform.position = new_pos;
		transform.position = Vector3.Lerp (transform.position, new_pos, Time.deltaTime * speed);
	}

	void checkInitialPosition(){
		Vector3 new_pos = new Vector3 (player_transform.position.x, player_transform.position.y, transform.position.z);
		
		RaycastHit2D rightHit = Physics2D.Raycast (player_transform.position, Vector2.right, raycast_dist_horizontal, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D leftHit = Physics2D.Raycast (player_transform.position, -Vector2.right, raycast_dist_horizontal, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D upHit = Physics2D.Raycast (player_transform.position, Vector2.up, raycast_dist_vertical, 1 << LayerMask.NameToLayer("Border"));
		RaycastHit2D downHit = Physics2D.Raycast (player_transform.position, -Vector2.up, raycast_dist_vertical, 1 << LayerMask.NameToLayer("Border"));

		if (rightHit.collider != null && rightHit.collider.tag == "Border") {
			new_pos.x = rightHit.point.x - raycast_dist_horizontal;	
		}
		if (leftHit.collider != null && leftHit.collider.tag == "Border") {
			new_pos.x = leftHit.point.x + raycast_dist_horizontal;
		}
		if (upHit.collider != null && upHit.collider.tag == "Border") {
			new_pos.y = upHit.point.y - raycast_dist_vertical;
		}
		if (downHit.collider != null && downHit.collider.tag == "Border") {
			new_pos.y = downHit.point.y + raycast_dist_vertical;
		}
		transform.position = new_pos;
	}
}
