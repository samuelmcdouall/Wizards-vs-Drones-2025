using System;
using UnityEngine;

public struct WVDPlayerDirection : IEquatable<WVDPlayerDirection>
{
    public Vector3 DirectionVector; // Actual movement direction
    public Vector3 InputVector; // Keys being input for animation

    public WVDPlayerDirection(Vector3 directionVector, Vector3 inputVector)
    {
        DirectionVector = directionVector;
        InputVector = inputVector;
    }
    public bool Equals(WVDPlayerDirection other)
    {
        return DirectionVector.x == other.DirectionVector.x &&
               DirectionVector.y == other.DirectionVector.y &&
               DirectionVector.z == other.DirectionVector.z &&
               InputVector.x == other.DirectionVector.x     &&
               InputVector.y == other.DirectionVector.y     &&
               InputVector.z == other.DirectionVector.z;
    }
    public override string ToString()
    {
        return $"Direction Vector: ({DirectionVector.x}, {DirectionVector.y}, {DirectionVector.z})" +
               $"Input Vector: ({InputVector.x}, {InputVector.y}, {InputVector.z})";
    }
}