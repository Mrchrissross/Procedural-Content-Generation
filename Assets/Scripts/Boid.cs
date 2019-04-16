using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Height")]
    public float lowestHeight = 50.0f;
    public float highestHeight = 100.0f;

    [Header("Distance")]
    public Vector2 distance = new Vector2(-100.0f, 100.0f);

    [Header("Speed")]
    public float speed = 5.0f;

    [Header("Rotation")]
    public bool directionOnly;

    [Header("Nesting")]
    GameObject nearestNest;
    public float nestRange = 100f;
    public float nestAttraction = 0.05f;

    Vector3 force;
    Rigidbody rb;

    [Header("Attraction"), Tooltip("How close they need to be before they attract to each other."), HideInInspector]
    public float attractionRange = 25.0f;

    [Tooltip("The maximum amount of force in attraction."), HideInInspector]
    public float attractionMultiplier = 3.0f;

    [Tooltip("The attraction force they have after every calculation."), HideInInspector]
    public float attractionForce = 15.0f;

    [Header("Repulsion"), Tooltip("How close they need to be before they repel each other."), HideInInspector]
    public float repelRange = 20.0f;

    [Tooltip("The repel force they have after every calculation."), HideInInspector]
    public float repulsionForce = 12.0f;

    [Header("Alignment"), Tooltip("How close they need to be before they align with each other."), HideInInspector]
    public float alignmentRange = 15.0f;

    [Tooltip("The alignment force they have after every calculation."), HideInInspector]
    public float alignmentForce = 12.0f;

    public List<GameObject> listOfNests = new List<GameObject>();

    public void Initialise(bool ship, bool randomSail, Color sailColour)
    {
        rb = GetComponent<Rigidbody>();

        if(ship)
        {
            transform.GetChild(0).GetComponent<GeneratePirateShip>()._randomColour = randomSail;
            transform.GetChild(0).GetComponent<GeneratePirateShip>().sailColour = sailColour;
        }
    }

    public void ComputeForce(List<Boid> boid, int nCount, int NumBoids)
    {
        nearestNest = null;
        foreach (var nest in listOfNests)
        {
            float dist = Vector3.Distance(nest.transform.position, transform.position);

            if (dist < nestRange)
            {
                if(nearestNest == null)
                    nearestNest = nest;
                else
                {
                    if(Vector3.Distance(nearestNest.transform.position,transform.position) > dist)
                        nearestNest = nest;
                }
            }
        }

        force = Vector3.zero;

        Vector3 pos = transform.position.xz(directionOnly),
                repelForce = Vector3.zero,
                sep = Vector3.zero,
                delta = Vector3.zero,
                direction = Vector3.zero,
                vDesired = Vector3.zero,
                centerOfMass = Vector3.zero,
                alignForce = Vector3.zero,
                attractForce = Vector3.zero;

        int     repelCount = 0,
                alignCount = 0,
                attractCount = 0;

        float distance = 0;

        for (int i = 0; i < NumBoids; i++)
        {
            Vector3 boidPos = boid[i].transform.position.xz(directionOnly);

            if (this == boid[i]) continue;

            sep = pos - boidPos;
            distance = sep.magnitude;

            // Repel
            if ((distance < repelRange) && (repelCount < nCount))
            {
                delta = pos - boidPos;
                repelForce += (delta / delta.magnitude);
                repelCount++;
            }

            // Align
            if ((distance < alignmentRange) && (alignCount < nCount))
            {
                direction += boid[i].rb.velocity.xz(directionOnly);
                alignCount++;
            }

            // Attract
            if ((distance < attractionRange) && (attractCount < nCount))
            {
                centerOfMass += boidPos;
                attractCount++;
            }
        }

        // Repel
        if (repelCount > 0)
            repelForce *= repulsionForce;

        // Align
        if (alignCount > 0)
        {
            direction /= alignCount;
            vDesired = direction;
            alignForce += (vDesired - rb.velocity.xz(directionOnly)) * alignmentForce;
        }

        // Attract
        if (attractCount > 0)
        {
            centerOfMass /= attractCount;
            direction = (centerOfMass - pos).normalized;
            vDesired = direction * attractionMultiplier;
            attractForce += (vDesired - rb.velocity.xz(directionOnly)) * attractionForce;
        }

        //move to nearest nest
        if (nearestNest != null)
        {
            direction = (nearestNest.transform.position - pos).normalized;
            vDesired = direction * attractionMultiplier * nestAttraction;
            attractForce += (vDesired - rb.velocity.xz(directionOnly)) * attractionForce;
        }

        force = repelForce + alignForce + attractForce;
    }

    public void UpdatePosition()
    {
        rb.AddForce(force.xz(directionOnly));

        Vector3 vel = rb.velocity.xz(directionOnly);
        float direction = vel.magnitude;

        if (direction < 10.0f)
        {
            direction = 10.0f;
            rb.velocity = vel.normalized * direction;
        }
        else if (direction > 100.0f)
        {
            direction = 100.0f;
            rb.velocity = vel.normalized * direction;
        }

        Vector3 pos = transform.position.xz(directionOnly);
        transform.LookAt(pos + -vel);

        if (directionOnly)
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        // Add constraints.
        if (pos.x < distance.x) { pos.x = distance.x; transform.position = pos; }
        else if (pos.x > distance.y) { pos.x = distance.y; transform.position = pos; }

        if (pos.y < lowestHeight) { pos.y = lowestHeight; transform.position = pos; }
        else if (pos.y > highestHeight) { pos.y = highestHeight; transform.position = pos; }

        if (pos.z < distance.x) { pos.z = distance.x; transform.position = pos; }
        else if (pos.z > distance.y) { pos.z = distance.y; transform.position = pos; }

        // Increase the boid speed.
        rb.velocity = rb.velocity * speed;

        // Fail-Safe: not all boids spawn near eachother and if a boid has no neighbours it will not move.
        // This makes them all move toward eachother, making them become neighbours with one another.
        if (force == Vector3.zero)
        {
            transform.LookAt(Vector3.zero);

            rb.velocity = transform.forward.normalized * 10;
        }
    }

    public void AcquireNests(List<GameObject> _listOfNests)
    {
        foreach (GameObject nest in _listOfNests)
        {
            listOfNests.Add(nest);
        }
    }
}

static class Vector3Extensions
{
    public static Vector3 xz(this Vector3 pos, bool directionOnly = false)
    {
        if (directionOnly)
            return new Vector3(pos.x, 0, pos.z);
        else
            return pos;
    }
}