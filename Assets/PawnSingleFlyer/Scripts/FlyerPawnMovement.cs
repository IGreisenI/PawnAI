using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerPawnMovement : Movement
{
    private Transform transform;
    public float _speed;
    public float _rotationSpeed;
    private float noiseFrequency;
    private float noiseMagnitude;

    public FlyerPawnMovement(Transform transform, float speed, float rotationSpeed, float noiseFrequency, float noiseMagnitude)
    {
        this._speed = speed;
        this._rotationSpeed = rotationSpeed;

        this.transform = transform;
        this.noiseFrequency = noiseFrequency;
        this.noiseMagnitude = noiseMagnitude;
    }

    public override void Move(Vector3 from, Vector3 to)
    {
        // Define a curve amount to control the amount of curve towards the target
        float curveAmount = 0.5f;

        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(transform.position, to);

        // Add perlin noise to movement
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, transform.position.y) * 2f - 1f;
        Vector3 noiseVector = transform.up * noise * noiseMagnitude + transform.right * noise * noiseMagnitude;
        Vector3 direction = Vector3.Lerp(transform.forward, (to - transform.position).normalized, curveAmount / distanceToTarget);
        Vector3 newPosition = transform.position + (direction + noiseVector) * _speed * Time.deltaTime;

        // Move towards the target position
        transform.position = newPosition;

        // Rotate towards the target position
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            transform.rotation = newRotation;
        }
    }
}
