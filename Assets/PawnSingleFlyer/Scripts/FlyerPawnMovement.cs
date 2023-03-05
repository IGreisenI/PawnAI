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
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, transform.position.y) * 2f - 1f;

        Vector3 noiseVector = GetRandomVector() * noise * noiseMagnitude;
        direction = Vector3.Lerp(GetForwardDirection(), (to - transform.position).normalized, curveAmount * _rotationSpeed / distanceToTarget);
        flyerVelocity = (direction + noiseVector) * _speed;

        // Move towards the target position
        if (DetectCollision())
        {
            flyerVelocity = transform.forward / _speed;
        }
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
        randomVector.y = Mathf.Abs(randomVector.y);
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
        if (Physics.Raycast(ray, out hit, flyerVelocity.magnitude)){
            flyerVelocity += hit.normal * flyerVelocity.magnitude;
            return true;
        }
        return false;
    }
}
