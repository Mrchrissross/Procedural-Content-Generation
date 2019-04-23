using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
    [System.Serializable]
    public class WorldObjects
    {
        public GameObject terrain;
        public GameObject ships;
        public GameObject birds;
        public GameObject fish;
        public GameObject stoneTower;
        public GameObject japanTower;
        public GameObject house;
        public GameObject tree;
        public GameObject palm;
        public GameObject rock;
    };

    [System.Serializable]
    public class Fleets
    {
        public int numberOfShips = 100;
        public Vector3 fleetPosition;
        public Vector2 spawnDistance = new Vector2(-100.0f, 100.0f);
        public bool randomSailColour = false;
        public Color sailColour;
    };

    public WorldObjects worldObjects;

    [Header("Terrain")]
    public LayerMask noWater;
    [VectorLabels("Min", "Max")]
    public Vector2 seaLevel = new Vector2(19.0f, 26.0f);

    [Header("Ships")]
    public Fleets[] fleets;

    [Header("Birds")]
    public bool generateBirds = true;
    public int numberOfBirds = 50;

    [Header("Fish")]
    public bool generateFish = true;
    public int numberOfFish = 50;

    [Header("Stone Tower")]
    public bool generateStoneTower = true;

    [Header("Japanese Tower")]
    public bool generateJapaneseTower = true;
    public int numberOfTowers = 30;

    [Header("Houses")]
    public bool generateHouses = true;
    public int numberOfHouses = 100;

    [Header("Trees")]
    public bool generateTrees = true;
    public int numberOfTrees = 50;

    [Header("Palm Trees")]
    public bool generatePalmTrees = true;
    public int numberOfPalms = 100;

    [Header("Rocks")]
    public bool generateRocks = true;
    public int numberOfRocks = 400;

    GameObject terrain;

    // Start is called before the first frame update
    void Start()
    {
        // Terrain
        terrain = Instantiate(worldObjects.terrain);
        terrain.GetComponent<GenerateTerrain>().seaLevel = new Vector2(seaLevel.x, seaLevel.y);
        terrain.GetComponent<GenerateTerrain>().Generate();

        // Boids
        GameObject temp;

        foreach(Fleets fleet in fleets)
        {
            temp = Instantiate(worldObjects.ships);
            temp.transform.position = fleet.fleetPosition;
            temp.GetComponent<GenerateBoids>().distance = fleet.spawnDistance;
            temp.GetComponent<GenerateBoids>().numberOfBoids = fleet.numberOfShips;
            temp.GetComponent<GenerateBoids>().randomSailColour = fleet.randomSailColour;
            temp.GetComponent<GenerateBoids>().sailColour = fleet.sailColour;
        }

        if(generateBirds)
        { 
            temp = Instantiate(worldObjects.birds);
            temp.GetComponent<GenerateBoids>().numberOfBoids = numberOfBirds;
        }

        if(generateFish)
        {
            temp = Instantiate(worldObjects.fish);
            temp.GetComponent<GenerateBoids>().numberOfBoids = numberOfFish;
        }

        // Stone Tower
        if(generateStoneTower)
        {
            temp = Instantiate(worldObjects.stoneTower);
            temp.transform.position = new Vector3(0, 100, 0);
            temp.GetComponent<GenerateStoneTower>().Generate();
            FindGround(temp, 5.0f, false);
        }

        // Japan Towers
        if(generateJapaneseTower)
        {
            temp = new GameObject();
            temp.name = "Japanese Towers";
            for (int i = 0; i < numberOfTowers; i++)
            {
                GameObject temp2 = Instantiate(worldObjects.japanTower);
                temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
                temp2.transform.eulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0.0f);
                temp2.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                temp2.GetComponent<GenerateJapanTower>().Generate();
                temp2.transform.parent = temp.transform;
                FindGround(temp2, 0.0f, true);
            }
        }

        // Houses
        if (generateHouses)
        {
            temp = new GameObject();
            temp.name = "Houses";
            for (int y = 0; y < numberOfHouses; y++)
            {
                GameObject temp2 = Instantiate(worldObjects.house, temp.transform);
                temp2.transform.localPosition = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
                temp2.transform.eulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0.0f);
                temp2.GetComponent<GenerateHouse>().Generate();
                FindGround(temp2, 0.0f, true);
            }
        }

        // Trees
        if (generateTrees)
        {
            temp = new GameObject();
            temp.name = "Trees";
            for (int i = 0; i < numberOfTrees; i++)
            {
                GameObject temp2 = Instantiate(worldObjects.tree);
                temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
                temp2.GetComponent<GenerateTree>().Generate();
                temp2.transform.parent = temp.transform;
                FindGround(temp2, 0.0f, true);
            }
        }

        // Palm Trees
        if(generatePalmTrees)
        {
            temp = new GameObject();
            temp.name = "Palm Trees";
            for (int i = 0; i < numberOfPalms; i++)
            {
                GameObject temp2 = Instantiate(worldObjects.palm);
                temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
                temp2.GetComponent<GeneratePalmTree>().randomExtraTrunk = (Random.value > 0.5f);
                temp2.GetComponent<GeneratePalmTree>().randomExtraLeaves = (Random.value > 0.5f);
                temp2.GetComponent<GeneratePalmTree>().Generate();
                temp2.transform.parent = temp.transform;
                FindGround(temp2, 0.0f, true);
            }
        }

        // Rocks
        if(generateRocks)
        {
            temp = new GameObject();
            temp.name = "Rocks";
            for (int i = 0; i < numberOfRocks; i++)
            {
                float scale = Random.Range(1.0f, 6.0f);
                GameObject temp2 = Instantiate(worldObjects.rock);
                temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
                temp2.GetComponent<GenerateRock>().Generate();
                temp2.transform.parent = temp.transform;
                temp2.transform.localScale = new Vector3(scale, scale, scale);
                FindGround(temp2, 0.0f, false);
            }
        }
    }

    private void FindGround(GameObject _prefab, float offset, bool landBased)
    {
        RaycastHit hit;

        if (Physics.Raycast(_prefab.transform.position, -_prefab.transform.up, out hit, 100, noWater))
            _prefab.transform.position = new Vector3(hit.point.x, hit.point.y + offset, hit.point.z);

        if (_prefab.transform.position.y < terrain.GetComponent<GenerateTerrain>().waterHeight + 0.5f && landBased)
            DestroyImmediate(_prefab);
    }
}
