using System.Collections.Generic;
using UnityEngine;

public class GenerateStoneTower : MonoBehaviour
{
    [System.Serializable]
    public class TowerParts
    {
        public GameObject Base;
        public GameObject Tower;
        public GameObject FloatingRock1;
        public GameObject FloatingRock2;
        public GameObject FloatingRock3;
        public GameObject Galaxy;
        public GameObject Rock;
    };

    public TowerParts parts;

    private GameObject tempBase;
    private GameObject TempTower;
    private GameObject TempGalaxy;

    Transform flyingRockHolder;
    Transform floatingRockHolder;

    [Header("Rocks")]
    List<GameObject> flyingRocks = new List<GameObject>();
    List<float> flyingRocksSpeeds = new List<float>();
    List<GameObject> floatingSideRocks = new List<GameObject>();

    [Header("Tower"), VectorLabels("Min", "Max")]
    public Vector2 size = new Vector2(3.0f, 5.0f);

    [VectorLabels("Min", "Max")]
    public Vector2 height = new Vector2(0.5f, 2.5f);

    [Range(0.05f, 1.0f)]
    public float sidePulseSpeed = 0.1f;

    [Header("Clouds"), VectorLabels("Min", "Max")]
    public Vector2 orbScale = new Vector2(1.0f, 2.0f);

    [Range(-30.0f, 30.0f)]
    public float orbsRotationSpeed = 4.0f;

    [Range(0.01f, 0.2f)]
    public float orbsScaleSpeed = 0.05f;

    [Header("Storm")]
    [VectorLabels("Min", "Max")]
    public Vector2 galaxyGrowth = new Vector2(0.6f, 1.0f);

    [Range(0.01f, 2.0f)]
    public float galaxyGrowthSpeed = 1.0f;

    private GenerateGalaxy generateGalaxy;

    [Header("Flying Rocks"), VectorLabels("Min", "Max")]
    public Vector2Int rockCount = new Vector2Int(4, 10);

    [VectorLabels("Min", "Max")]
    public Vector2 rockRotation = new Vector2(0.5f, 7.0f);

    [Range(-150.0f, 150.0f)]
    public float roundRotation = 4.0f;

    [Range(5.0f, 15.0f)]
    public float distance = 15.0f;

    bool sideRocksExpand;
    bool growStorm;

    private void Start()
    {
        if(transform.Find("tower"))
        {
            flyingRockHolder = transform.Find("tower").Find("FlyingRockHolders");
            floatingRockHolder = transform.Find("tower").Find("FloatingRockHolders");

            floatingSideRocks.Clear();
            flyingRocks.Clear();

            foreach (Transform child in floatingRockHolder)
            {
                if (child.childCount > 0)
                    floatingSideRocks.Add(child.GetChild(0).gameObject);
            }

            foreach (Transform child in flyingRockHolder)
            {
                flyingRocks.Add(child.gameObject);

                float rotationSpeed = Random.Range(rockRotation.x, rockRotation.y);
                bool direction = (Random.value > 0.5f);
                rotationSpeed = (direction) ? rotationSpeed : -rotationSpeed;
                flyingRocksSpeeds.Add(rotationSpeed);
            }

            generateGalaxy = transform.Find("tower").Find("SpawnGalaxy").GetChild(0).GetComponent<GenerateGalaxy>();
        }
    }

    public void Generate()
    {
        GenerateBase();
        GenerateSideRocks();
        GenerateFlyingRocks();
    }

    void GenerateBase()
    {
        DestroyObject();

        tempBase = Instantiate(parts.Base, transform);
        if (tempBase)
        {
            float baseSize = Random.Range(size.x, size.y);
            this.name = "StoneTower";
            transform.localScale = new Vector3(baseSize, baseSize, baseSize);
            tempBase.name = "base";
            tempBase.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("Failed to created base on " + this.name + " object.");
            DestroyObject();
            return;
        }

        TempTower = Instantiate(parts.Tower, transform);
        if (TempTower)
        {
            TempTower.name = "tower";
            TempTower.transform.localPosition = Vector3.zero;

            float towerHeight = Random.Range(height.x, height.y);
            TempTower.transform.localScale = new Vector3(1.0f, 1.0f, towerHeight);
        }
        else
        {
            Debug.LogError("Failed to created tower on " + this.name + " object.");
            DestroyObject();
            return;
        }

        TempGalaxy = Instantiate(parts.Galaxy, transform.Find("tower").Find("SpawnGalaxy"));
        if (TempGalaxy)
        {
            TempGalaxy.name = "galaxy";
            TempGalaxy.transform.localPosition = Vector3.zero;
            TempGalaxy.transform.localScale = new Vector3(orbScale.y / 2, orbScale.y / 2, orbScale.y / 2);
            generateGalaxy = TempGalaxy.GetComponent<GenerateGalaxy>();
        }
        else
        {
            Debug.LogError("Failed to created galaxy on " + this.name + " object.");
            DestroyObject();
            return;
        }
    }

    void GenerateSideRocks()
    {
        floatingRockHolder = transform.Find("tower").Find("FloatingRockHolders");

        foreach (Transform child in floatingRockHolder)
        {
            bool spawn = (Random.value > 0.5f);

            if (spawn)
            {
                GameObject rock = null;
                int rockPrefab = Random.Range(1, 4);

                switch (rockPrefab)
                {
                    case 1:
                        rock = Instantiate(parts.FloatingRock1, child);
                        break;
                    case 2:
                        rock = Instantiate(parts.FloatingRock2, child);
                        break;
                    case 3:
                        rock = Instantiate(parts.FloatingRock3, child);
                        break;
                }

                floatingSideRocks.Add(rock);
                int rotation = Random.Range(0, 4);

                switch (rotation)
                {
                    case 0:
                        rock.transform.localEulerAngles = Vector3.zero;
                        break;
                    case 1:
                        rock.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                        break;
                    case 2:
                        rock.transform.localEulerAngles = new Vector3(180.0f, 0.0f, 0.0f);
                        break;
                    case 3:
                        rock.transform.localEulerAngles = new Vector3(270.0f, 0.0f, 0.0f);
                        break;
                }
            }
        }
    }

