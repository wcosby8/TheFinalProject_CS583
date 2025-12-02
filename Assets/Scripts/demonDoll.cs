using UnityEngine;
using UnityEngine.AI;

public class demonDoll : MonoBehaviour {
    [Header("Target")]
    public Transform playerTransform;   

    [Header("Movement")]
    public float rotationSpeed = 10f;   

    private NavMeshAgent agent;

    void Awake() {
        
        agent = GetComponent<NavMeshAgent>();

        if (playerTransform == null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (agent != null) {
            agent.updateRotation = false;
        }
    }

    void Update() {
        if (agent == null || playerTransform == null)
            return;

        agent.SetDestination(playerTransform.position);

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
