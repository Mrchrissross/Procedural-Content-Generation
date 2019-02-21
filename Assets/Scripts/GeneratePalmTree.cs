using UnityEngine;
using System.Collections.Generic;

public class GeneratePalmTree : MonoBehaviour
{
    [System.Serializable]
    public class TowerParts
    {
        public GameObject Empty;
        public GameObject Trunk;
        public GameObject Leaves;
    };

    public TowerParts parts;

    public bool extraLeaves;

    private GameObject tempEmpty;
    private GameObject tempTrunk;
    private GameObject tempLeaves;

    private List<GameObject> bones = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    //void Start()
    //{
    //  Generate();
    //}

    public void Generate()
    {
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
            DestroyObject();

            this.name = "PalmTree";
            tempEmpty.name = "Palm";
            tempEmpty.transform.position = new Vector3(0, 0, 0);
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
        tempLeaves = Instantiate(parts.Leaves);
        if (tempLeaves)
        {
            float leavesSize = Random.Range(0.85f, 1.15f);
            float leavesRotation = Random.Range(0.0f, 360.0f);

            tempLeaves.name = "Leaves";
            tempLeaves.transform.localScale = new Vector3(leavesSize, leavesSize, leavesSize);
            tempLeaves.transform.eulerAngles = new Vector3(0, leavesRotation, 0);

            tempLeaves.transform.parent = spawnPoints[spawnPoints.Count - 1].transform;
            tempLeaves.transform.localPosition = Vector3.zero;
        }
        else
            Debug.LogError("The leaves prefab has not been set on " + this.name + " object.");

        if(extraLeaves)
        {
            for (int i = 0; i < 2; i++)
            {
                int truFalse = Random.Range(0, 2);

                if (truFalse == 0)
                    continue;

                tempLeaves = Instantiate(parts.Leaves);
                if (tempLeaves)
                {
                    float leavesSize = Random.Range(0.5f, 0.8f);
                    float leavesRotation = Random.Range(0.0f, 360.0f);

                    tempLeaves.name = "Leaves";
                    tempLeaves.transform.localScale = new Vector3(leavesSize, leavesSize, leavesSize);
                    tempLeaves.transform.eulerAngles = new Vector3(0, leavesRotation, 0);

                    tempLeaves.transform.parent = spawnPoints[i].transform;
                    tempLeaves.transform.localPosition = Vector3.zero;
                }
                else
                    Debug.LogError("The leaves prefab has not been set on " + this.name + " object.");
            }
        }
    }

    public void DestroyObject()
    {
        if (transform.Find("Palm"))
        {
            DestroyImmediate(GameObject.Find("Palm").gameObject);
            this.name = "SpawnPalmTree";
        }

        bones.Clear();
        spawnPoints.Clear();
    }
}
