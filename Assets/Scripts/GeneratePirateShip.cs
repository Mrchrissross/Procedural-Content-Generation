using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePirateShip : MonoBehaviour
{
    [System.Serializable]
    public class PirateShipParts
    {
        public GameObject Front;
        public GameObject Back;
        public GameObject Window;
        public GameObject Oar;
        public GameObject Lamp;
        public GameObject SmallMast;
        public GameObject LargeMast;
    };

    public PirateShipParts parts;

    private GameObject front;
    private GameObject back;
    private GameObject window;
    private GameObject oar;
    private GameObject lamp;
    private GameObject smallMast;
    private GameObject largeMast;

    bool boat;

    public void Generate()
    {
        GenerateShip();
        GenerateMast();
    }

    void GenerateShip()
    {
        DestroyObject();

        boat = (Random.value > 0.5f);

        front = Instantiate(parts.Front, transform);
        if (front)
        {
            float size = Random.Range(0.9f, 1.2f);

            this.name = "PirateShip";
            front.name = "Front";
            front.transform.localPosition = Vector3.zero;
            front.transform.localScale = new Vector3(1.0f, 1.0f, size);

            if (boat)
                DestroyImmediate(front.transform.GetChild(0).Find("Pole").gameObject);
        }
        else
        {
            Debug.LogError("Failed to create front on " + this.name + " object.");
            DestroyObject();
            return;
        }

        if(boat)
        {
            back = Instantiate(parts.Front, transform);
            if (back)
            {
                float size = Random.Range(0.9f, 1.2f);

                back.name = "Back";
                back.transform.GetChild(0).name = "Back";
                back.transform.localPosition = Vector3.zero;
                back.transform.eulerAngles = new Vector3(0, 180, 0);
                back.transform.localScale = new Vector3(1.0f, 1.0f, size);

                DestroyImmediate(back.transform.GetChild(0).Find("Anchor").gameObject);
                DestroyImmediate(back.transform.GetChild(0).Find("Pole").gameObject);
            }
            else
            {
                Debug.LogError("Failed to create back on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
        else
        {
            back = Instantiate(parts.Back, transform);
            if (back)
            {
                float size = Random.Range(1.0f, 1.3f);

                back.name = "Back";
                back.transform.localPosition = Vector3.zero;
                back.transform.localScale = new Vector3(1.0f, 1.0f, size);
            }
            else
            {
                Debug.LogError("Failed to create back on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
    }

    void GenerateMast()
    {
        float smallerMastSize = Random.Range(0.8f, 1.2f);

        if(boat)
        {
            smallMast = Instantiate(parts.SmallMast, transform);
            if (smallMast)
            {
                this.name = "PirateShip";
                smallMast.name = "Mast";
                smallMast.transform.localPosition = Vector3.zero;
                smallMast.transform.localScale = new Vector3(smallerMastSize, smallerMastSize, 1.0f);
            }
            else
            {
                Debug.LogError("Failed to create mast on " + this.name + " object.");
                DestroyObject();
                return;
            }

            return;
        }

        bool smallerMast = (Random.value > 0.5f);

        if(smallerMast)
        {
            smallMast = Instantiate(parts.SmallMast);
            if (smallMast)
            {
                this.name = "PirateShip";
                smallMast.name = "Mast";
                smallMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnSmallMast");
                smallMast.transform.localPosition = Vector3.zero;
                smallMast.transform.localScale = new Vector3(smallerMastSize, smallerMastSize, 1.0f);
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }

        bool largerMast = (Random.value > 0.5f);

        if(largerMast)
        {
            largeMast = Instantiate(parts.LargeMast);
            if (largeMast)
            {
                this.name = "PirateShip";
                largeMast.name = "Mast";
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.localScale = new Vector3(smallerMastSize, smallerMastSize, smallerMastSize);
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
        else
        {
            float size = Random.Range(1.1f, 1.5f);

            largeMast = Instantiate(parts.SmallMast);
            if (largeMast)
            {
                this.name = "PirateShip";
                largeMast.name = "Mast";
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.localScale = new Vector3(size, size, 1.0f);
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            objectsToDelete.Add(transform.GetChild(i).gameObject);

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        this.name = "SpawnPirateShip";
    }
}
