using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TargetSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;           // Posição do spawn (objeto vazio)
        public GameObject targetPrefab;      // Qual target vai nascer
        public int quantity = 1;             // Quantos targets
        public Vector3 scale = Vector3.one;  // Tamanho
        public Vector3 rotation = Vector3.zero; // Rotação
        
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
    private Dictionary<SpawnPoint, Coroutine> respawnCoroutines = new Dictionary<SpawnPoint, Coroutine>();
    
    void Start()
    {
        SpawnAllTargets();
    }
    
    void Update()
    {
        // Remove targets destruídos da lista
        int removedCount = spawnedTargets.RemoveAll(t => t == null);
        
        // Se algum target foi destruído, verifica quais spawn points precisam de respawn
        if (removedCount > 0)
        {
            CheckAndRespawnTargets();
        }
    }
    
    void CheckAndRespawnTargets()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            // Conta quantos targets deste spawn point ainda existem
            int currentCount = spawnedTargets.Count(t => 
                t != null && t.GetComponent<Target>()?.spawnPoint == point);
            
            // Se faltar target e não houver uma coroutine de respawn ativa para este ponto
            if (currentCount < point.quantity && !respawnCoroutines.ContainsKey(point))
            {
                // Inicia a coroutine de respawn com delay
                Coroutine coroutine = StartCoroutine(RespawnWithDelay(point));
                respawnCoroutines[point] = coroutine;
            }
            // Se já tem a quantidade certa e existe coroutine, remove ela
            else if (currentCount >= point.quantity && respawnCoroutines.ContainsKey(point))
            {
                StopCoroutine(respawnCoroutines[point]);
                respawnCoroutines.Remove(point);
            }
        }
    }
    
    IEnumerator RespawnWithDelay(SpawnPoint point)
    {
        // Aguarda 2 segundos
        yield return new WaitForSeconds(2f);
        
        // Remove a coroutine do dicionário
        respawnCoroutines.Remove(point);
        
        // Verifica novamente se ainda falta target
        int currentCount = spawnedTargets.Count(t => 
            t != null && t.GetComponent<Target>()?.spawnPoint == point);
        
        // Se ainda faltar target, cria um novo
        if (currentCount < point.quantity)
        {
            SpawnTarget(point);
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
        if (point.position == null || point.targetPrefab == null) return;
        
        // Cria o target
        GameObject target = Instantiate(point.targetPrefab, point.position.position, Quaternion.Euler(point.rotation));
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