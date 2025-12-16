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
using System.Collections.Generic;
using System.Linq;

public class demonDoll : MonoBehaviour {
    [Header("Target")]
    public Transform playerTransform;   // You can assign this in the Inspector

    [Header("Movement")]
    public float rotationSpeed = 10f;   // How fast the enemy turns to face the player
    public float normalSpeed = 3.5f;    // Normal movement speed
    public float slowedSpeed = 1f;      // Speed when near light (but not in it)
    
    [Header("Light Avoidance")]
    [Tooltip("Outer radius: inside here the doll is slowed, but still tries to chase the player.")]
    public float lightDetectionRadius = 20f;   // Outer radius (slow zone)
    [Tooltip("Inner radius: inside here the doll panics and retreats back to the detection radius.")]
    public float lightFearRadius = 10f;        // Inner radius (panic/retreat zone)
    public float retreatSpeed = 5f;            // Speed when retreating from light
    [Tooltip("Buffer distance beyond detection radius to retreat to (prevents cycling).")]
    public float retreatBuffer = 3f;           // How far beyond detection radius to retreat
    
    private NavMeshAgent agent;
    private List<LightSource> nearbyLights = new List<LightSource>();
    private LightSource closestLight = null;
    private bool isInFearRadius = false;       // Inside inner panic radius
    private bool isInDetectionRadius = false;  // Inside outer slow radius (but outside fear radius)
    private bool isRetreating = false;         // Currently retreating from light
    private Vector3 retreatTargetPosition = Vector3.zero;  // Where we're retreating to

    void Awake() {
        // Get the NavMeshAgent on this enemy
        agent = GetComponent<NavMeshAgent>();

        // If no player is plugged into the Inspector, try finding them by tag
        if (playerTransform == null) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // We'll rotate manually so the agent doesn't auto-rotate
        if (agent != null) {
            agent.updateRotation = false;
            agent.speed = normalSpeed;
        }
    }

    void Update() {
        if (agent == null || playerTransform == null)
            return;

        // --- 1. Detect nearby light sources ---
        DetectNearbyLights();

        // --- 2. Check if we're in light detection / fear radii ---
        CheckLightStatus();

        // --- 3. Handle movement based on light status ---
        if (isInFearRadius) {
            // Retreat from light beyond the detection radius
            RetreatFromLight();
        }
        else if (isRetreating) {
            // Still retreating - continue until we reach the retreat target
            ContinueRetreat();
        }
        else if (isInDetectionRadius) {
            // Slow down but still try to chase player
            SlowChasePlayer();
        }
        else {
            // Normal behavior: chase the player
            ChasePlayer();
        }

        // --- 4. Rotation: face the movement direction ---
        UpdateRotation();
    }

    /// <summary>
    /// Find all nearby light sources
    /// </summary>
    void DetectNearbyLights()
    {
        nearbyLights.Clear();
        closestLight = null;
        float closestDistance = float.MaxValue;

        // Find all LightSource objects in the scene
        LightSource[] allLights = FindObjectsOfType<LightSource>();
        
        foreach (LightSource light in allLights)
        {
            if (!light.IsLit) continue;  // Skip unlit lights
            
            float distance = Vector3.Distance(transform.position, light.transform.position);
            
            // Only consider lights within detection radius
            if (distance <= lightDetectionRadius)
            {
                nearbyLights.Add(light);
                
                // Track closest light
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLight = light;
                }
            }
        }
    }

    /// <summary>
    /// Check if demon doll is currently inside detection / fear radii
    /// </summary>
    void CheckLightStatus()
    {
        isInFearRadius = false;
        isInDetectionRadius = false;

        foreach (LightSource light in nearbyLights)
        {
            float distanceToLight = Vector3.Distance(transform.position, light.transform.position);

            // Inside inner fear radius: panic/retreat
            if (distanceToLight <= lightFearRadius)
            {
                isInFearRadius = true;
                isInDetectionRadius = true; // also inside detection by definition
                return;
            }

            // Inside outer detection radius (but outside fear radius): slow down
            if (distanceToLight <= lightDetectionRadius)
            {
                isInDetectionRadius = true;
            }
        }
    }

    /// <summary>
    /// Retreat from light when inside fear radius
    /// </summary>
    void RetreatFromLight()
    {
        if (closestLight == null) return;

        // Calculate direction away from the closest light
        Vector3 awayFromLight = transform.position - closestLight.transform.position;
        awayFromLight.y = 0f;
        awayFromLight.Normalize();

        // Target position: BEYOND the detection radius (with buffer) to prevent cycling
        float retreatDistance = lightDetectionRadius + retreatBuffer;
        Vector3 retreatTarget = closestLight.transform.position + awayFromLight * retreatDistance;
        
        // Use NavMesh to find a valid retreat position
        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatTarget, out hit, 10f, NavMesh.AllAreas))
        {
            retreatTargetPosition = hit.position;
            agent.SetDestination(hit.position);
            agent.speed = retreatSpeed;
            isRetreating = true;
        }
        else
        {
            // If we can't find a valid NavMesh position, try a closer position
            retreatTarget = closestLight.transform.position + awayFromLight * (lightDetectionRadius + 1f);
            if (NavMesh.SamplePosition(retreatTarget, out hit, 10f, NavMesh.AllAreas))
            {
                retreatTargetPosition = hit.position;
                agent.SetDestination(hit.position);
                agent.speed = retreatSpeed;
                isRetreating = true;
            }
            else
            {
                // If we still can't find a valid position, just stop
                agent.SetDestination(transform.position);
                agent.speed = 0f;
                isRetreating = false;
            }
        }
    }

    /// <summary>
    /// Continue retreating until we reach the retreat target
    /// </summary>
    void ContinueRetreat()
    {
        // Check if we've reached the retreat target (or close enough)
        float distanceToTarget = Vector3.Distance(transform.position, retreatTargetPosition);
        
        if (distanceToTarget < 2f)
        {
            // We've reached the retreat target, stop retreating
            isRetreating = false;
            // Now check status again - should be outside detection radius
            CheckLightStatus();
            if (!isInDetectionRadius)
            {
                ChasePlayer();
            }
            else
            {
                SlowChasePlayer();
            }
        }
        else
        {
            // Still retreating - continue to target
            agent.SetDestination(retreatTargetPosition);
            agent.speed = retreatSpeed;
        }
    }

    /// <summary>
    /// Slow chase behavior inside detection radius but outside fear radius
    /// </summary>
    void SlowChasePlayer()
    {
        agent.speed = slowedSpeed;
        agent.SetDestination(playerTransform.position);
    }

    /// <summary>
    /// Normal behavior: chase the player
    /// </summary>
    void ChasePlayer()
    {
        agent.speed = normalSpeed;
        agent.SetDestination(playerTransform.position);
    }

    /// <summary>
    /// Update rotation to face movement direction
    /// </summary>
    void UpdateRotation()
    {
        Vector3 direction;
        
        if (isInFearRadius && closestLight != null)
        {
            // Face away from light when retreating
            direction = transform.position - closestLight.transform.position;
        }
        else
        {
            // Face movement direction
            direction = agent.velocity;
        }
        
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
