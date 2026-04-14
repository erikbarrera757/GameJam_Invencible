using UnityEngine;
using UnityEngine.AI; // Necesario para NavMesh
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float intervaloSpawn = 10f;
    public int maxEnemigos = 3; // límite de enemigos activos

    private float tiempoSiguienteSpawn;
    private List<GameObject> enemigosActivos = new List<GameObject>();

    void Start()
    {

        tiempoSiguienteSpawn = Time.time + intervaloSpawn;
    }

    void Update()
    {
        // Solo spawnea si hay menos de maxEnemigos
        if (Time.time >= tiempoSiguienteSpawn && enemigosActivos.Count < maxEnemigos)
        {
            SpawnEnemy();
            tiempoSiguienteSpawn = Time.time + intervaloSpawn;
        }

        // Limpieza: eliminar referencias nulas (enemigos destruidos)
        enemigosActivos.RemoveAll(e => e == null);
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        Transform punto = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = punto.position;

        // Ajustar posición al NavMesh más cercano
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
        {
            GameObject enemigo = Instantiate(enemyPrefab, hit.position, punto.rotation);
            enemigosActivos.Add(enemigo);
            Debug.Log("Enemigo spawneado en: " + punto.name);
        }
        else
        {
            Debug.LogWarning("Spawner fuera del NavMesh: " + punto.name);
        }
    }
}
