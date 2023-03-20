using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class FlyerPawnMovement : Movement
{
    [Tooltip("Parent Transform of object that is moved")]
    [SerializeField] public Transform flyerTransform;
    [Tooltip("Scriptable object that are the movement settings")]
    [SerializeField] FlyerSettings flyerSettings;

    [Tooltip("NavMeshDatas to use to find path")]
    [SerializeField] private List<NavMeshData> navMeshDatas = new List<NavMeshData>();

    private List<NavMeshDataInstance> navMeshDatainstances = new List<NavMeshDataInstance>();
    private NavMeshPath navMeshPath;
    private int currentCornerIndex = 1;

    private Vector3 flyerVelocity;
    private float currentSpeed;

    // Caching frequently-used values for performance
    #region CACHE
    private Vector3 direction;
    private Quaternion startRotation = Quaternion.identity;
    private Vector3 avoidanceForce;

    float distanceToTarget;
    float time;
    float normalizedTime;
    float clampedXRotation;
    Vector3 bobbingVector;
    Vector3 currentRotation;
    #endregion

    public void NavMeshInit()
    {
        // Create a new NavMeshData instance for each NavMeshData in the data list
        foreach (NavMeshData navMeshData in navMeshDatas)
        {
            NavMeshDataInstance navMesh = NavMesh.AddNavMeshData(navMeshData);
            navMeshDatainstances.Add(navMesh);
        }
        navMeshPath = new NavMeshPath();
    }

    public void DebugGizmo()
    {
        if (flyerTransform == null || flyerVelocity == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(flyerTransform.position, flyerTransform.position + flyerVelocity.normalized);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(flyerTransform.position, flyerTransform.position + avoidanceForce * currentSpeed);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(flyerTransform.position + flyerTransform.forward, flyerSettings.avoidDistance);

        if (navMeshPath == null) return;
        foreach(Vector3 pos in navMeshPath.corners)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    public override void Move(Vector3 from, Vector3 to)
    {
        // If there is a collision and the current corner index is less than the number of corners in the NavMeshPath, adjust the from and to positions
        if (DetectCollision(flyerTransform.position, to) && currentCornerIndex < navMeshPath.corners.Length)
        {
            from = flyerTransform.position;
            to = navMeshPath.corners[currentCornerIndex] + new Vector3(0f, from.y);

            if(Vector3.Distance(from, to) < 0.1f)
            {
                currentCornerIndex++;
            }
        }
        else
        {
            currentCornerIndex = 1;
            navMeshPath?.ClearCorners();
        }

        // Calculate distance to target
        distanceToTarget = Vector3.Distance(flyerTransform.position, to);

        // Add sine wave to movement
        time = Time.time * flyerSettings.bobbingFrequency;
        bobbingVector = new Vector3(0.0f, Mathf.Sin(time) * flyerSettings.bobbingAmplitude, 0.0f);

        direction = Vector3.Lerp(GetForwardDirection(), (to - flyerTransform.position).normalized, flyerSettings._rotationSpeed / distanceToTarget).normalized;

        // Speed adjustment
        if (from == flyerTransform.position)
            currentSpeed = flyerSettings.speed;
        else
            currentSpeed = flyerSettings.speedFalloff.Evaluate(Vector3.Distance(flyerTransform.position, to) / (to - from).magnitude) * flyerSettings.speed;

        flyerVelocity = direction * currentSpeed + bobbingVector / currentSpeed;

        StayAwayFromObjects();

        // Move towards the target position
        flyerTransform.position += flyerVelocity * Time.deltaTime;

        // Rotate towards the target position
        if (direction != Vector3.zero && Vector3.Distance(flyerTransform.position, to) > 0.1f)
        {
            normalizedTime = ((Time.time) % flyerSettings.rotationDuration) / flyerSettings.rotationDuration;
            if(normalizedTime > 0.99f)
            {
                startRotation = flyerTransform.rotation;
            }

            currentRotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(flyerVelocity.normalized), flyerSettings.rotationCurve.Evaluate(normalizedTime)).eulerAngles;

            clampedXRotation = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
            clampedXRotation = Mathf.Clamp(clampedXRotation, -flyerSettings.clampedXAngle, flyerSettings.clampedXAngle);

            // Set the new rotation of the object
            flyerTransform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(clampedXRotation, currentRotation.y, currentRotation.z), flyerSettings.rotationCurve.Evaluate(normalizedTime));
        }
        else
        {
            startRotation = flyerTransform.rotation;
        }
    }

    private Vector3 GetForwardDirection()
    {
        return (flyerVelocity != Vector3.zero) ? flyerVelocity.normalized : Vector3.forward;
    }

    public Vector3 GetVelocity()
    {
        return flyerVelocity;
    }

    private bool DetectCollision(Vector3 from, Vector3 to)
    {
        Ray ray = new Ray(flyerTransform.position, (to - flyerTransform.position).normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, flyerSettings.avoidDistance + 0.5f))
        {
            if (navMeshPath.corners.Length == 0 && NavMesh.CalculatePath(new Vector3(from.x, 0, from.z), new Vector3 (to.x, 0, to.z), NavMesh.AllAreas, navMeshPath))
            {

            }
            return true;
        }
        return false;
    }

    private void StayAwayFromObjects()
    {
        // Cast a sphere in front of the AI to detect nearby obstacles
        Collider[] colliders = Physics.OverlapSphere(flyerTransform.position + flyerTransform.forward, flyerSettings.avoidDistance);
        if (colliders.Length == 0)
        {
            return;
        }

        // Calculate a steering force to avoid the obstacles
        avoidanceForce = Vector3.zero;
        foreach (Collider collider in colliders)
        {
            // Get the closest point on the collider to the flyer's position
            Vector3 obstaclePosition = Physics.ClosestPoint(flyerTransform.position, collider, collider.transform.position, collider.transform.rotation);

            Vector3 toObstacle = obstaclePosition - flyerTransform.position;
            float distance = toObstacle.magnitude;
            if (distance > 0f && distance < flyerSettings.avoidDistance)
            {
                float weight = 1f - (distance / flyerSettings.avoidDistance);
                Vector3 direction = toObstacle.normalized;
                avoidanceForce -= direction * weight;
            }
        }

        // Apply the steering force to the velocity vector
        flyerVelocity += avoidanceForce * flyerSettings.avoidForce;
    }

    public override void ChangeSpeed(float speed)
    {
        flyerSettings.speed = speed;
    }

    public override float GetSpeed()
    {
        return flyerSettings.speed;
    }    
    
    public override void ResetMovement()
    {
        flyerVelocity = Vector3.zero;
    }

}