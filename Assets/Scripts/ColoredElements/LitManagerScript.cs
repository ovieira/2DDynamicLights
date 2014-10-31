using UnityEngine;
using System.Collections;

public abstract class LitManagerScript : MonoBehaviour {

	protected bool lit = false;

	abstract protected void isLit();
	abstract protected void isUnlit();

	void Start(){
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
	}

	void LateUpdate(){
		if(!lit){
			isUnlit();
		}
	}

	void notLit(){
		lit = false;
	}
	
	void rayHit(PointLightScript.LightColor color){
		CancelInvoke("notLit");
		lit = true;
		isLit();
		Invoke("notLit", Time.deltaTime);
	}
}
