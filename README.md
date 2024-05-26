# PawnAI

## Overview
Small project for a flying drone that doesn't use navmesh agent but does utilise the navmesh class.

## State Machine
Drone operates on a statemachine with current implemented states being:
 - IdleState: The drone remains idle for a specified duration.
 - IdlePlayState: The drone idles with some play actions for a specified duration.
 - PatrolState: The drone patrols through a series of waypoints.
 - ReturnToTargetState: The drone returns to a specified target.
 - FocusTargetState: The drone focuses on a specified target.
 - FlyForEffectState: The drone flies for a specified effect duration.


## Flying movement

For collisiton detection drone is doing a sphere cast in the direction of movement and makes adjustments based on detected collision. \
The movement of the drone has bobbing up and down implemented to simulate hovering, bobs more on smaller speeds. \
Otherwise the movement is achieved by concept of velocity which is calculated by the following piece of code: \
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

## Some Examples
### Idle State->Idle Play where drone spins->Patrol

This example also shows the movement and collision detection/clearing

https://github.com/IGreisenI/PawnAI/assets/58489283/1f19e5bb-6ed4-42f6-80bf-60d0a760cf1a

### Idle State->Focus on Target (It's focusing the bean)

https://github.com/IGreisenI/PawnAI/assets/58489283/f0e7dfa1-79db-4906-b107-94c58d4362a5





