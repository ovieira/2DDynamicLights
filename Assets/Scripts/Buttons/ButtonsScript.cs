using UnityEngine;
using System.Collections;

public abstract class ButtonsScript : MonoBehaviour {

	public GameObject[] buttonTarget;
	public int pressed = 0;

	abstract protected void onButtonPress();
	abstract protected void onButtonRelease();

	void OnTriggerEnter2D(Collider2D col){
		pressed ++;
		onButtonPress();
	}
	void OnTriggerExit2D(Collider2D col){
		pressed--;
		if(pressed == 0){
			onButtonRelease();
		}
	}
}
