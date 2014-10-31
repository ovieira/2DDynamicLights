using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialChangerScript : LitManagerScript{
	    
	override protected void isLit(){
		Material new_mat = Resources.Load<Material>("TranslucentColorMaterial");
		GetComponentInParent<SpriteRenderer>().material = new_mat;
	}

	override protected void isUnlit(){
		Material new_mat = Resources.Load<Material>("SolidColorMaterial");
		GetComponentInParent<SpriteRenderer>().material = new_mat;
	}
}

