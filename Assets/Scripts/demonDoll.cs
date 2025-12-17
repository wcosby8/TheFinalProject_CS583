//using UnityEngine;

//public class demonDoll : MonoBehaviour{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    private Transform playerTransform;
//    void Start(){
//        GameObject player = GameObject.Find("Player");
//        playerTransform = player.GetComponent<Transform>();
//    }



//    // Update is called once per frame
//    void Update(){

//    }
//}


using UnityEngine;
using UnityEngine.AI;

public class demonDoll : MonoBehaviour {
    [Header("Target")]
    public Transform playerTransform;   // You can assign this in the Inspector

    [Header("Movement")]
    public float rotationSpeed = 10f;   // How fast the enemy turns to face the player

    private NavMeshAgent agent;

    void Awake() {
        // Get the NavMeshAgent on this enemy
        agent = GetComponent<NavMeshAgent>();

        // If no player is plugged into the Inspector, try finding them by tag
        if (playerTransform == null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // We’ll rotate manually so the agent doesn’t auto-rotate
        if (agent != null) {
            agent.updateRotation = false;
        }
    }

    void Update() {
        if (agent == null || playerTransform == null)
            return;

        // --- 1. Pathfinding: move towards the player ---
        agent.SetDestination(playerTransform.position);

        // --- 2. Rotation: face the player ---
        // Direction from enemy to player (ignore vertical difference)
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
