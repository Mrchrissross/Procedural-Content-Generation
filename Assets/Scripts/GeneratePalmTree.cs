using UnityEngine;
using System.Collections.Generic;

public class GeneratePalmTree : MonoBehaviour
{
    [System.Serializable]
    public class TowerParts
    {
        public GameObject Empty;
        public GameObject Trunk;
        public GameObject Leaf;
    };

    public TowerParts parts;

    public bool randomExtraTrunk;
    public bool randomExtraLeaves;

    private GameObject tempEmpty;
    private GameObject tempTrunk;
    private GameObject tempLeaf;

    private List<GameObject> bones = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    //void Start()
    //{
        //Generate();
    //}

    public void Generate()
    {
        DestroyObject();

        GenerateBase();

        int truFalse = Random.Range(0, 2);

        if (truFalse == 0 && randomExtraTrunk)
            GenerateBase();
    }

    void GenerateBase()
    {
        GenerateTrunk();

        SortChildren();

        ModifyTrunk();

        GenerateLeaves();
    }

    void GenerateTrunk()
    {
        tempEmpty = Instantiate(parts.Empty, this.transform);
        if (tempEmpty)
        {
            this.name = "PalmTree";
            tempEmpty.name = "Palm";
            tempEmpty.transform.localPosition = Vector3.zero;
        }
        else
            Debug.LogError("The empty prefab has not been set on " + this.name + " object.");

        tempTrunk = Instantiate(parts.Trunk, tempEmpty.transform);
        if (tempTrunk)
            tempTrunk.name = "Trunk";
        else
            Debug.LogError("The trunk prefab has not been set on " + this.name + " object.");
    }

    void SortChildren()
    {
        foreach (Transform child in tempTrunk.GetComponentsInChildren<Transform>(true))
        {
            bones.Add(child.gameObject);
        }

        for (int i = 0; i < bones.Count; i++)
        {
            if (bones[i].name == "Trunk")
                bones.Remove(bones[i]);

            if (bones[i].name == "Bone")
                bones.Remove(bones[i]);

            if (bones[i].name == "LeavesSpawn")
            {
                spawnPoints.Add(bones[i]);
                bones.Remove(bones[i]);
            }
        }
    }

    void ModifyTrunk()
    {
        float   trunkHeight = Random.Range(0.8f, 1.5f),
                trunkWidth = Random.Range(0.7f, 1.2f);

        tempTrunk.transform.localScale = new Vector3(trunkWidth, trunkWidth, trunkHeight);

        float   offsetY = Random.Range(-0.15f, 0.15f),
                offsetZ = Random.Range(-0.15f, 0.15f);

        foreach(GameObject bone in bones)
        {
            bone.transform.localPosition = new Vector3(bone.transform.localPosition.x, offsetY, offsetZ);
        }
    }

    void GenerateLeaves()
    {
        PositionLeaves(spawnPoints.Count - 1, new Vector2(0.85f, 1.15f));

        if (randomExtraLeaves)
        {
            for (int i = 0; i < 2; i++)
            {
                int truFalse = Random.Range(0, 2);

                if (truFalse == 0)
                    continue;

                PositionLeaves(i, new Vector2(0.5f, 0.8f));
            }
        }
    }

    void PositionLeaves(int spawnPoint, Vector2 scale)
    {
        tempEmpty = Instantiate(parts.Empty);
        if (tempEmpty)
        {
            tempEmpty.name = "Leaves";
            int numberOfLeaves = Random.Range(10, 14);

            for (int i = 0; i < numberOfLeaves; i++)
            {
                tempLeaf = Instantiate(parts.Leaf);
                if (tempLeaf)
                {
                    float leavesSize = Random.Range(scale.x, scale.y);
                    float leavesRotationX = Random.Range(-25.0f, 35.0f);
                    float leavesRotationY = Random.Range(0.0f, 360.0f);

                    tempLeaf.name = "Leaves";
                    tempLeaf.transform.localScale = new Vector3(leavesSize, leavesSize, leavesSize);
                    tempLeaf.transform.eulerAngles = new Vector3(leavesRotationX, leavesRotationY, 0);

                    tempLeaf.transform.parent = tempEmpty.transform;
                    tempLeaf.transform.localPosition = Vector3.zero;
                }
                else
                    Debug.LogError("The leaf prefab has not been set on " + this.name + " object.");
            }

            tempEmpty.transform.parent = spawnPoints[spawnPoint].transform;
            tempEmpty.transform.localPosition = Vector3.zero;
        }
        else
            Debug.LogError("The empty prefab has not been set on " + this.name + " object.");
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            objectsToDelete.Add(transform.GetChild(i).gameObject);
        }

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        this.name = "SpawnPalmTree";

        bones.Clear();
        spawnPoints.Clear();
    }
}
