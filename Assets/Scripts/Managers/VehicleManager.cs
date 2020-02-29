using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    private static VehicleManager _instance;
    public static VehicleManager Instance { get { return _instance; } }
    public List<GameObject> vehiclePrefabs;
    public List<Transform> vehicleSpawnPoints;
    public List<GameObject> vehiclePool;
    public int maxVehicles;
    public float vehicleSpawnCooldown;
    public float lastSpawnTime;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            if(_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        lastSpawnTime = 0.0f;
        InstantiateVehicles();
    }

    private void Update()
    {
        if(Time.time - lastSpawnTime > vehicleSpawnCooldown)
        {
            SpawnVehicle();
            lastSpawnTime = Time.time;
        }
    }

    private void InstantiateVehicles()
    {
        vehiclePool = new List<GameObject>();
        for(int i=0; i<maxVehicles; i++)
        {
            GameObject g = Instantiate(vehiclePrefabs[i%4]);
            g.transform.parent = gameObject.transform;
            g.SetActive(false);
            vehiclePool.Add(g);
        }
    }

    private GameObject GetVehicle()
    {
        for(int i=0; i<maxVehicles; i++)
        {
            if (!vehiclePool[i].activeInHierarchy)
                return vehiclePool[i];
        }
        return null;
    }
    private void SpawnVehicle()
    {
        GameObject g = GetVehicle();
        if (g == null) return;
        g.transform.position = vehicleSpawnPoints[Random.Range(0, 2)].position;
        g.SetActive(true);
        g.GetComponent<Vehicle>().SFXVehicle.Play();
    }

    public void DespawnVehicle(GameObject g)
    {
        g.transform.position = Vector3.zero;
        g.transform.rotation = g.GetComponent<Vehicle>().initialRotation;
        g.GetComponent<Vehicle>().isBraking = false;
        g.GetComponent<Vehicle>().SFXVehicle.Stop();
        g.SetActive(false);
    }
}
