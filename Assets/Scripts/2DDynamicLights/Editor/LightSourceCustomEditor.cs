using UnityEngine;
using UnityEditor; 
using System.Collections;

[CustomEditor (typeof(LightSourceScript))]
public class LightSourceCustomEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LightSourceScript myLightSourceScript = (LightSourceScript) target;

        if (myLightSourceScript.LightFlickr) {
            myLightSourceScript.LightFlickrOffSetRange = EditorGUILayout.FloatField("Range", myLightSourceScript.LightFlickrOffSetRange);
        }

    }

}
