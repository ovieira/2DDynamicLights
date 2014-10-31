using UnityEngine;
using System.Collections;

public class SpriteIllumination : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;
    public float InitialIllumination;
    public int current_intensity;
    //public Color color;
	// Use this for initialization
	void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, InitialIllumination);

        //_spriteRenderer.material.SetColor("_Color", color);
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnCollisionEnter2D(Collision2D col) {
        print(col.collider.name);
    }

    public void SetIllumination(int intensity) {
        current_intensity = intensity;
        float new_alpha = intensity / 255f;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, new_alpha);

        //_spriteRenderer.color.a
    }

}
