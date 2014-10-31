using UnityEngine;
using System.Collections;

public class DoorButtonScript : ButtonsScript {

	override protected void onButtonPress(){
		foreach(GameObject target in buttonTarget){
			target.SetActive(false);
		}
	}
	override protected void onButtonRelease(){
		foreach(GameObject target in buttonTarget){
			target.SetActive(true);
		}
	}
}
