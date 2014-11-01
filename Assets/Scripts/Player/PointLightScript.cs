using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointLightScript : MonoBehaviour {
    /**************************VARIABLEs*********************************/
    Quaternion fixedRotation;
    //GameObject target;
    List<LineRenderer> lightRays;
    public int rays_no;
    public float lightHeight;
    public float lightDist;

    public Color LightColor;
    private Color AlphaToZero = new Color(1,1,1,0);

    public Material line_mat;

    public float alph = 1f;

    public float lineWidth;
    private bool canIlluminate = true;
    public bool collideLight;

    /***********************************************************/

    /**************************SETUP*********************************/

    // Use this for initialization
    void Awake() {
        fixedRotation = transform.localRotation;
        Debug.Log(fixedRotation);
    }

    void Start() {
        //color1 = LightColor;
        //color1.a = 0;
        collideLight = true;
        lightRays = new List<LineRenderer>();
        addLineRenderers();
        setColor();
        //target = GameObject.FindGameObjectWithTag("Player");
    }
    /*creates all the light renderers and sets them as children of the player (any parameter like tag, layer, etc, should be added here)*/
    private void addLineRenderers() {
        for (int i = 0; i < rays_no; i++) {
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
    /*sets which collision layers should be ignored, based on the color of the player*/
    void setIgnoredLayers() {
        /*switch (currentColor) {
        case LightColor.GREEN:
            Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("GreenElement"), true);
            Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("RedElement"), false);
            break;
        case LightColor.RED:
            Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("GreenElement"), false);
            Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("RedElement"), true);
            break;
        }*/
    }
    /*changes the visual color of the line renderers*/
    void setLineColor() {

        for (int i = 0; i < lightRays.Count; i++) {


            lightRays[i].SetColors(LightColor, LightColor*AlphaToZero);
        }
    }

    //Always call this function when the color is changed
    void setColor() {
        setIgnoredLayers();
        setLineColor();
    }
    /***********************************************************/

    // Update is called once per frame
    void Update() {
        //transform.position = target.transform.position + Vector3.up * lightHeight;

        if (Input.GetKeyDown("c")) {
            collideLight ^= true;
        }

        StartCoroutine("Illuminate");

        //if (Input.GetKeyDown("q"))
        //{
        //    if (hasRed)
        //    {
        //        currentColor = LightColor.RED;
        //        setColor(); 
        //    }
        //}
    }
    /*check if the player can currently change color (if it isnt inside a colored block)*/
    //bool isInsideObject()
    //{
    //    switch (currentColor)
    //    {
    //        case LightColor.RED:
    //            return redBool;
    //        case LightColor.GREEN:
    //            return greenBool;
    //        default:
    //            return false;
    //    }
    //}

    void OnTriggerEnter2D(Collider2D col) {
        //switch (col.tag)
        //{
        //    case "RedElement":
        //        redBool = true;
        //        break;
        //    case "GreenElement":
        //        greenBool = true;
        //        break;
        //}
    }

    void OnTriggerExit2D(Collider2D col) {
        //switch (col.tag)
        //{
        //    case "RedElement":
        //        redBool = false;
        //        break;
        //    case "GreenElement":
        //        greenBool = false;
        //        break;
        //}
    }

    /*has all functionality regarding the line renderers interaction*/
    IEnumerator Illuminate() {
        //if (currentColor == LightColor.NONE)
        if (!canIlluminate) {
            yield return null;
        }
        else {
            Vector3 firstRay = transform.up;
            Dictionary<GameObject, Color> sprites_dictionary = new Dictionary<GameObject, Color>();
            //ArrayList hitObjs = new ArrayList();
            for (int i = 0; i < lightRays.Count; i++) {

                //Fase1: Set Up standart light ray distance and color

                LineRenderer lightRay = lightRays[i];
                lightRay.SetVertexCount(2);
                Vector3 dest = RotatePointAroundPivot(firstRay, Vector3.zero, (360.0f / rays_no) * i);
                lightRay.SetPosition(0, transform.position);
                lightRay.SetPosition(1, transform.position + lightDist * dest);
                lightRay.SetColors(LightColor, LightColor*AlphaToZero);

                LayerMask l = defineLayerMask();

                //hitVec = Physics2D.RaycastAll(transform.position, dest, lightDist, l);

                //Fase 2: Check light colisions and calculate each new light ray distance

                if (collideLight) {
                    RecalculateLightRays(lightRay, dest, l, sprites_dictionary);
                }
            }

            if (collideLight) {
                spritesIlluminate(sprites_dictionary);
            }

        }
        //lightRay.enabled = false;
        yield return null;
    }

    private void spritesIlluminate(Dictionary<GameObject, Color> sprites_dictionary) {
        foreach (KeyValuePair<GameObject, Color> entry in sprites_dictionary) {
            if (entry.Key.GetComponent<SpriteIllumination>() != null) {
                SpriteIllumination.LightSourceInfo info = new SpriteIllumination.LightSourceInfo();
                info.lightcolor = LightColor;
                info.alpha = entry.Value;
                entry.Key.SendMessage("SetIllumination", info);
            }
        }
    }

    private void RecalculateLightRays(LineRenderer lightRay, Vector3 dest, LayerMask l, Dictionary<GameObject, Color> dictionary) {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dest, lightDist, l);

        if (hit.collider != null) {
            lightRay.SetPosition(1, hit.point);
            //print(hit.collider.gameObject.name);
            Color alpha = calculateAlpha(hit.point);
            lightRay.SetColors(LightColor, alpha);
            AddToHash(dictionary, hit.collider.gameObject, alpha);
        }
        else {
            //lightRay.SetColors(color0, color1);
            //lightRay.SetColors(getCurrentColor(), calculateAlpha(hit.point));
            lightRay.SetPosition(1, transform.position + lightDist * dest);
        }
    }

    private void AddToHash(Dictionary<GameObject, Color> dictionary, GameObject gameObject, Color c) {
        if (!dictionary.ContainsKey(gameObject)) {
            dictionary.Add(gameObject, c);

        }
        else {
        }
    }

    /*function used to define with which layers should the line renderer rays collide*/
    LayerMask defineLayerMask() {
        return ~(1 << LayerMask.NameToLayer("LightSource"));
    }

    /*no idea*/
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        dir = q * dir; // rotate it
        return dir; // return it
    }



    //Color getCurrentColor()
    //{
    //    switch (currentColor)
    //    {
    //        case LightColor.RED:
    //            return Color.red;
    //        case LightColor.GREEN:
    //            return Color.green;
    //        default:
    //            return Color.black;
    //    }
    //}

    //Color getCurrentEndColor()
    //{
    //    switch (currentColor)
    //    {
    //        case LightColor.RED:
    //            return redEnd;
    //        case LightColor.GREEN:
    //            return greenEnd;
    //        default:
    //            return Color.black;
    //    }
    //}

    Color calculateAlpha(Vector2 point) {
        float d = Vector2.Distance(transform.position, point);
        float alph = LightColor.a - map(d, 0, lightDist, 0, LightColor.a);

        //switch (currentColor)
        //{wd
        //    case LightColor.RED:
        //        return new Color(1f, 0f, 0f, alph);
        //    case LightColor.GREEN:
        //        return new Color(0f, 1.0f, 0f, alph);
        //    default:
        //        return Color.black;
        //}
        Color c = LightColor*AlphaToZero;
        c.a = alph;
        return c;
    }

    float map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

}
