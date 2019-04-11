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
        public Shader SailShader;
        public Material SailMaterial;
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

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        // Reset the rotation of the object during generation.
        Vector3 oldRotation = transform.eulerAngles;
        transform.eulerAngles = Vector3.zero;

        // Build the base of the ship.
        GenerateShip();
        // Generate all the masts.
        GenerateMast();

        // If we're building a ship, install some stairs.
        if(!boat)
            GenerateStairs();

        // Place some lamps on there.
        GenerateLamps();

        // Install all the windows.
        GenerateWindows();

        // Randomise the scale of the object.
        float scale = Random.Range(0.8f, 1.5f);
        transform.localScale = new Vector3(scale, scale, scale);

        // Finally set the object rotation back to how it was originally.
        transform.eulerAngles = oldRotation;
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
                this.name = "PirateBoat";
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
        Color randomColour = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        if (boat)
        {
            smallMast = Instantiate(parts.SmallMast, transform);
            if (smallMast)
            {
                float boatMastSize = Random.Range(1.0f, 1.25f);

                smallMast.name = "Mast";
                smallMast.transform.localPosition = Vector3.zero;
                smallMast.transform.localScale = new Vector3(boatMastSize * 1.2f, boatMastSize, 1.0f);

                ChangeSailColour(randomColour, smallMast.transform.GetChild(0).GetChild(0).gameObject);
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
                smallMast.name = "Mast";
                smallMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnSmallMast");
                smallMast.transform.localPosition = Vector3.zero;
                smallMast.transform.localScale = new Vector3(smallerMastSize, smallerMastSize, 1.0f);

                ChangeSailColour(randomColour, smallMast.transform.GetChild(0).GetChild(0).gameObject);
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
                largeMast.name = "Mast";
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.localScale = new Vector3(smallerMastSize, smallerMastSize, smallerMastSize);

                GameObject jib = front.transform.GetChild(0).Find("Jib").gameObject;
                GameObject spinakker = front.transform.GetChild(0).Find("Spinakker").gameObject;

                ChangeSailColour(randomColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
                ChangeSailColour(randomColour, jib);
                ChangeSailColour(randomColour, spinakker);

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
                largeMast.name = "Mast";
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.localScale = new Vector3(size, size, 1.0f);

                GameObject jib = front.transform.GetChild(0).Find("Jib").gameObject;
                GameObject spinakker = front.transform.GetChild(0).Find("Spinakker").gameObject;

                ChangeSailColour(randomColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
                ChangeSailColour(randomColour, jib);
                ChangeSailColour(randomColour, spinakker);
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
                largeMast.name = "Mast";
                largeMast.transform.parent = transform.GetChild(0).GetChild(0).Find("SpawnLargeMast");
                largeMast.transform.localPosition = Vector3.zero;
                largeMast.transform.localScale = new Vector3(size, size, 1.0f);
                largeMast.transform.localPosition = new Vector3(largeMast.transform.localPosition.x, largeMast.transform.localPosition.y + 1.0f, largeMast.transform.localPosition.z);

                ChangeSailColour(randomColour, largeMast.transform.GetChild(0).GetChild(0).gameObject);
            }
            else
            {
                Debug.LogError("Failed to create smaller mast on " + this.name + " object.");
                DestroyObject();
                return;
            }
        }
    }


    void ChangeSailColour(Color randomColour, GameObject sail)
    {
        parts.SailMaterial = new Material(parts.SailShader);
        parts.SailMaterial.color = randomColour;
        parts.SailMaterial.name = "SailMaterial";

        sail.GetComponent<MeshRenderer>().material = parts.SailMaterial;
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

    void GenerateWindows()
    {
        if(boat)
        {
            for (int i = 0; i < front.transform.GetChild(0).Find("Windows").childCount; i++)
                DestroyImmediate(front.transform.GetChild(0).Find("Windows").GetChild(i).gameObject);
            for (int i = 0; i < back.transform.GetChild(0).Find("Windows").childCount; i++)
                DestroyImmediate(back.transform.GetChild(0).Find("Windows").GetChild(i).gameObject);
        }

        for (int i = 0; i < front.transform.GetChild(0).Find("Windows").childCount; i++)
        {
            bool despawn = (Random.value > 0.5f);

            if (despawn)
                DestroyImmediate(front.transform.GetChild(0).Find("Windows").GetChild(i).gameObject);
        }

        for (int i = 0; i < back.transform.GetChild(0).Find("Windows").childCount; i++)
        {
            bool despawn = (Random.value > 0.5f);

            if (despawn)
                DestroyImmediate(back.transform.GetChild(0).Find("Windows").GetChild(i).gameObject);
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
