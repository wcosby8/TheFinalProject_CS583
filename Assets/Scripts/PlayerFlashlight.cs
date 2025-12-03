
using UnityEngine;

public class FirstPersonFlashlight : MonoBehaviour {
    [Header("Flashlight Position (relative to CAMERA)")]
    public Vector3 localPosition = new Vector3(0.4f, -0.3f, 0.7f);
    public Vector3 localEuler = new Vector3(0f, 0f, 0f);

    [Header("Flashlight Light Settings")]
    public float range = 20f;
    public float intensity = 6f;
    public float spotAngle = 35f;
    public Color color = Color.white;

    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.F;

    // which camera we want to follow (pitch + yaw)
    public Transform cameraTransform;    // assign in Inspector or auto-find

    private Transform flashlightModel;
    private Light flashlightLight;

    void Start() {
        // find the main camera if not assigned
        if (cameraTransform == null) {
            Camera cam = Camera.main;
            if (cam != null)
                cameraTransform = cam.transform;
        }

        if (cameraTransform == null) {
            Debug.LogError("FirstPersonFlashlight: No cameraTransform assigned and no MainCamera found.");
            enabled = false;
            return;
        }

        // create holder and keep it as a child of THIS object (PlayerCapsule)
        GameObject holder = new GameObject("FlashlightHolder");
        flashlightModel = holder.transform;
        flashlightModel.SetParent(transform); // stays under PlayerCapsule in hierarchy

        // light
        flashlightLight = holder.AddComponent<Light>();
        flashlightLight.enabled = true;

        flashlightLight.type = LightType.Spot;
        flashlightLight.range = range;
        flashlightLight.intensity = intensity;
        flashlightLight.spotAngle = spotAngle;
        flashlightLight.color = color;
        flashlightLight.shadows = LightShadows.Soft;

        // visible body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "FlashlightBody";
        body.transform.SetParent(flashlightModel);
        body.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        body.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        Destroy(body.GetComponent<Collider>());
    }

    void Update() {
        if (flashlightLight == null || cameraTransform == null) return;

        // position & rotation FOLLOW THE CAMERA each frame
        // but offset in camera local space so it sits in front/right in view
        flashlightModel.position = cameraTransform.TransformPoint(localPosition);
        flashlightModel.rotation = cameraTransform.rotation * Quaternion.Euler(localEuler);

        // toggle on/off
        if (Input.GetKeyDown(toggleKey)) {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }
}
