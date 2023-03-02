using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerPawnMovement : Movement
{
    private Transform transform;
    private float noiseFrequency;
    private float noiseMagnitude;
    private float navMeshDistanceThreshold = 1f;
    private int navMeshLayer = 1 << 10; // Use layer 10 for nav mesh

    public FlyerPawnMovement(Transform transform, float noiseFrequency, float noiseMagnitude)
    {
        this.transform = transform;
        this.noiseFrequency = noiseFrequency;
        this.noiseMagnitude = noiseMagnitude;
    }

    public override void Move(Transform target, float speed, float rotationSpeed)
    {
        // Add perlin noise to movement
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, transform.position.y) * 2f - 1f;
        Vector3 noiseVector = transform.up * noise * noiseMagnitude + transform.right * noise * noiseMagnitude;
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newPosition = transform.position + (direction + noiseVector) * speed * Time.deltaTime;

        // Move towards the target position
        transform.position = newPosition;

        // Rotate towards the target position
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }
}
