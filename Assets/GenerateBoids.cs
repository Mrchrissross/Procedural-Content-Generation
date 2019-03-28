using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoids : MonoBehaviour
{
    [Header("Stats")]
    [Header("Boids")]
    public Boid boidPrefab;
    List<Boid> boids = new List<Boid>();
    public int numberOfBoids = 100;
    public int neighbourCount = 10;

    [Header("Height")]
    public float lowestHeight = 50.0f;
    public float highestHeight = 100.0f;

    [Header("Distance")]
    public float distance = 100.0f;

    [Header("Attraction")]

    [Header("Behaviour"), Tooltip("How close they need to be before they attract to each other.")]
    public float attractionRange = 25.0f;

    [Tooltip("The maximum amount of force in attraction.")]
    public float attractionMultiplier = 3.0f;

    [Tooltip("The attraction force they have after every calculation.")]
    public float attractionForce = 15.0f;

    [Header("Repulsion"), Tooltip("How close they need to be before they repel each other.")]
    public float repelRange = 20.0f;

    [Tooltip("The repel force they have after every calculation.")]
    public float repulsionForce = 12.0f;

    [Header("Alignment"), Tooltip("How close they need to be before they align with each other.")]
    public float alignmentRange = 15.0f;

    [Tooltip("The alignment force they have after every calculation.")]
    public float alignmentForce = 12.0f;

    [Header("Nesting")]
    public List<GameObject> listOfNests = new List<GameObject>();
    public float nestRange = 100f;
    public float nestAttraction = 0.05f;

    public static GenerateBoids instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for (int i = 0; i < numberOfBoids; ++i)
        {
            Boid newBoid = Instantiate(boidPrefab, transform);
            newBoid.transform.position = new Vector3(Random.Range(50.0f, -50.0f), Random.Range(50.0f, -50.0f), Random.Range(50.0f, -50.0f));
            boids.Add(newBoid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfBoids; ++i)
        {
            // Assign variables
            boids[i].lowestHeight = lowestHeight;
            boids[i].highestHeight = highestHeight;
            boids[i].distance = distance;
            boids[i].nestRange = nestRange;
            boids[i].nestAttraction = nestAttraction;

            boids[i].ComputeForce(boids, neighbourCount, numberOfBoids);
            boids[i].UpdatePosition();
        }
    }

    public void AddNest(GameObject nest)
    {
        listOfNests.Add(gameObject);
    }

    public void RemoveNest(GameObject nest)
    {
        listOfNests.Remove(gameObject);
    }
}
