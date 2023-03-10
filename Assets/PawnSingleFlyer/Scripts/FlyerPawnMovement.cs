using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class FlyerPawnMovement : Movement
{
    [SerializeField] private Transform flyerTransform;

    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float bobbingFrequency = 1f;
    [SerializeField] private float bobbingAmplitude = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float avoidDistance = 1f;
    [SerializeField] private AnimationCurve speedFalloff;
    [SerializeField] private AnimationCurve rotationCurve;

    private Vector3 flyerVelocity;
    private float currentSpeed;

    #region CACHE
    private Vector3 direction;
    private Quaternion startRotation = Quaternion.identity;
    #endregion

    public FlyerPawnMovement(Transform transform)
    {
        this.flyerTransform = transform;

        flyerVelocity = transform.forward * speed;
    }

    public void DebugGizmo()
    {
        if (flyerTransform == null || flyerVelocity == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(flyerTransform.position, flyerTransform.position + flyerVelocity.normalized);
    }

    public override void Move(Vector3 from, Vector3 to)
    {
        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(flyerTransform.position, to);

        // Add sine wave to movement
        float time = Time.time * bobbingFrequency;
        Vector3 bobbingVector = new Vector3(0.0f, Mathf.Sin(time) * bobbingAmplitude, 0.0f);

        direction = Vector3.Lerp(GetForwardDirection(), (to - flyerTransform.position).normalized, _rotationSpeed / distanceToTarget);

        // Speed adjustment
        if (from == flyerTransform.position)
            currentSpeed = speed;
        else
            currentSpeed = speedFalloff.Evaluate(Vector3.Distance(flyerTransform.position, to) / (to - from).magnitude) * speed;

        flyerVelocity = (direction + bobbingVector) * currentSpeed;

        // Collision Handling
        DetectCollision();
        StayAwayFromObjects();

        // Move towards the target position
        flyerTransform.position += flyerVelocity * Time.deltaTime;

        // Rotate towards the target position
        if (direction != Vector3.zero && Vector3.Distance(flyerTransform.position, to) > 0.1f)
        {
            Vector3 rotationLook = flyerVelocity.normalized;
            rotationLook.y = Mathf.Clamp(rotationLook.y, -0.1f, 0.1f);

            float normalizedTime = (Time.time % animationDuration) / animationDuration;
            if(normalizedTime > 0.99f)
            {
                startRotation = flyerTransform.rotation;
            }

            flyerTransform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(rotationLook), rotationCurve.Evaluate(normalizedTime));
        }
        else
        {
            startRotation = flyerTransform.rotation;
        }
    }

    public override void ResetMovement()
    {
        flyerVelocity = Vector3.zero;
    }

    private Vector3 GetForwardDirection()
    {
        return (flyerVelocity != Vector3.zero) ? flyerVelocity.normalized : Vector3.forward;
    }

    private Quaternion GetRotation()
    {
        return Quaternion.LookRotation(GetForwardDirection());
    }

    public Vector3 GetVelocity()
    {
        return flyerVelocity;
    }

    private bool DetectCollision()
    {
        Ray ray = new Ray(flyerTransform.position, flyerVelocity);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, flyerVelocity.magnitude))
        {
            if((hit.point - flyerTransform.position).magnitude < avoidDistance)
            {
                flyerVelocity += hit.normal * (avoidDistance - (hit.point - flyerTransform.position).magnitude);
                return true;
            }
        }
        return false;
    }

    private void StayAwayFromObjects()
    {
        // Cast a sphere in front of the AI to detect nearby obstacles
        Collider[] colliders = Physics.OverlapSphere(flyerTransform.position + flyerTransform.forward * avoidDistance, avoidDistance * 2);
        if (colliders.Length == 0)
        {
            return;
        }

        // Calculate a steering force to avoid the obstacles
        Vector3 avoidanceForce = Vector3.zero;
        foreach (Collider collider in colliders)
        {
            Vector3 obstaclePosition = collider.transform.position;
            Vector3 toObstacle = obstaclePosition - flyerTransform.position;
            float distance = toObstacle.magnitude;
            if (distance > 0f && distance < avoidDistance)
            {
                float weight = 1f - (distance / avoidDistance);
                Vector3 direction = toObstacle.normalized;
                avoidanceForce -= direction * weight;
            }
        }

        // Apply the steering force to the velocity vector
        flyerVelocity += avoidanceForce.normalized * currentSpeed * Time.deltaTime;
    }
}
