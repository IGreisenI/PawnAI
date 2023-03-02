using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public abstract void Move(Vector3 position, float speed, float rotationSpeed);
}
