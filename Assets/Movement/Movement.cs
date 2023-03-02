using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    public abstract void Move(Transform target, float speed, float rotationSpeed);
}
