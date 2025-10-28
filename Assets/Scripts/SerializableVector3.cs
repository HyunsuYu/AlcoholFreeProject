using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public struct SerializableVector3
{
    public float x, y, z;


    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public SerializableVector3(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static bool operator ==(SerializableVector3 a, SerializableVector3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    public static bool operator !=(SerializableVector3 a, SerializableVector3 b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        if (obj is SerializableVector3 other)
        {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return (x, y, z).GetHashCode();
    }
}