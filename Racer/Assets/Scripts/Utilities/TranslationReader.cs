using UnityEngine;

public class VelocityReader
{
    private Vector3 lastPosition = Vector2.zero;
    internal float lerpSpeed = 5;
    private bool init = true;

    public Vector3 value { get; private set; }
    public float x { get { return value.x; } }
    public float y { get { return value.y; } }
    public float z { get { return value.z; } }

    public VelocityReader(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }

    public void Update(Vector3 position)
    {
        if (init)
        {
            lastPosition = position;
            init = false;
        }

        value = (position - lastPosition) / Time.deltaTime;
        lastPosition = position;
    }


    public static implicit operator Vector3(VelocityReader reader)
    {
        return reader.value;
    }

    public static Vector3 operator *(VelocityReader reader, float x)
    {
        return reader.value * x;
    }

    public static Vector3 operator /(VelocityReader reader, float x)
    {
        return reader.value / x;
    }

    public static Vector3 operator +(VelocityReader reader, Vector3 x)
    {
        return reader.value + x;
    }

    public static Vector3 operator -(VelocityReader reader, Vector3 x)
    {
        return reader.value - x;
    }
}

public class AcceleraReader
{
    public VelocityReader velocity = new VelocityReader(5);
    private Vector3 lastVelocity = Vector3.zero;

    public Vector3 value { get; private set; }
    public float x { get { return value.x; } }
    public float y { get { return value.y; } }
    public float z { get { return value.z; } }

    public AcceleraReader(float lerpSpeed)
    {
        velocity = new VelocityReader(lerpSpeed);
    }

    public void Update(Vector3 position)
    {
        velocity.Update(position);
        value = velocity - lastVelocity;
        lastVelocity = velocity;
    }

    public static implicit operator Vector3(AcceleraReader reader)
    {
        return reader.value;
    }

    public static Vector3 operator *(AcceleraReader reader, float x)
    {
        return reader.value * x;
    }

    public static Vector3 operator /(AcceleraReader reader, float x)
    {
        return reader.value / x;
    }

    public static Vector3 operator +(AcceleraReader reader, Vector3 x)
    {
        return reader.value + x;
    }

    public static Vector3 operator -(AcceleraReader reader, Vector3 x)
    {
        return reader.value - x;
    }
}


