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
        public GameObject tree;
        public GameObject palm;
        public GameObject rock;
    };

    public WorldObjects worldObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = Instantiate(worldObjects.terrain);
        temp.GetComponent<GenerateTerrain>().Generate();
        Instantiate(worldObjects.ships);
        Instantiate(worldObjects.birds);
        Instantiate(worldObjects.fish);
    }
}
