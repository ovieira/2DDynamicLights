using UnityEngine;
using System.Collections;

public class PresserScript : LitManagerScript {
	public ArrayList buttonsPressed = new ArrayList();

	void inside(){
		transform.root.GetComponent<Rigidbody2D>().isKinematic = true;
	}

	void notInside(){
		transform.root.GetComponent<Rigidbody2D>().isKinematic = false;
	}

	override protected void isLit(){
		foreach(GameObject button in buttonsPressed){
			Physics2D.IgnoreCollision(transform.root.GetComponent<Collider2D>(), button.GetComponent<Collider2D>(), true);
		}
	}
	override protected void isUnlit(){
		foreach(GameObject button in buttonsPressed){
			Physics2D.IgnoreCollision(transform.root.GetComponent<Collider2D>(), button.GetComponent<Collider2D>(), false);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.tag == "Button"){
			if(!buttonsPressed.Contains(col.gameObject)) buttonsPressed.Add(col.gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if(col.tag == "Button" && !lit){
			buttonsPressed.Remove(col.gameObject);
		}
	}
}
