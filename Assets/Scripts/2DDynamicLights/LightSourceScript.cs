using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LightSourceScript : MonoBehaviour {
    /**************************VARIABLEs*********************************/


    public enum LightEffects {
        Normal,
        //Wobble,
        Sonar
    }

    public LightEffects LightEffect;
    Quaternion fixedRotation;
    //GameObject target;
    List<LineRenderer> lightRays;
    public int NumberOfLightRays;
    public float LightRayMagnitude;
    private float lightdist;

    public Color LightColor;
    private Color AlphaToZero = new Color(1,1,1,0);


    public float alph = 1f;

    public float lineWidth;
    private bool canIlluminate = true;
    public bool collideLight;
    public int LightConeAngle;


    public bool LightWobble;
    public float LightWobbleRange;
    private float random;
    private float LightFlickrAux;


    public float SonarPulseInterval;
    private float _timeOfLastPulse;
    public float PulseThickness;
    /***********************************************************/

    /**************************SETUP*********************************/

    // Use this for initialization
    void Awake() {
        fixedRotation = transform.localRotation;
        Debug.Log(fixedRotation);
    }

    void Start() {
        lightdist = LightRayMagnitude;
        collideLight = true;
        lightRays = new List<LineRenderer>();
        addLineRenderers();
        setColor();
        if (LightWobble) {
            random = Random.Range(0f, 100f);
            LightFlickrAux = LightRayMagnitude;
            LightWobbleRange = 1.5f;
        }
        _timeOfLastPulse = Time.time;
    }


    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("c")) {
            collideLight ^= true;
        }


        switch (LightEffect) {
            case LightEffects.Normal:
                //StartCoroutine("Illuminate");
                Illuminate2();
                break;
            case LightEffects.Sonar:
                SonarPulse();
                break;
            default:
                break;
        }            

        if (LightWobble && LightEffect == LightEffects.Normal) {
            float noise = Mathf.PerlinNoise(random, Time.time);
            lightdist = Mathf.Lerp(LightRayMagnitude - LightWobbleRange, LightRayMagnitude + LightWobbleRange, noise);
        }
        else {
            lightdist = LightRayMagnitude;
        }
    }

   

    /*creates all the light renderers and sets them as children of the player (any parameter like tag, layer, etc, should be added here)*/
    private void addLineRenderers() {
        for (int i = 0; i < NumberOfLightRays; i++) {
            GameObject lineOBJ = new GameObject("lightRay " + i);
            LineRenderer l = lineOBJ.AddComponent<LineRenderer>();
            Material new_mat = Resources.Load<Material>("TranslucentColorMaterial");
            //Material new_mat = new Material(Shader.Find("Transparent/Diffuse"));
            l.renderer.material = new_mat;
            //l.SetColors(Color.red, Color.red);
            l.SetWidth(lineWidth, lineWidth);
            l.transform.parent = transform;
            lightRays.Add(l);
        }
    }
  
    /*changes the visual color of the line renderers*/
    void setLineColor() {
        for (int i = 0; i < lightRays.Count; i++)
            lightRays[i].SetColors(LightColor, LightColor * AlphaToZero);
    }

    //Always call this function when the color is changed
    void setColor() {
        setLineColor();
    }
    /***********************************************************/

    

    /*has all functionality regarding the line renderers interaction*/
    IEnumerator Illuminate() {
        if (!canIlluminate) {
            yield return null;
        }
        else {
            Vector3 firstRay = transform.up;
            Dictionary<GameObject, Color> sprites_dictionary = new Dictionary<GameObject, Color>();
            for (int i = 0; i < lightRays.Count; i++) {

                //Fase1: Set Up standart light ray distance and color

                LineRenderer lightRay = lightRays[i];
                lightRay.SetVertexCount(2);
                Vector3 dest = RotatePointAroundPivot(firstRay, Vector3.zero, ((float)LightConeAngle / NumberOfLightRays) * i);
                lightRay.SetPosition(0, transform.position);
                lightRay.SetPosition(1, transform.position + lightdist * dest);
                lightRay.SetColors(LightColor, LightColor*AlphaToZero);

                LayerMask l = defineLayerMask();


                //Fase 2: Check light colisions and calculate each new light ray distance

                if (collideLight) {
                    RecalculateLightRays(lightRay, dest, l, sprites_dictionary);
                }
            }

            if (collideLight) {
                spritesIlluminate(sprites_dictionary);
            }

        }
        yield return null;
    }

    private void Illuminate2() {
        if (!canIlluminate) {
            return;
        }
        else {
            Vector3 firstRay = transform.up;
            Dictionary<GameObject, Color> sprites_dictionary = new Dictionary<GameObject, Color>();
            for (int i = 0; i < lightRays.Count; i++) {

                //Fase1: Set Up standart light ray distance and color

                LineRenderer lightRay = lightRays[i];
                lightRay.SetVertexCount(2);
                Vector3 dest = RotatePointAroundPivot(firstRay, Vector3.zero, ((float)LightConeAngle / NumberOfLightRays) * i);
                lightRay.SetPosition(0, transform.position);
                lightRay.SetPosition(1, transform.position + lightdist * dest);
                lightRay.SetColors(LightColor, LightColor * AlphaToZero);

                LayerMask l = defineLayerMask();


                //Fase 2: Check light colisions and calculate each new light ray distance

                if (collideLight) {
                    RecalculateLightRays(lightRay, dest, l, sprites_dictionary);
                }
            }

            if (collideLight) {
                spritesIlluminate(sprites_dictionary);
            }

        }
        return;
    }

    private void spritesIlluminate(Dictionary<GameObject, Color> sprites_dictionary) {
        foreach (KeyValuePair<GameObject, Color> entry in sprites_dictionary) {
            if (entry.Key.GetComponent<SpriteIlluminationScript>() != null) {
                SpriteIlluminationScript.LightSourceInfo info = new SpriteIlluminationScript.LightSourceInfo();
                info.lightcolor = LightColor;
                info.alpha = entry.Value;
                entry.Key.SendMessage("SetIllumination", info);
            }
        }
    }

    private void RecalculateLightRays(LineRenderer lightRay, Vector3 dest, LayerMask l, Dictionary<GameObject, Color> dictionary) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dest, lightdist, l);

        if (hit.collider != null) {
            lightRay.SetPosition(1, hit.point);
            //print(hit.collider.gameObject.name);
            Color alpha = calculateAlpha(hit.point);
            lightRay.SetColors(LightColor, alpha);
            AddToHash(dictionary, hit.collider.gameObject, alpha);
        }
        else {
            
            lightRay.SetPosition(1, transform.position + lightdist * dest);
        }
    }

    private void AddToHash(Dictionary<GameObject, Color> dictionary, GameObject gameObject, Color c) {
        if (!dictionary.ContainsKey(gameObject))
            dictionary.Add(gameObject, c);
    }

    /*function used to define with which layers should the line renderer rays collide*/
    LayerMask defineLayerMask() {
        return ~(1 << LayerMask.NameToLayer("LightSource"));
    }

   

    Color calculateAlpha(Vector2 point) {
        float d = Vector2.Distance(transform.position, point);
        float alph = LightColor.a - map(d, 0, lightdist, 0, LightColor.a);
        Color c = LightColor*AlphaToZero;
        c.a = alph;
        return c;
    }


    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        dir = q * dir; // rotate it
        return dir; // return it
    }

    float map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    //+++++++++++++++++++++++SONAR CODE++++++++++++++++++++++++++
    private void SonarPulse() {
        float currentTime = Time.time;

        if (currentTime - _timeOfLastPulse >= SonarPulseInterval) {
            StartCoroutine("SonarPulseCoRoutine");
        }
    }

    IEnumerator SonarPulseCoRoutine() {

        Debug.Log("Pulse!");
        _timeOfLastPulse = Time.time;


        List<LineRenderer> copy = new List<LineRenderer>(lightRays);
        //float _distance = 0;

        //Vector3 firstRay = transform.up;
        //Dictionary<GameObject, Color> sprites_dictionary = new Dictionary<GameObject, Color>();
        //for (int i = 0; i < copy.Count; i++) {

        //    //Fase1: Set Up standart light ray distance and color

        //    LineRenderer lightRay = copy[i];
        //    lightRay.SetVertexCount(2);
        //    Vector3 dest = RotatePointAroundPivot(firstRay, Vector3.zero, ((float)LightConeAngle / NumberOfLightRays) * i);
        //    lightRay.SetPosition(0, transform.position);
        //    lightRay.SetPosition(1, transform.position + PulseThickness * dest);
        //    lightRay.SetColors(LightColor, LightColor * AlphaToZero);

        //    LayerMask l = defineLayerMask();


        //    //Fase 2: Check light colisions and calculate each new light ray distance

        //    //if (collideLight) {
        //    //    RecalculateLightRays(lightRay, dest, l, sprites_dictionary);
        //    //}
        //}

        //while (_distance <= LightRayMagnitude) {
            
        //}

        yield return null;
    }
}
