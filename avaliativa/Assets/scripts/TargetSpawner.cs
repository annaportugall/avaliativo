using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        // Posição do spawn (objeto vazio)
        public Transform position;

        // Qual target vai nascer
        public GameObject targetPrefab;

        // Quantos targets
        public int quantity = 1;

        // Tamanho
        public Vector3 scale = Vector3.one;

        // Rotação
        public Vector3 rotation = Vector3.zero;

        // Movimento
        public bool moveHorizontal = false;
        public bool moveVertical = false;
        public float moveSpeed = 3f;
        public float moveRange = 5f;

        // Stats
        public int health = 1;
        public int pointsValue = 10;
    }

    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private List<GameObject> spawnedTargets = new List<GameObject>();

    void Start()
    {
        SpawnAllTargets();
    }

    void Update()
    {
        // Remove targets destruídos da lista
        spawnedTargets.RemoveAll(t => t == null);

        // Conta targets ativos por spawn point
        foreach (SpawnPoint point in spawnPoints)
        {
            int currentCount = 0;

            foreach (GameObject target in spawnedTargets)
            {
                if (target != null)
                {
                    Target targetScript = target.GetComponent<Target>();

                    if (targetScript != null &&
                        targetScript.spawnPoint == point)
                    {
                        currentCount++;
                    }
                }
            }

            // Se faltar target, cria um novo
            if (currentCount < point.quantity)
            {
                SpawnTarget(point);
            }
        }
    }

    void SpawnAllTargets()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            for (int i = 0; i < point.quantity; i++)
            {
                SpawnTarget(point);
            }
        }
    }

    void SpawnTarget(SpawnPoint point)
    {
        if (point.position == null || point.targetPrefab == null)
            return;

        // Cria o target
        GameObject target = Instantiate(
            point.targetPrefab,
            point.position.position,
            Quaternion.Euler(point.rotation)
        );

        target.transform.localScale = point.scale;

        // Configura o target
        Target targetScript = target.GetComponent<Target>();

        if (targetScript != null)
        {
            targetScript.spawnPoint = point;

            targetScript.moveHorizontal = point.moveHorizontal;
            targetScript.moveVertical = point.moveVertical;
            targetScript.moveSpeed = point.moveSpeed;
            targetScript.moveRange = point.moveRange;

            targetScript.health = point.health;
            targetScript.pointsValue = point.pointsValue;
        }

        spawnedTargets.Add(target);
    }
}