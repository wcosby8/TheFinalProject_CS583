using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        PPlayerInventory playerInventory = other.GetComponent<PPlayerInventory>();
    

    if(playerInventory != null){
        playerInventory.SphereCollected();
        gameObject.SetActive(false);
    }
    }

}
