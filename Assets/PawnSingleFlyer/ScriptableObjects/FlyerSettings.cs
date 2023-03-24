using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "MovementSettings/FlyerSettings", order = 1)]
public class FlyerSettings : ScriptableObject
{
    [Header("Speed")]
    [Tooltip("Maximum speed of the flyer")]
    [SerializeField] public float speed = 1f;
    [Tooltip("Acceleration time")]
    [SerializeField] public float accelerationTime = 1f;
    [Tooltip("Speed acceleration over time")]
    [SerializeField] public AnimationCurve speedAcceleration;
    [Tooltip("Speed curve over distance from point to point")]
    [SerializeField] public AnimationCurve speedDeceleration;

    [Header("Rotation")]
    [Tooltip("Rotation speed of the flyer")]
    [SerializeField] public float _rotationSpeed = 1f;
    [Tooltip("How long does the rotation last")]
    [SerializeField] public float rotationDuration = 1f;
    [Tooltip("How long does the rotation last")]
    [SerializeField] public float clampedXAngle = 10f;
    [Tooltip("Rotation amount over time")]
    [SerializeField] public AnimationCurve rotationCurve;

    [Header("Noise")]
    [Tooltip("How frequent is the change of going up/down")]
    [SerializeField] public float bobbingFrequency = 1f;
    [Tooltip("How much will the flyer go up and down")]
    [SerializeField] public float bobbingAmplitude = 1f;

    [Header("Avoidance Settings")]
    [Tooltip("How far will the flyer try to stay away from obstacles")]
    [SerializeField] public float avoidDistance = 1f;
    [Tooltip("How strongly will the flyer try to stay away from obstacles")]
    [SerializeField] public float avoidForce = 1f;
}
