using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSphere : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool hasBeenCollected = false;
    
    void Awake()
    {
        //store original position
        originalPosition = transform.position;
    }
    
    void OnEnable()
    {
        //reset collected flag when sphere is reactivated
        hasBeenCollected = false;
    }
    
    private void OnTriggerEnter(Collider other){
        PPlayerInventory playerInventory = other.GetComponent<PPlayerInventory>();
    
        if(playerInventory != null && !hasBeenCollected){
            //pass this sphere and its position to inventory
            playerInventory.SphereCollected(this, originalPosition);
            hasBeenCollected = true;
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Respawn this sphere at its original position
    /// </summary>
    public void Respawn()
    {
        transform.position = originalPosition;
        hasBeenCollected = false;
        gameObject.SetActive(true);
    }
}
