using UnityEngine;
using System.Collections.Generic;

public class GenerateGalaxy : MonoBehaviour
{
    public GameObject star;
    private GameObject tempStar;

    private List<GameObject> stars = new List<GameObject>();

    public int numberOfStars;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        numberOfStars = Random.Range(100, 300);
        distance = Random.Range(50.0f, 150.0f);

        for(int i = 0; i < numberOfStars; i++)
        {
            tempStar = Instantiate(star, transform);

            //The x is just sin(angle) * distance
            var x = Mathf.Sin(Random.value * Mathf.Deg2Rad) * (Random.value * distance * 30);
            //The z is just cos(angle) * distance
            var z = Mathf.Cos(Random.value * Mathf.Deg2Rad) * (Random.value * distance);
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
