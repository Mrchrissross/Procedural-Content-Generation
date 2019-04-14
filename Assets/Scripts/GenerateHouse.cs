using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateHouse : MonoBehaviour
{
    [System.Serializable]
    public class HouseParts
    {
        [Header("ShipParts")]
        public GameObject Base;
        public GameObject Entrance;
        public GameObject Main;
        public GameObject Window;
    };

    public HouseParts parts;

    GameObject _base;
    GameObject _entrance;
    GameObject _main;
    GameObject _window;

    [Header("Base Size")]
    public Vector3 baseMinimum = new Vector3(1.8f, 1.8f, 1.8f);
    public Vector3 baseMaximum = new Vector3(2.2f, 2.2f, 2.2f);

    [Header("1st Layer Size")]
    public Vector3 firstMinimum = new Vector3(1.5f, 1.5f, 1.5f);
    public Vector3 firstMaximum = new Vector3(1.7f, 1.7f, 1.7f);

    public List<Transform> SpawnPoints = new List<Transform>();

    public void Generate()
    {
        DestroyObject();

        GenerateStructure();
    }

    void GenerateStructure()
    {
        this.name = "House";

        _base = SpawnObject(parts.Base, transform, baseMinimum, baseMaximum, "Base");

        for (int i = 0; i < _base.transform.childCount; i++)
        {
            if (_base.transform.GetChild(i).name == "SpawnPoint")
                SpawnPoints.Add(_base.transform.GetChild(i));
        }

        GenerateEntrance();

        GenerateRooms();
    }

    void GenerateEntrance()
    {
        bool extra = (Random.value > 0.5f);

        if(extra)
        {
            _main = SpawnObject(parts.Main, SpawnPoints[0], firstMinimum, firstMaximum, "LayerOne");
            _main.transform.eulerAngles = new Vector3(0, 180, 0);

            _entrance = SpawnObject(parts.Entrance, null, Vector3.one, Vector3.one, "Entrance");
            _entrance.transform.parent = _main.transform.Find("SpawnWindow");
            _entrance.transform.localPosition = new Vector3(0, 0, 0.0f - 0.7f);

            bool extra2 = (Random.value > 0.5f);

            if (extra2)
                _main = SpawnObject(parts.Main, SpawnPoints[0].GetChild(0), Vector3.one, Vector3.one, "LayerTwo");

            extra2 = (Random.value > 0.5f);

            if (extra2)
                _main = SpawnObject(parts.Main, SpawnPoints[0].GetChild(1), Vector3.one, Vector3.one, "LayerTwo");
        }
        else
        {
            for (int i = 0; i < SpawnPoints[0].childCount; i++)
                DestroyImmediate(SpawnPoints[0].GetChild(i).gameObject);

            _entrance = SpawnObject(parts.Entrance, null, Vector3.one, Vector3.one, "Entrance");
            _entrance.transform.parent = SpawnPoints[0];
            _entrance.transform.localPosition = Vector3.zero;
        }
    }

    void GenerateRooms()
    {
        for(int i = 1; i < SpawnPoints.Count; i++)
        {
            if(i < SpawnPoints.Count - 2)
            {
                bool addRoom = (Random.value > 0.5f);

                if (addRoom)
                    _main = SpawnObject(parts.Main, SpawnPoints[i], firstMinimum, firstMaximum, "LayerOne");

                bool addWindow = (Random.value > 0.5f);
                    
                if(addWindow)
                {
                    _window = Instantiate(parts.Window);
                    _window.name = "Window";

                    if(addRoom)
                        _window.transform.parent = _main.transform.Find("SpawnWindow");
                    else
                        _window.transform.parent = SpawnPoints[i];

                    _window.transform.localPosition = new Vector3(0, 0, 0.025f); ;
                    _window.transform.localEulerAngles = Vector3.zero;
                }

                addWindow = (Random.value > 0.5f);

                if (addWindow)
                {
                    _window = Instantiate(parts.Window);
                    _window.name = "Window";

                    if (addRoom)
                        _window.transform.parent = _main.transform.Find("SpawnWindow");
                    else
                        _window.transform.parent = SpawnPoints[i];

                    _window.transform.localPosition = new Vector3(0, 2, 0.025f);
                    _window.transform.localEulerAngles = Vector3.zero;
                }
            }
            else
            {
                bool extra = (Random.value > 0.5f);

                if (extra)
                    _main = SpawnObject(parts.Main, SpawnPoints[i], Vector3.one, Vector3.one, "LayerTwo");
            }
        }
    }

    GameObject SpawnObject(GameObject _object, Transform _parent, Vector3 sizeMin, Vector3 sizeMax, string name)
    {
        GameObject temp;

        temp = Instantiate(_object, _parent);

        float sizeX = Random.Range(sizeMin.x, sizeMax.x);
        float sizeY = Random.Range(sizeMin.y, sizeMax.y);
        float sizeZ = Random.Range(sizeMin.z, sizeMax.z);

        temp.name = name;
        temp.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);

        return temp;
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            objectsToDelete.Add(transform.GetChild(i).gameObject);

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        SpawnPoints.Clear();

        this.name = "SpawnHouse";
    }
}
