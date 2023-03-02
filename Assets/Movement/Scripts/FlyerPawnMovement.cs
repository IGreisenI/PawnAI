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
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > navMeshDistanceThreshold)
        {
            // Use raycast to check for obstacles
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, distance, ~navMeshLayer))
            {
                // If obstacle detected, move towards the hit point instead
                target.position = hit.point;
                direction = hit.point - transform.position;
            }
        }

        // Add perlin noise to movement
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, transform.position.y) * 2f - 1f;
        direction += transform.right * noise * noiseMagnitude;

        // Move towards the target position
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
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
