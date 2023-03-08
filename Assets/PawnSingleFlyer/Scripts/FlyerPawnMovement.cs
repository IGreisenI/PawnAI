using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerPawnMovement : Movement
{
    private Transform transform;
    private float _speed;
    private float _rotationSpeed;
    private float noiseFrequency;
    private float noiseMagnitude;

    private Vector3 flyerVelocity;

    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    private float currentSpeed;
    private float distanceFromObsticale = 2f;

    #region CACHE
    Vector3 direction;
    #endregion

    public FlyerPawnMovement(Transform transform, float speed, float rotationSpeed, float noiseFrequency, float noiseMagnitude)
    {
        this._speed = speed;
        this._rotationSpeed = rotationSpeed;

        this.transform = transform;
        this.noiseFrequency = noiseFrequency;
        this.noiseMagnitude = noiseMagnitude;

        flyerVelocity = transform.forward / _speed;
    }

    public void DebugGizmo()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + flyerVelocity.normalized);
    }

    public override void Move(Vector3 from, Vector3 to)
    {
        // Define a curve amount to control the amount of curve towards the target
        float curveAmount = 0.1f;

        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(transform.position, to);

        // Add perlin noise to movement
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, Time.time * noiseFrequency);

        Vector3 noiseVector = GetRandomVector() * noise * noiseMagnitude;
        direction = Vector3.Lerp(GetForwardDirection(), (to - transform.position).normalized, curveAmount * _rotationSpeed / distanceToTarget);

        // Speed adjustment
        currentSpeed = Mathf.Lerp(_speed / 5f , _speed, Mathf.Abs(1 -Mathf.Clamp(Vector3.Distance(transform.position, from + (to-from)/2) / 30f, 0, 1)));
        flyerVelocity = (direction + noiseVector) * currentSpeed;

        // Collision Handling
        DetectCollision();
        StayAwayFromObjects();

        // Move towards the target position
        transform.position += flyerVelocity * Time.deltaTime;

        // Rotate towards the target position
        if (direction != Vector3.zero && Vector3.Distance(transform.position, to) > 0.5f)
        {
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, GetRotation(), _speed * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }


    public override void ResetMovement()
    {

    }

    private Vector3 GetRandomVector()
    {
        Vector3 randomVector = Random.insideUnitSphere;
        randomVector.x = 0;
        randomVector.z = 0;
        return randomVector.normalized;
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
        Ray ray = new Ray(transform.position, flyerVelocity);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, flyerVelocity.magnitude))
        {
            if((hit.point - transform.position).magnitude < distanceFromObsticale)
            {
                flyerVelocity += hit.normal * (distanceFromObsticale - (hit.point - transform.position).magnitude);
                return true;
            }
        }
        return false;
    }

    private void StayAwayFromObjects()
    {
        // Cast a sphere in front of the AI to detect nearby obstacles
        float avoidDistance = 5f;
        float sphereRadius = 3f;
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * avoidDistance, sphereRadius);
        if (colliders.Length == 0)
        {
            return;
        }

        // Calculate a steering force to avoid the obstacles
        Vector3 avoidanceForce = Vector3.zero;
        foreach (Collider collider in colliders)
        {
            Vector3 obstaclePosition = collider.transform.position;
            Vector3 toObstacle = obstaclePosition - transform.position;
            float distance = toObstacle.magnitude;
            if (distance > 0f && distance < avoidDistance)
            {
                float weight = 1f - (distance / avoidDistance);
                Vector3 direction = toObstacle.normalized;
                avoidanceForce -= direction * weight;
            }
        }

        // Apply the steering force to the velocity vector
        flyerVelocity += avoidanceForce.normalized * _speed * Time.deltaTime;
    }
}
