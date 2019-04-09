using UnityEngine;
using System.Collections.Generic;

public class GenerateGalaxy : MonoBehaviour
{
    [System.Serializable]
    public class Star
    {
        public GameObject star;
        public Material starMaterial;
    };

    public Star assets;

    private List<GameObject> stars = new List<GameObject>();

    int numberOfStars;
    float distance;

    public float rotationSpeed = 2.0f;
    public float scaleSpeed = 0.05f;

    [VectorLabels("Min", "Max")]
    public Vector2 scale = new Vector2(1.0f, 2.0f);

    bool scaleDir;

    // Start is called before the first frame update
    void Start()
    {
        spawnStarsSpiral();
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
            GameObject tempStar = Instantiate(assets.star, transform);
            tempStar.transform.localPosition = pos;
            tempStar.transform.GetChild(0).GetComponent<MeshRenderer>().material = assets.starMaterial;
            stars.Add(tempStar);
        }
        //Continue
    }

    private void Update()
    {
        float newRotationY = (transform.rotation.eulerAngles.y > 360.0f) ? 0 : transform.rotation.eulerAngles.y + (Time.deltaTime * rotationSpeed);
        transform.eulerAngles = new Vector3(transform.rotation.x, newRotationY, transform.rotation.z);

        if (transform.localScale.x > scale.y)
            scaleDir = false;
        else if (transform.localScale.x < scale.x)
            scaleDir = true;

        float scaleModifier = 0.0f;
        if (transform.parent)
            scaleModifier = transform.parent.localScale.y;

        float newScale = (!scaleDir) ? transform.localScale.x - (Time.deltaTime * scaleSpeed) : transform.localScale.x + (Time.deltaTime * scaleSpeed);
        transform.localScale = new Vector3(newScale, newScale * (scaleModifier * 2), newScale);
    }

}
