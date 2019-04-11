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
    
    [Header("Spawning")]
    public float distance = 100.0f;

    [Space, VectorLabels("Lowest", "Highest")]
    public Vector2 height = new Vector2(50.0f, 100.0f);

    [Header("Behaviour")]
    public bool bird;
    public bool ship;
    public bool fish;

    [Tooltip("Speed at which the boids move."), Range(1.0f, 2.0f)]
    public float speed = 1.0f;

    [Space, Tooltip("How attracted they are to each other."), VectorLabels("Range", "Multiplier", "Force")]
    public Vector3 attraction = new Vector3(25.0f, 3.0f, 15.0f);

    [Space, Tooltip("How close they need to be before they repel each other."), VectorLabels("Range", "Force")]
    public Vector2 repulsion = new Vector2(20.0f, 12.0f);

    [Space, Tooltip("How close they need to be before they align with each other."), VectorLabels("Range", "Force")]
    public Vector2 alignment = new Vector2(15.0f, 12.0f);

    [Header("Nesting")]
    public List<GameObject> listOfNests = new List<GameObject>();
    [Space, Tooltip("The range at which the nest will attract boids.")]
    public float nestRange = 100f;
    [Space, Tooltip("How strong the boids wil be attracted to the nest."), VectorLabels("Force", "Current Force")]
    public Vector2 nestAttraction = new Vector2(2.0f, 0.0f);
    [Space, Tooltip("Time between attractions."), VectorLabels("Time", "Count")]
    public Vector2 nestTimer = new Vector2(8.0f, 0.0f);
    [Space, Tooltip("How long the attraction will last."), VectorLabels("Time", "Count")]
    public Vector2 nestAttractionTimer = new Vector2(1.5f, 0.0f);

    GameObject water;

    // Start is called before the first frame update
    void Start()
    {
        nestTimer.y = nestTimer.x;
        nestAttractionTimer.y = nestAttractionTimer.x;

        water = GameObject.Find("water");
        if(bird)
            this.name = "Birds";
        else if (ship)
        {
            this.name = "Ships";
            height.x = water.transform.position.y + 0.25f;
            height.y = water.transform.position.y + 0.5f;
        }
        else if(fish)
        {
            this.name = "Fish";
            height.x = water.transform.position.y - 4.0f;
            height.y = water.transform.position.y - 1.0f;
        }

        Transform nest = transform.GetChild(0).GetChild(0);
        nest.position = new Vector3(nest.position.x, height.y - ((height.y - height.x) / 2), nest.position.z);

        for (int i = 0; i < numberOfBoids; ++i)
        {
            Boid newBoid = Instantiate(boidPrefab, transform);
            newBoid.transform.position = new Vector3(Random.Range(-distance, distance), Random.Range(height.x, height.y), Random.Range(-distance, distance));
            newBoid.AcquireNests(listOfNests);
            boids.Add(newBoid);
        }

        distance *= 10;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfBoids; ++i)
        {
            // Assign variables
            if(fish || ship)
                boids[i].directionOnly = true;

            boids[i].speed = speed;
            boids[i].distance = distance;
            boids[i].lowestHeight = height.x;
            boids[i].highestHeight = height.y;
            boids[i].attractionRange = attraction.x;
            boids[i].attractionMultiplier = attraction.y;
            boids[i].attractionForce = attraction.z;
            boids[i].repelRange = repulsion.x;
            boids[i].repulsionForce = repulsion.y;
            boids[i].alignmentRange = alignment.x;
            boids[i].alignmentForce = alignment.y;
            boids[i].nestRange = nestRange;
            boids[i].nestAttraction = nestAttraction.y;

            boids[i].ComputeForce(boids, neighbourCount, numberOfBoids);
            boids[i].UpdatePosition();
        }

        nestTimer.y -= Time.deltaTime;

        if(nestTimer.y < 0)
        {
            nestAttractionTimer.y -= Time.deltaTime;
            nestAttraction.y = nestAttraction.x;

            if(nestAttractionTimer.y < 0)
            {
                nestAttraction.y = 0;
                nestTimer.y = nestTimer.x;
                nestAttractionTimer.y = nestAttractionTimer.x;
            }
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
