using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private Color initialColor;
    private SpriteRenderer spriteRenderer;
    private float elapsedTime;

    public void SetLifeTime(float value) {
        this.lifetime = value;
    }

    private void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer)) {
            spriteRenderer = renderer;
            initialColor = renderer.color;
        }
        Destroy(gameObject, lifetime); 
    }

    private void Update() {
        // In modo che, per ii gameobject con spriteRenderer, la distruzione avvenga con lerp del colore
        // accade con dashEffect in playercontroller.cs!
        if(spriteRenderer != null) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lifetime;

            spriteRenderer.color = Color.Lerp(initialColor, new Color(spriteRenderer.color.r, spriteRenderer.color.g,
                spriteRenderer.color.b, 0f), t);
        }
    }
}
