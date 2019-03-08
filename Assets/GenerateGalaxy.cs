using UnityEngine;
using System.Collections.Generic;

public class GenerateGalaxy : MonoBehaviour
{
    public GameObject star;
    private GameObject tempStar;

    private List<GameObject> stars = new List<GameObject>();

    public float minDistance = 5.0f;
    public float MaxDistance = 10.0f;

    public int numberOfStars;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        numberOfStars = Random.Range(100, 300);
        distance = Random.Range(minDistance, MaxDistance);

        for(int i = 0; i < numberOfStars; i++)
        {
            tempStar = Instantiate(star, transform);

            var rd = Random.value * 360;
            var rnd = Random.value * distance;

            //The x is just sin(angle) * distance
            var x = Mathf.Sin(rd * Mathf.Deg2Rad) * rnd;
            //The z is just cos(angle) * distance             
            var z = Mathf.Cos(rd * Mathf.Deg2Rad) * rnd;
            //Set the star’s position
            tempStar.transform.position = new Vector3(x, 0, z);
            stars.Add(tempStar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
