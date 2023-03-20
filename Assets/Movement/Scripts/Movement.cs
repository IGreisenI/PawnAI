using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    public abstract void ChangeSpeed(float speed);
    public abstract float GetSpeed();

    public abstract void Move(Vector3 from, Vector3 to);
    public abstract void ResetMovement();
}
