using UnityEngine;

public class FirstPersonFlashlight : MonoBehaviour {
    [Header("Flashlight Position (relative to camera)")]
    public Vector3 localPosition = new Vector3(0.4f, -0.3f, 0.7f);
    public Vector3 localEuler = new Vector3(0f, 0f, 0f);

    [Header("Flashlight Light Settings")]
    public float range = 20f;
    public float intensity = 6f;
    public float spotAngle = 35f;
    public Color color = Color.white;

    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.F;

    private Transform flashlightModel;
    private Light flashlightLight;

    void Start() {

        GameObject holder = new GameObject("FlashlightHolder");
        flashlightModel = holder.transform;

        flashlightModel.SetParent(transform); 
        flashlightModel.localPosition = localPosition;
        flashlightModel.localEulerAngles = localEuler;


        flashlightLight = holder.AddComponent<Light>();
        flashlightLight.type = LightType.Spot;
        flashlightLight.range = range;
        flashlightLight.intensity = intensity;
        flashlightLight.spotAngle = spotAngle;
        flashlightLight.color = color;
        flashlightLight.shadows = LightShadows.Soft;


        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "FlashlightBody";
        body.transform.SetParent(flashlightModel);
        body.transform.localPosition = new Vector3(0, -0.15f, 0);
        body.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        Destroy(body.GetComponent<Collider>()); // remove collider
    }

    void Update() {

        if (Input.GetKeyDown(toggleKey))
            flashlightLight.enabled = !flashlightLight.enabled;
    }
}
