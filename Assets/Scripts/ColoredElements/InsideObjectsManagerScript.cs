using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InsideObjectsManagerScript : LitManagerScript{

	ArrayList insideObjects = new ArrayList();
	Dictionary<GameObject, Vector3> tangentObjects = new Dictionary<GameObject, Vector3>();

	GameObject player;
	bool playerInside = false;

	override protected void isLit(){
		disableCollision(player, true);
		foreach(GameObject obj in insideObjects){
			disableCollision(obj, true);
		}
	}
	
	override protected void isUnlit(){
		if(!playerInside)disableCollision(player, false);
		/*foreach(GameObject obj in insideObjects){
			Physics2D.IgnoreCollision(collider2D, obj.GetComponent<Collider2D>(), false);
		}*/
	}

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void disableCollision(GameObject obj, bool disable){
		Collider2D[] colls = transform.root.GetComponents<Collider2D>();
		foreach(Collider2D col in colls){
			if(!col.isTrigger){
				Physics2D.IgnoreCollision(col, obj.GetComponent<Collider2D>(), disable);
				break;
			}
		}
	}

	bool canAddInsideObject(Collider2D col){
		if(!insideObjects.Contains(col.transform.root.gameObject) && 
		   ((tangentObjects.ContainsKey(col.transform.root.gameObject) && tangentObjects [col.transform.root.gameObject] != col.transform.root.position) ||
		 !tangentObjects.ContainsKey(col.transform.root.gameObject))){
			return true;
		} else{
			return false;
		}
	}

	void OnCollisionStay2D(Collision2D col){
		if(lit){
			if(canAddInsideObject(col.collider)){
				insideObjects.Add(col.transform.root.gameObject);
				Physics2D.IgnoreCollision(transform.root.GetComponent<Collider2D>(), col.transform.root.GetComponent<Collider2D>(), true);
			}
		} else{
			if(!tangentObjects.ContainsKey(col.transform.root.gameObject)){
				tangentObjects.Add(col.transform.root.gameObject, col.transform.root.position);
			} else{
				tangentObjects [col.transform.root.gameObject] = col.transform.root.position;
			}
		}
	}

	void OnCollisionExit2D(Collision2D col){
		insideObjects.Remove(col.transform.root.gameObject);
	}

	void OnTriggerStay2D(Collider2D col){
		if(col.tag != "Button"){
			if(col.tag == "Player"){
				playerInside = true;
			}
			if(lit){
				col.rigidbody2D.isKinematic = false;
			}
			else{
				col.rigidbody2D.isKinematic = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if(col.tag == "Player"){
			playerInside = false;
		}
		disableCollision(col.transform.gameObject, false);
		insideObjects.Remove(col.transform.root.gameObject);
	}
}
