using UnityEngine;
using System.Collections.Generic;

public class GenerateGalaxy : MonoBehaviour
{
    public GameObject star;
    public Material starMaterial;
    private GameObject tempStar;
    private List<GameObject> stars = new List<GameObject>();

    float minDistance = 5.0f;
    float MaxDistance = 10.0f;

    int numberOfStars;
    float distance;

    public float rotationSpeed = 2.0f;
    public float scaleSpeed = 0.05f;
    public float maxScale = 2.0f;
    public float minScale = 1.0f;
    bool scaleDir;

    // Start is called before the first frame update
    void Start()
    {
        spawnStarsSpiral();

        //numberOfStars = Random.Range(100, 300);
        //distance = Random.Range(minDistance, MaxDistance);

        //for (int i = 0; i < numberOfStars; i++)
        //{
        //    tempStar = Instantiate(star, transform);

        //    var rd = Random.value * 360;
        //    var rnd = Random.value * distance;

        //    //The x is just sin(angle) * distance
        //    var x = Mathf.Sin(rd * Mathf.Deg2Rad) * rnd;
        //    //The z is just cos(angle) * distance             
        //    var z = Mathf.Cos(rd * Mathf.Deg2Rad) * rnd;
        //    //Set the star’s position
        //    tempStar.transform.position = new Vector3(x, 0, z);

        //    //spawnStarsSpiral();
        //    stars.Add(tempStar);
        //}
    }

    void spawnStarsSpiral()
    {
        float A = 40.0f;
        float B = 11.12f;
        float N = 0.706f;
        float theta = Mathf.Atan2(A, B);

        for(int i = 0; i < (360 * 5); i++)
        {
            float angleRadians = i * Mathf.Deg2Rad;
            float dist = A / Mathf.Log10(B * Mathf.Tan(angleRadians / (2 * N)));

            if (dist > Mathf.Abs(A))
                continue;

            float threshold = A * 0.9f;

            dist = Mathf.Clamp(dist, 0, threshold);

            if (Mathf.Approximately(dist, threshold))
                continue;

            float offset = Random.Range(-10f, 10f) * Mathf.Deg2Rad;

            //Get the x and z coordinates
            float x = Mathf.Cos(angleRadians + offset) * dist;
            float z = Mathf.Sin(angleRadians + offset) * dist;

            if (float.IsNaN(x) || float.IsNaN(z) || (x == 0 && z == 0))
                continue;

            //Put them in a vector
            var pos = new Vector3(x, 0, z);

            //And spawn the star at this position
            tempStar = Instantiate(star, transform);
            tempStar.transform.localPosition = pos;
            tempStar.transform.GetChild(0).GetComponent<MeshRenderer>().material = starMaterial;
            stars.Add(tempStar);
        }
        //Continue
    }

    private void Update()
    {
        float newRotationY = (transform.rotation.eulerAngles.y > 360.0f) ? 0 : transform.rotation.eulerAngles.y + (Time.deltaTime * rotationSpeed);
        transform.eulerAngles = new Vector3(transform.rotation.x, newRotationY, transform.rotation.z);

        if (transform.localScale.x > maxScale)
            scaleDir = false;
        else if (transform.localScale.x < minScale)
            scaleDir = true;

        float scale = (!scaleDir) ? transform.localScale.x - (Time.deltaTime * scaleSpeed) : transform.localScale.x + (Time.deltaTime * scaleSpeed);
        transform.localScale = new Vector3(scale, 1, scale);
    }

}
