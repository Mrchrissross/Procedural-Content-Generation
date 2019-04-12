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

    public LayerMask TerrainAndWater;
    public LayerMask noWater;

    // Start is called before the first frame update
    void Start()
    {
        // Terrain
        GameObject temp = Instantiate(worldObjects.terrain);
        temp.GetComponent<GenerateTerrain>().Generate();

        // Boids
        Instantiate(worldObjects.ships);
        Instantiate(worldObjects.birds);
        Instantiate(worldObjects.fish);

        // Stone Tower
        temp = Instantiate(worldObjects.stoneTower);
        temp.transform.position = new Vector3(0, 100, 0);
        temp.GetComponent<GenerateStoneTower>().Generate();
        FindGround(temp, 5.0f, false);

        // Japan Towers
        temp = new GameObject();
        temp.name = "Japan Towers";
        for (int i = 0; i < 30; i++)
        {
            GameObject temp2 = Instantiate(worldObjects.japanTower);
            temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
            temp2.GetComponent<GenerateJapanTower>().Generate();
            FindGround(temp2, 0.0f, true);
            temp2.transform.parent = temp.transform;
        }

        // Trees
        temp = new GameObject();
        temp.name = "Trees";
        for (int i = 0; i < 50; i++)
        {
            GameObject temp2 = Instantiate(worldObjects.tree);
            temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
            temp2.GetComponent<GenerateTree>().Generate();
            FindGround(temp2, 0.0f, true);
            temp2.transform.parent = temp.transform;
        }

        // Palm Trees
        temp = new GameObject();
        temp.name = "Palm Trees";
        for (int i = 0; i < 100; i++)
        {
            GameObject temp2 = Instantiate(worldObjects.palm);
            temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
            temp2.GetComponent<GeneratePalmTree>().Generate();
            FindGround(temp2, 0.0f, true);
            temp2.transform.parent = temp.transform;
        }

        // Palm Trees
        temp = new GameObject();
        temp.name = "Rocks";
        for (int i = 0; i < 400; i++)
        {
            float scale = Random.Range(1.0f, 6.0f);
            GameObject temp2 = Instantiate(worldObjects.rock);
            temp2.transform.position = new Vector3(Random.Range(-250.0f, 250.0f), 100, Random.Range(-250.0f, 250.0f));
            temp2.GetComponent<GenerateRock>().Generate();
            FindGround(temp2, 0.0f, false);
            temp2.transform.parent = temp.transform;
            temp2.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void FindGround(GameObject _prefab, float offset, bool landBased)
    {
        RaycastHit hit;

        if (landBased)
        {
            if (Physics.Raycast(_prefab.transform.position, -_prefab.transform.up, out hit, 100, TerrainAndWater))
                _prefab.transform.position = new Vector3(hit.point.x, hit.point.y + offset, hit.point.z);

            if (hit.transform.name == "water")
                Destroy(_prefab);
        }
        else
        {
            if (Physics.Raycast(_prefab.transform.position, -_prefab.transform.up, out hit, 100, noWater))
                _prefab.transform.position = new Vector3(hit.point.x, hit.point.y + offset, hit.point.z);
        }
    }
}
