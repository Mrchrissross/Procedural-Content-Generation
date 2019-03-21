using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePirateShip : MonoBehaviour
{
    [System.Serializable]
    public class PirateShipParts
    {
        [Header("ShipParts")]
        public GameObject Front;
        public GameObject Back;
        public GameObject Window;
        public GameObject Oar;
        public GameObject Lamp;
        public GameObject Stairs;
        public GameObject SmallMast;
        public GameObject LargeMast;
        [Header("Colours")]
        public Material colour1;
        public Material colour2;
        public Material colour3;
        public Material colour4;
        public Material colour5;
    };

    public PirateShipParts parts;

    private GameObject front;
    private GameObject back;
    private GameObject window;
    private GameObject oar;
    private GameObject lamp;
    private GameObject smallMast;
    private GameObject largeMast;

    private List<GameObject> stairs = new List<GameObject>();

    bool boat;

    public void Generate()
    {
        GenerateShip();
        GenerateMast();

        if(!boat)
            GenerateStairs();

        GenerateLamps();
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
            {
                DestroyImmediate(front.transform.GetChild(0).Find("Pole").gameObject);
                DestroyImmediate(front.transform.GetChild(0).Find("Jib").gameObject);
                DestroyImmediate(front.transform.GetChild(0).Find("Spinakker").gameObject);
            }
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
                DestroyImmediate(back.transform.GetChild(0).Find("Jib").gameObject);
                DestroyImmediate(back.transform.GetChild(0).Find("Spinakker").gameObject);
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
        int mastColour = Random.Range(0, 5);

        if(boat)
        {
            smallMast = Instantiate(parts.SmallMast, transform);
            if (smallMast)
            {
                float boatMastSize = Random.Range(1.0f, 1.25f);

                this.name = "PirateShip";
                smallMast.name = "Mast";
                smallMast.transform.localPosition = Vector3.zero;
                smallMast.transform.localScale = new Vector3(boatMastSize * 1.1f, boatMastSize, 1.0f);

                ChangeSailColour(mastColour, smallMast.transform.GetChild(0).GetChild(0).gameObject);
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

                ChangeSailColour(mastColour, smallMast.transform.GetChild(0).GetChild(0).gameObject);
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

                GameObject jib = front.transform.GetChild(0).Find("Jib").gameObject;
                GameObject spinakker = front.transform.GetChild(0).Find("Spinakker").gameObject;

                ChangeSailColour(mastColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
                ChangeSailColour(mastColour, jib);
                ChangeSailColour(mastColour, spinakker);

                if (!smallerMast)
                {
                    DestroyImmediate(jib);
                    DestroyImmediate(spinakker);
                }
                else
                {
                    int frontSail = Random.Range(0, 4);

                    if (frontSail == 0)
                        DestroyImmediate(jib);
                    else if (frontSail == 1)
                        DestroyImmediate(spinakker);
                    else if (frontSail == 2)
                    {
                        DestroyImmediate(jib);
                        DestroyImmediate(spinakker);
                    }
                }
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

                GameObject jib = front.transform.GetChild(0).Find("Jib").gameObject;
                GameObject spinakker = front.transform.GetChild(0).Find("Spinakker").gameObject;

                ChangeSailColour(mastColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
                ChangeSailColour(mastColour, jib);
                ChangeSailColour(mastColour, spinakker);
                if (!smallerMast)
                {
                    DestroyImmediate(jib);
                    DestroyImmediate(spinakker);
                }
                else
                {
                    int frontSail = Random.Range(0, 4);

                    if (frontSail == 0)
                        DestroyImmediate(jib);
                    else if (frontSail == 1)
                        DestroyImmediate(spinakker);
                    else if (frontSail == 2)
                    {
                        DestroyImmediate(jib);
                        DestroyImmediate(spinakker);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }

            largeMast = Instantiate(parts.LargeMast);
            if (largeMast)
            {
                this.name = "PirateShip";
                largeMast.name = "Mast";
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localScale = new Vector3(size, size, 1.0f);
                largeMast.transform.localPosition = new Vector3(largeMast.transform.localPosition.x, largeMast.transform.localPosition.y + 0.3f, largeMast.transform.localPosition.z);

                ChangeSailColour(mastColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
    }

    void ChangeSailColour(int colour, GameObject sail)
    {
        switch(colour)
        {
            case 1:
                sail.GetComponent<MeshRenderer>().material = parts.colour1;
                break;
            case 2:
                sail.GetComponent<MeshRenderer>().material = parts.colour2;
                break;
            case 3:
                sail.GetComponent<MeshRenderer>().material = parts.colour3;
                break;
            case 4:
                sail.GetComponent<MeshRenderer>().material = parts.colour4;
                break;
            case 5:
                sail.GetComponent<MeshRenderer>().material = parts.colour5;
                break;
        }
    }

    void GenerateStairs()
    {
        for(int i = 0; i < back.transform.GetChild(0).Find("Stairs").childCount; i++)
        {
            bool spawn = (Random.value > 0.5f);

            if(spawn)
                Instantiate(parts.Stairs, back.transform.GetChild(0).Find("Stairs").GetChild(i));
        }
    }

    void GenerateLamps()
    {
        for (int i = 0; i < front.transform.GetChild(0).Find("Lamps").childCount; i++)
        {
            bool spawn = (Random.value > 0.5f);

            if (spawn)
                Instantiate(parts.Lamp, front.transform.GetChild(0).Find("Lamps").GetChild(i));
        }

        bool extraBackLights = (Random.value > 0.5f);

        if (!extraBackLights)
            DestroyImmediate(back.transform.GetChild(0).Find("Lamps").GetChild(1).gameObject);
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
