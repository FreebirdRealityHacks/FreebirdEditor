using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Fireworks.Demo
{
    public class FireworkSpawner : MonoBehaviour
    {
        
        public List<GameObject> fireworkPrefabs = new List<GameObject>();
        public float spawnRadius = 20f;
        public float spawnRate = 2f;
        
        float randomizedRate;
        float timeTracker;


        void Start()
        {
            timeTracker = Time.time;
            randomizedRate = spawnRate * Random.Range(1f, 2f);
        }


        void Update()
        {
            if (Time.time - timeTracker > randomizedRate)
            {
                Spawn();
                timeTracker = Time.time;
                randomizedRate = spawnRate * Random.Range(1f, 2f);
            }
        }

        void Spawn()
        {
            GameObject go = fireworkPrefabs[Random.Range(0, fireworkPrefabs.Count)];
            Vector3 sphere = Random.insideUnitSphere * spawnRadius;
            Instantiate(go, new Vector3(sphere.x, 0, sphere.y), Quaternion.identity);
        }
    }
}