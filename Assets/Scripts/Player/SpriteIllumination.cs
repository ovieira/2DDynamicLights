using UnityEngine;
using System.Collections;

public class SpriteIllumination : MonoBehaviour {


    public class LightSourceInfo {
        public Color lightcolor { get; set; }
        public Color alpha { get; set; }

    }

    private SpriteRenderer _spriteRenderer;
    private Color OriginalColor;
    public float InitialIllumination;
    public int current_intensity;
    private Color newColor;
    public float LerpSpeed = 5;
    private bool canReset;
    //public Color color;
    // Use this for initialization
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        OriginalColor = _spriteRenderer.color;

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, InitialIllumination);
        newColor = _spriteRenderer.color;
        //_spriteRenderer.material.SetColor("_Color", color);
    }

    // Update is called once per frame
    void Update() {
     _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, newColor, Time.deltaTime*LerpSpeed);
    }

    void OnCollisionEnter2D(Collision2D col) {
        print(col.collider.name);
    }

    public void SetIllumination(LightSourceInfo info) {
        //current_intensity = intensity;

        //float new_alpha = intensity / 255f;
        CancelInvoke("ResetNewColor");
        newColor = OriginalColor * info.lightcolor;
        newColor.a = map(info.alpha.a, 0, info.lightcolor.a, 0, 1);
        Invoke("ResetNewColor", .3f);
        
        //_spriteRenderer.color = newColor;
        //Color.Lerp(_spriteRenderer.color, newColor, Time.deltaTime);
        //_spriteRenderer.color.a
    }


    void ResetNewColor() {
            newColor.a = 0;            
    }

    float map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