    void GenerateFlyingRocks()
    {
        int count = Random.Range(rockCount.x, rockCount.y);

        flyingRockHolder = transform.Find("tower").Find("FlyingRockHolders");

        for(int i = 0; i < count; i++)
        {
            float distanceX = Random.Range(-distance, distance);
            float distanceY = Random.Range(-distance, distance);
            float height = Random.Range(-5.0f, 2.0f);

            if (distanceX < 10.0f && distanceX < -10.0f)
            {
                if(distanceX < 0)
                    distanceX = Random.Range(-distance, -3.0f);
                else
                    distanceX = Random.Range(3.0f, distance);
            }

            if (distanceY < 10.0f && distanceY < -10.0f)
            {
                if (distanceY < 0)
                    distanceY = Random.Range(-distance, -10.0f);
                else
                    distanceY = Random.Range(10.0f, distance);
            }

            GameObject rock = Instantiate(parts.Rock, flyingRockHolder);

            rock.transform.localPosition = new Vector3(distanceX, distanceY, height);

            // Change the maximum x scale
            rock.GetComponent<GenerateRock>()._x.y = 0.55f;
            // Change the maximum z scale
            rock.GetComponent<GenerateRock>()._z.y = 0.55f;

            rock.GetComponent<GenerateRock>().Generate();

            rock.GetComponent<GenerateRock>().body.GetComponent<MeshRenderer>().material = TempTower.transform.Find("Tower").GetComponent<MeshRenderer>().sharedMaterials[1];

            float rotationSpeed = Random.Range(rockRotation.x, rockRotation.y);
            bool direction = (Random.value > 0.5f);
            rotationSpeed = (direction) ? rotationSpeed : -rotationSpeed;
            flyingRocksSpeeds.Add(rotationSpeed);

            flyingRocks.Add(rock);
        }
    }

    private void Update()
    {
        if (transform.childCount < 1)
            return;

        if(generateGalaxy)
        {
            if(generateGalaxy.scale.x != orbScale.x)
                generateGalaxy.scale.x = orbScale.x;
            if(generateGalaxy.scale.y != orbScale.y)
                generateGalaxy.scale.y = orbScale.y;
            if (generateGalaxy.rotationSpeed != orbsRotationSpeed)
                generateGalaxy.rotationSpeed = orbsRotationSpeed;
            if (generateGalaxy.scaleSpeed != orbsScaleSpeed)
                generateGalaxy.scaleSpeed = orbsScaleSpeed;
        }

        foreach (GameObject rock in floatingSideRocks)
        {
            if (rock.transform.localPosition.x < -0.35f)
                sideRocksExpand = false;
            if (rock.transform.localPosition.x > 0.0f)
                sideRocksExpand = true;

            if (sideRocksExpand)
                rock.transform.localPosition = new Vector3(rock.transform.localPosition.x - (Time.deltaTime * sidePulseSpeed), 0, 0);
            else
                rock.transform.localPosition = new Vector3(rock.transform.localPosition.x + (Time.deltaTime * sidePulseSpeed), 0, 0);
        }

        for(int i = 0; i < flyingRocks.Count; i++)
        {
            if (flyingRocks[i].transform.GetChild(0).localEulerAngles.z > 360.0f)
                flyingRocks[i].transform.GetChild(0).localEulerAngles = Vector3.zero;

            flyingRocks[i].transform.GetChild(0).localEulerAngles = new Vector3(0, 0, flyingRocks[i].transform.GetChild(0).localEulerAngles.z + (Time.deltaTime * flyingRocksSpeeds[i]));
        }

        flyingRockHolder.localEulerAngles = new Vector3(
                        flyingRockHolder.localEulerAngles.x, 
                        flyingRockHolder.localEulerAngles.y,
                        flyingRockHolder.localEulerAngles.z + Time.deltaTime * roundRotation);

        if(flyingRockHolder.localEulerAngles.y < 360.0f)
            flyingRockHolder.localEulerAngles = new Vector3(
                        flyingRockHolder.localEulerAngles.x,
                        flyingRockHolder.localEulerAngles.y,
                        flyingRockHolder.localEulerAngles.z - 360.0f);
        
        if(!growStorm)
        {
            Vector3 galScale = TempGalaxy.transform.parent.localScale;
            galScale.x -= Time.deltaTime * galaxyGrowthSpeed;
            TempGalaxy.transform.parent.localScale = galScale;

            if (TempGalaxy.transform.parent.localScale.x < galaxyGrowth.x)
                growStorm = true;
        }
        else
        {
            Vector3 galScale = TempGalaxy.transform.parent.localScale;
            galScale.x += Time.deltaTime * galaxyGrowthSpeed;
            TempGalaxy.transform.parent.localScale = galScale;

            if (TempGalaxy.transform.parent.localScale.x > galaxyGrowth.y)
                growStorm = false;
        }
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            objectsToDelete.Add(transform.GetChild(i).gameObject);

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        floatingSideRocks.Clear();
        flyingRocks.Clear();
        flyingRocksSpeeds.Clear();

        this.name = "SpawnStoneTower";
    }
}