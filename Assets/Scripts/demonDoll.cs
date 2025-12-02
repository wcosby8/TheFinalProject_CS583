using UnityEngine;

public class demonDoll : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform playerTransform;
    void Start(){
        GameObject player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
    }

    

    // Update is called once per frame
    void Update(){
        
    }
}
