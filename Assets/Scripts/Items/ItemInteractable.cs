using UnityEngine;

public interface IInteractable { 
    public void OnInteract();
}

public class ItemInteractable : MonoBehaviour, IInteractable
{
    // per ora non devo accederci da inspector, quindi lascio tutto privato ed accessibile solo da ItemSpawner.cs
    private ItemData item; 
    private GameObject effect;
    private Vector3 effectStartScale;
    private Vector3 effectMinScale;
    private float pulseSpeed = 1f;

    public void SetItemData(ItemData item) { // Richiamato in ItemSpanwer
        this.item = item;

        SetUpScriptableObject();

        // set up dell'effect (puramente visivo)
        GameObject newEffect = new GameObject("item effect");
        newEffect.AddComponent<SpriteRenderer>();
        newEffect.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        newEffect.GetComponent<SpriteRenderer>().color = new Color32(item.itemColor.r, item.itemColor.g, item.itemColor.b, 10);

        effect = Instantiate(newEffect, transform.position, Quaternion.identity);
        Destroy(newEffect); // Visto che lo spawna anche senza instantiate
        effect.transform.SetParent(this.transform);
        effect.AddComponent<PulseAnimation>(); // Per effetto pulse

        effectStartScale = effect.transform.localScale * 3;
        effect.transform.localScale = effectStartScale;
        effectMinScale = effectStartScale / 2;
        // Imposto parametri per pulse -> aggiornato in update
        effect.GetComponent<PulseAnimation>().SetUpParamaeters(pulseSpeed, effectMinScale, effectStartScale);
    }

    private void SetUpScriptableObject() { // viene richiamato in ItemSpawner!
        if (this.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer)) {
            renderer.sprite = item.itemSprite; // Imposto sprite
            renderer.color = item.itemColor; // Imposto colore
        }

        this.gameObject.name = item.name;
    }

    public void OnInteract() {
        item.ItemEffect(); // A seguito dell'interazione eseguo l'effetto dell'item

        
        // InGameUIEvents.Instance.ShowEffectText(item.name); // Spostato nei singoli per far vedere
        // singolo valore -> in caso mettere il valore direttamente in ItemData

        if(item is not (SpawnShield_Item or RestoreHealth_Item)) // Se l'item non e' di tipo shield o cura, allora spawno l'icona
            InGameUIEvents.Instance.AddEffectIcon(item.itemSprite, item.itemColor, item.effectDuration);

        // Some effect
        Destroy(this.gameObject); // Distruggo l'item
    }
}
