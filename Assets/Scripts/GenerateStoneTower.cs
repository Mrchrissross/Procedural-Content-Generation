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
    };

    public TowerParts parts;

    private GameObject tempBase;
    private GameObject TempTower;
    private GameObject TempRock;
    private GameObject TempGalaxy;

    public List<GameObject> floatingSideRocks = new List<GameObject>();
    bool floatingSideRocksChecked;

    [Header("Tower")]
    public float minSize = 3.0f;
    public float maxSize = 5.0f;
    public float minHeight = 0.5f;
    public float maxHeight = 2.5f;
    public float sidePulseSpeed = 0.1f;

    [Header("Orbs")]
    public float orbsMinScale = 1.0f;
    public float orbsMaxScale = 2.0f;
    public float orbsRotationSpeed = 2.0f;
    public float orbsScaleSpeed = 0.05f;
    private GenerateGalaxy generateGalaxy;

    bool sideRocksExpand;

    private void Start()
    {
        if(transform.Find("tower"))
        {
            floatingSideRocks.Clear();

            foreach (Transform child in transform.Find("tower").Find("FloatingRockHolders"))
            {
                if (child.childCount > 0)
                    floatingSideRocks.Add(child.GetChild(0).gameObject);
            }

            generateGalaxy = transform.Find("tower").Find("SpawnGalaxy").GetChild(0).GetComponent<GenerateGalaxy>();
        }
    }

    public void Generate()
    {
        GenerateBase();
    }

    void GenerateBase()
    {
        DestroyObject();

        tempBase = Instantiate(parts.Base, transform);
        if (tempBase)
        {
            float size = Random.Range(minSize, maxSize);
            this.name = "StoneTower";
            transform.localScale = new Vector3(size, size, size);
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

            float height = Random.Range(minHeight, maxHeight);
            TempTower.transform.localScale = new Vector3(1.0f, 1.0f, height);
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
            TempGalaxy.transform.localScale = new Vector3(orbsMaxScale / 2, orbsMaxScale / 2, orbsMaxScale / 2);
        }
        else
        {
            Debug.LogError("Failed to created galaxy on " + this.name + " object.");
            DestroyObject();
            return;
        }

        Transform floatingRockHolder = transform.Find("tower").Find("FloatingRockHolders");

        foreach (Transform child in floatingRockHolder)
        {
            bool spawn = (Random.value > 0.5f);

            if(spawn)
            {
                GameObject rock = null;
                int rockPrefab = Random.Range(1, 4);

                switch(rockPrefab)
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

    private void Update()
    {
        if(generateGalaxy)
        {
            if(generateGalaxy.minScale != orbsMinScale)
                generateGalaxy.minScale = orbsMinScale;
            if(generateGalaxy.maxScale != orbsMaxScale)
                generateGalaxy.maxScale = orbsMaxScale;
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
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            objectsToDelete.Add(transform.GetChild(i).gameObject);

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        floatingSideRocks.Clear();

        this.name = "SpawnStoneTower";
    }
}