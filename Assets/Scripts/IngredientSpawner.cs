using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private int _maxAtTime = 5;

    [SerializeField] private float _spawnCooldown = 10.0f;

    [SerializeField] private ResourceType[] _possibleResourceSpawns;
    [SerializeField] private Transform _resource;

    private Transform[] _spawnPoints;

    private float _timer;

    public IEnumerable<Transform> PossibleSpawnPoints => _spawnPoints.Where(t => t.childCount == 0);
    public int SpawnedCount => _spawnPoints.Where(t => t.childCount >= 0).Count();

    private void Start()
    {
        _spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            _spawnPoints[i] = transform.GetChild(i);
        }

        for (int i = 0; i < _maxAtTime; ++i)
        {
            Spawn();
        }
    }

    private void Update()
    {
        // Max for spawner spawned, no op. needed.
        if (SpawnedCount >= _maxAtTime)
        {
            return;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        Transform[] possibleSpawnPoints = PossibleSpawnPoints.ToArray();

        ResourceType selectedType = null;

        foreach (ResourceType t in _possibleResourceSpawns)
        {
            if (!selectedType)
            {
                selectedType = t;
                continue;
            }

            if (t.Count < selectedType.Count)
            {
                selectedType = t;
            }
        }

        int randSpawnPosIndex = Random.Range(0, possibleSpawnPoints.Length);

        var resource = Instantiate(_resource, possibleSpawnPoints[randSpawnPosIndex]);
        resource.GetComponent<Resource>().Initialise(selectedType);

        _timer = _spawnCooldown;
    }
}
