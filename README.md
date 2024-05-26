# PawnAI

## Overview
Small project for a flying drone that doesn't use pathfinding.

## State Machine
Drone operates on a statemachine with current implemented states being:
 - IdleState
 - IdlePlayState
 - PatrolState
 - ReturnToTargetState
 - FocusTargetState
 - FlyForEffectState

## Flying movement

For collisiton detection drone is doing a sphere cast in the direction of movement and makes adjustments based on detected collision. \
The movement of the drone has bobbing up and down implemented to simulate hovering, bobs more on smaller speeds.
Otherwise the movement is achieved by concept of velocity which is calculated by the following piece of code:
 ```
        // Calculate distance to target
        direction = Vector3.Lerp(GetForwardDirection(), (to - flyerTransform.position).normalized, flyerSettings._rotationSpeed / distanceToTarget).normalized;

        // Speed adjustment

        if (distanceToTarget < distanceFromTo / 2)
        {
            if(accTime > 0f) accTime -= Time.deltaTime;
            normalizedAccTime -= (accTime % flyerSettings.accelerationTime) / flyerSettings.accelerationTime;
            currentSpeed = flyerSettings.speedDeceleration.Evaluate(1f - distanceToTarget / (distanceFromTo / 2)) * flyerSettings.speed;
        }
        else
        {
            accTime += Time.deltaTime;
            if (normalizedAccTime < 0.95f)
                normalizedAccTime = (accTime % flyerSettings.accelerationTime) / flyerSettings.accelerationTime;
            currentSpeed = flyerSettings.speedAcceleration.Evaluate(normalizedAccTime) * flyerSettings.speed;
        }

        flyerVelocity = direction * currentSpeed + bobbingVector / currentSpeed;
 ```
