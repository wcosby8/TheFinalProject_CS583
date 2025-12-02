using UnityEngine;

public class CameraLook : MonoBehaviour {
    public float sensitivity = 300f;

    public float minPitch = -60f;  
    public float maxPitch = 60f;   

    private float pitch = 0f;      

    public Transform playerBody;   

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;


        playerBody.Rotate(Vector3.up * mouseX);


        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
