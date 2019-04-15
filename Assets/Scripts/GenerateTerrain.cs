using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class GenerateTerrain : MonoBehaviour
{

    public bool generateWater;

    public Terrain terrain;
    Terrain body;
    public GameObject water;
    GameObject sea;

    [VectorLabels("Min", "Max")]
    public Vector2 seaLevel = new Vector2(19.0f, 26.0f);
    [HideInInspector]
    public float waterHeight;

    //The frequency of the noise
    public float frequency = 1f;
    
    //The lacunarity of the noise
    public float lacu = 1f;
    
    //The number of octaves
    public int octaves = 1;
    
    //The persistance of the noise
    public float persist = 1f;

    public void Generate()
    {
        DestroyObject();

        GenerateObject();
    }

    void GenerateObject()
    {
        body = Instantiate(terrain);
        if (body)
        {
            this.name = "Terrain";
            body.transform.parent = transform;
            body.name = "body";
            body.transform.localPosition = new Vector3(-250.0f, 0.0f, -250.0f);
        }
        else
        {
            Debug.LogError("Failed to created body on " + this.name + " object.");
            return;
        }

        if(generateWater)
        {
            sea = Instantiate(water);
            if (sea)
            {
                sea.transform.parent = transform;
                sea.name = "water";
                waterHeight = Random.Range(seaLevel.x, seaLevel.y);
                sea.transform.localPosition = new Vector3(0.0f, waterHeight, 0.0f);
            }
            else
            {
                Debug.LogError("Failed to created water on " + this.name + " object.");
                return;
            }
        }

        //Get the terrain data of the currently active terrain
        var terrainData = Terrain.activeTerrain.terrainData;

        #region One Generator

            ////Create a new ridged multifractal generator with the settings, and a random seed
            //var generator = new RidgedMultifractal(frequency, lacu, octaves, Random.Range(0, 0xffffff), QualityMode.High);

            ////Create a 2D noise generator for the terrain heightmap, using the generator we just created
            //var noise = new Noise2D(terrainData.heightmapResolution, generator);

        #endregion

        #region Scaling

            ////Create a new perlin noise generator with the given settings.
            //var perlinGenerator = new Perlin(frequency, lacu, persist, octaves, Random.Range(0, 0xffffff), QualityMode.High);

            ////Create a constant value generator - every sampled point will be 0.8.
            //var constGenerator = new Const(0.8f);

            ////Combine the perlin noise generator and the const generator through multiplication -(perlin[x, y] * 0.8) for all x, y
            //var mixedGenerator = new Multiply(perlinGenerator, constGenerator);

            ////Create a new noise map
            //var noise = new Noise2D(terrainData.heightmapWidth, terrainData.heightmapHeight, mixedGenerator);

        #endregion

        #region Adding Two Functions

            //To scale each point by half
            var constGenerator = new Const(0.3f);
            
            //Create a new perlin noise generator with the given settings.
            var perlinGenerator = new Perlin(frequency, lacu, persist, octaves, Random.Range(0, 0xffffff), QualityMode.High);
            
            //Create a second perlin noise generator with some different settings.
            var perlinGenerator2 = new Perlin(frequency * 2, lacu, persist, octaves, Random.Range(0, 0xffffff) + 1, QualityMode.High);
            
            //Add together both generators scaled by 0.5f (so they add up to 1)
            var finalGenerator = new Add(new Multiply(perlinGenerator, constGenerator), new Multiply(perlinGenerator2, constGenerator));

            var constGenerator2 = new Const(0.8f);

            var finalGenerator2 = new Multiply(constGenerator2, finalGenerator);

            //Create a new noise map
            var noise = new Noise2D(terrainData.heightmapWidth, terrainData.heightmapHeight, finalGenerator2);

        #endregion

        //Generate a plane from [0, 1] on x, [0, 1] on y
        noise.GeneratePlanar(0, 1, 0, 1);

        //Get the data in an array so we can use it to set the heights
        var data = noise.GetData(true, 0, 0, true);

        //.. and actually set the heights
        terrainData.SetHeights(0, 0, data);

    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            objectsToDelete.Add(transform.GetChild(i).gameObject);
        }

        foreach(GameObject item in objectsToDelete)
            DestroyImmediate(item);

        this.name = "SpawnTerrain";
    }
}
