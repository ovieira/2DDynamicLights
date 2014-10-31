using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointLightScript : MonoBehaviour
{
/**************************VARIABLEs*********************************/
	Quaternion fixedRotation;
	GameObject target;
	List<LineRenderer> lightRays;
	public int rays_no;
	public float lightHeight;
	public float lightDist;
	public enum LightColor
	{
		NONE,
		RED,
		GREEN
	}

    public Color color0, color1;

	public Material line_mat;
	[SerializeField]
	protected LightColor
		currentColor;
	public float alph = 1f;

	public float lineWidth;
    private bool canIlluminate = true;

/***********************************************************/

/**************************SETUP*********************************/

	// Use this for initialization
	void Awake()
	{
		fixedRotation = transform.localRotation;
		Debug.Log(fixedRotation);
	}
    
	void Start()
	{
		lightRays = new List<LineRenderer>();
		addLineRenderers();
		setColor();
		target = GameObject.FindGameObjectWithTag("Player");
	}
	/*creates all the light renderers and sets them as children of the player (any parameter like tag, layer, etc, should be added here)*/
	private void addLineRenderers()
	{
		for (int i = 0; i < rays_no; i++)
		{
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
	void setIgnoredLayers()
	{
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
	void setLineColor()
	{
        
		for (int i = 0; i < lightRays.Count; i++)
		{


            lightRays[i].SetColors(color0, color1);    
		}
	}

	//Always call this function when the color is changed
	void setColor()
	{
		setIgnoredLayers();
		setLineColor();
	}
/***********************************************************/

	// Update is called once per frame
	void Update()
	{
		transform.position = target.transform.position + Vector3.up * lightHeight;
		StartCoroutine("Illuminate");

        //if (Input.GetKeyDown("e"))
        //{
        //    if (hasGreen)
        //    {
        //        currentColor = LightColor.GREEN;
        //        setColor();     
        //    }
        //}
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

	void OnTriggerEnter2D(Collider2D col)
	{
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

	void OnTriggerExit2D(Collider2D col)
	{
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

            ArrayList hitObjs = new ArrayList();
            for (int i = 0; i < lightRays.Count; i++) {
                LineRenderer lightRay = lightRays[i];
                lightRay.SetVertexCount(2);
                Vector3 dest = RotatePointAroundPivot(firstRay, Vector3.zero, (360.0f / rays_no) * i);
                RaycastHit2D hit;
                lightRay.SetPosition(0, transform.position);
                lightRay.SetPosition(1, transform.position + lightDist * dest);
                lightRay.SetColors(color0, color1);

                LayerMask l = defineLayerMask();

                //hitVec = Physics2D.RaycastAll(transform.position, dest, lightDist, l);
                hit = Physics2D.Raycast(transform.position, dest, lightDist, l);

                if (hit.collider != null) {
                    lightRay.SetPosition(1, hit.point);
                    //print(hit.collider.gameObject.name);
                    lightRay.SetColors(color0, calculateAlpha(hit.point));

                }
                else {
                    //lightRay.SetColors(color0, color1);
                    //lightRay.SetColors(getCurrentColor(), calculateAlpha(hit.point));
                    lightRay.SetPosition(1, transform.position + lightDist * dest);
                }
            }
        }
        //lightRay.enabled = false;
        yield return null;
    }

	/*function used to define with which layers should the line renderer rays collide*/
	LayerMask defineLayerMask()
	{
		return ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("IgnoreLight")));
		/*switch (currentColor) {
		case LightColor.RED:
			return (1 << LayerMask.NameToLayer ("Wall")) | (1 << LayerMask.NameToLayer ("GreenElement"));
			break;
		case LightColor.GREEN:
			return (1 << LayerMask.NameToLayer ("Wall")) | (1 << LayerMask.NameToLayer ("RedElement"));
			break;
		default:
			return (1 << LayerMask.NameToLayer ("Wall"));
			break;
		}*/
	}
    
	/*no idea*/
	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		dir = q * dir; // rotate it
		return dir; // return it
	}

	public LightColor CurrentColor
	{
		get
		{
			return currentColor;
		}
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

	Color calculateAlpha(Vector2 point)
	{
		float d = Vector2.Distance(transform.position, point);
		float alph = color0.a - map(d, 0, lightDist, 0, color0.a);

        //switch (currentColor)
        //{wd
        //    case LightColor.RED:
        //        return new Color(1f, 0f, 0f, alph);
        //    case LightColor.GREEN:
        //        return new Color(0f, 1.0f, 0f, alph);
        //    default:
        //        return Color.black;
        //}
        Color c = color1;
        c.a = alph;
        return c;
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}
}
