using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
    

    if(playerInventory != null){
        playerInventory.SphereCollected();
        gameObject.SetActive(false);
    }
    }

}
