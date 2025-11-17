using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    private float pulseSpeed = 1f;
    private Vector3 minScale = Vector3.one;
    private Vector3 maxScale = Vector3.one * 2f;

    private void Update() {
        PulseAnim();
    }

    public void SetUpParamaeters(float pulseSpeed, Vector3 minScale, Vector3 maxScale) {
        this.pulseSpeed = pulseSpeed;
        this.minScale = minScale;
        this.maxScale = maxScale;
    }

    public void PulseAnim() {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        this.transform.localScale = Vector3.Lerp(minScale, maxScale, t);
    }
}
