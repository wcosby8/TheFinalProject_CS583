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
    [Tooltip("Speed increase per sphere collected")]
    public float speedIncreasePerSphere = 1f;
    [Tooltip("Speed decrease per sphere deposited")]
    public float speedDecreasePerDeposit = 0.5f;
    
    private float baseNormalSpeed;       // Store original speed
    private float baseSlowedSpeed;      // Store original slowed speed
    private float baseRetreatSpeed;     // Store original retreat speed
    private float currentSpeedModifier = 0f; // Track current speed modifier (can be fractional)
    
    [Header("Light Avoidance")]
    [Tooltip("Outer radius: inside here the doll is slowed, but still tries to chase the player.")]
    public float lightDetectionRadius = 20f;   // Outer radius (slow zone)
    [Tooltip("Inner radius: inside here the doll panics and retreats back to the detection radius.")]
    public float lightFearRadius = 10f;        // Inner radius (panic/retreat zone)
    public float retreatSpeed = 5f;            // Speed when retreating from light
    [Tooltip("Buffer distance beyond detection radius to retreat to (prevents cycling).")]
    public float retreatBuffer = 3f;           // How far beyond detection radius to retreat
    
    [Header("Player Contact")]
    [Tooltip("Distance at which demon doll can touch/steal spheres from player")]
    public float contactDistance = 2f;
    [Tooltip("Cooldown between contact attempts (seconds)")]
    public float contactCooldown = 1f;
    
    private NavMeshAgent agent;
    private List<LightSource> nearbyLights = new List<LightSource>();
    private LightSource closestLight = null;
    private bool isInFearRadius = false;       // Inside inner panic radius
    private bool isInDetectionRadius = false;  // Inside outer slow radius (but outside fear radius)
    private bool isRetreating = false;         // Currently retreating from light
    private Vector3 retreatTargetPosition = Vector3.zero;  // Where we're retreating to
    private int speedIncreaseCount = 0;        // Track how many times speed has been increased
    private PPlayerInventory playerInventory; // Reference to player inventory
    private GameManager gameManager;          // Reference to game manager
    private float contactCooldownTimer = 0f;   // Timer for contact cooldown
    private int lastKnownSphereCount = 0;      // Track previous sphere count to detect actual collection

    void Awake() {
        // Get the NavMeshAgent on this enemy
        agent = GetComponent<NavMeshAgent>();

        // If no player is plugged into the Inspector, try finding them by tag
        GameObject player = null;
        if (playerTransform == null) {
            player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
        else
        {
            player = playerTransform.gameObject;
        }

        // We'll rotate manually so the agent doesn't auto-rotate
        if (agent != null) {
            agent.updateRotation = false;
            agent.speed = normalSpeed;
        }
        
        // Store base speeds
        baseNormalSpeed = normalSpeed;
        baseSlowedSpeed = slowedSpeed;
        baseRetreatSpeed = retreatSpeed;
        
        // Subscribe to sphere collection events and store references
        if (player != null)
        {
            playerInventory = player.GetComponent<PPlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.OnSphereCollected.AddListener(OnSphereCollected);
                playerInventory.OnSpheresDeposited.AddListener(OnSphereStolen);
                playerInventory.OnSphereStolen.AddListener(OnSphereStolenForRespawn);
                // Initialize last known count
                lastKnownSphereCount = playerInventory.NumberOfSpheres;
            }
        }
        
        // Find game manager
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (playerInventory != null)
        {
            playerInventory.OnSphereCollected.RemoveListener(OnSphereCollected);
            playerInventory.OnSpheresDeposited.RemoveListener(OnSphereStolen);
            playerInventory.OnSphereStolen.RemoveListener(OnSphereStolenForRespawn);
        }
    }
    
    /// <summary>
    /// Called when player collects a sphere - increases demon doll speed
    /// </summary>
    void OnSphereCollected(PPlayerInventory inventory)
    {
        if (inventory == null) return;
        
        int currentCount = inventory.NumberOfSpheres;
        
        // Only increase speed if sphere count actually increased (not decreased)
        if (currentCount > lastKnownSphereCount)
        {
            speedIncreaseCount++;
            currentSpeedModifier += speedIncreasePerSphere;
            
            // Update all speeds
            UpdateSpeeds();
            
            Debug.Log($"Demon doll speed increased! Normal speed: {normalSpeed}");
        }
        
        // Update last known count
        lastKnownSphereCount = currentCount;
    }
    
    /// <summary>
    /// Called when player loses a sphere - updates last known count
    /// </summary>
    void OnSphereStolen(int amount)
    {
        // Update last known count when spheres are removed
        if (playerInventory != null)
        {
            lastKnownSphereCount = playerInventory.NumberOfSpheres;
        }
    }
    
    /// <summary>
    /// Decrease demon doll speed when a sphere is stolen (full decrease)
    /// </summary>
    void DecreaseSpeed()
    {
        if (speedIncreaseCount > 0)
        {
            speedIncreaseCount--;
            currentSpeedModifier -= speedIncreasePerSphere;
            
            // Update all speeds
            UpdateSpeeds();
            
            Debug.Log($"Demon doll speed decreased! Normal speed: {normalSpeed}");
        }
    }
    
    /// <summary>
    /// Decrease demon doll speed when spheres are deposited (partial decrease)
    /// </summary>
    public void DecreaseSpeedOnDeposit(int sphereCount)
    {
        if (currentSpeedModifier > 0f)
        {
            float decreaseAmount = speedDecreasePerDeposit * sphereCount;
            currentSpeedModifier = Mathf.Max(0f, currentSpeedModifier - decreaseAmount);
            
            // Update all speeds
            UpdateSpeeds();
            
            Debug.Log($"Demon doll speed decreased by {decreaseAmount} from deposit! Normal speed: {normalSpeed}");
        }
    }
    
    /// <summary>
    /// Update all speeds based on current modifier
    /// </summary>
    void UpdateSpeeds()
    {
        normalSpeed = baseNormalSpeed + currentSpeedModifier;
        slowedSpeed = baseSlowedSpeed + currentSpeedModifier;
        retreatSpeed = baseRetreatSpeed + currentSpeedModifier;
        
        // Update current speed if agent exists
        if (agent != null)
        {
            // Update speed based on current state
            if (isInFearRadius)
            {
                agent.speed = retreatSpeed;
            }
            else if (isInDetectionRadius)
            {
                agent.speed = slowedSpeed;
            }
            else
            {
                agent.speed = normalSpeed;
            }
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
        
        // --- 5. Check for player contact ---
        UpdateContactCooldown();
        CheckPlayerContact();
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
    
    /// <summary>
    /// Update contact cooldown timer
    /// </summary>
    void UpdateContactCooldown()
    {
        if (contactCooldownTimer > 0f)
        {
            contactCooldownTimer -= Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Check if demon doll is close enough to touch player and steal sphere/kill
    /// </summary>
    void CheckPlayerContact()
    {
        if (playerTransform == null || contactCooldownTimer > 0f) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Check if within contact distance
        if (distanceToPlayer <= contactDistance)
        {
            HandlePlayerContact();
        }
    }
    
    /// <summary>
    /// Handle contact with player - steal sphere or kill
    /// </summary>
    void HandlePlayerContact()
    {
        if (playerInventory == null) return;
        
        // Reset cooldown
        contactCooldownTimer = contactCooldown;
        
        // Check if player has spheres
        if (playerInventory.NumberOfSpheres > 0)
        {
            // Steal one sphere (this will trigger respawn via event)
            SSphere stolenSphere = playerInventory.StealSphere();
            
            // Decrease demon doll speed when stealing
            DecreaseSpeed();
            
            Debug.Log("Demon doll stole a sphere! Remaining: " + playerInventory.NumberOfSpheres);
        }
        else
        {
            // Player has no spheres - die and reset
            Debug.Log("Player died! No spheres remaining. Game resetting...");
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
            else
            {
                // Fallback: reload scene directly
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }
    }
    
    /// <summary>
    /// Called when a sphere is stolen - respawns it at original location
    /// </summary>
    void OnSphereStolenForRespawn(SSphere sphere)
    {
        if (sphere != null)
        {
            sphere.Respawn();
            Debug.Log("Sphere respawned at original location");
        }
    }
}
