using UnityEngine;

public class CustomCursorFollowMouse : MonoBehaviour {

    [SerializeField] private GameObject cursorVisual;
    private GameObject cursorSpawned;
    [SerializeField] private Vector2 offset;

    private Camera mainCamera;

    void Start() {
        Cursor.visible = false;
        mainCamera = Camera.main;

        if(cursorVisual != null)
            cursorSpawned = Instantiate(cursorVisual, Vector2.zero, Quaternion.identity);
    }

    void Update() {
        if (cursorVisual == null) return;

        Vector2 mousePos = Input.mousePosition;
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        cursorSpawned.transform.position = worldPos + offset;
    }

    private void OnDisable() {
        Cursor.visible = true;
    }
}