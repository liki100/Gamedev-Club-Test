using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour, IService
{
    [SerializeField, Min(0)] private int _spawnCount;
    [SerializeField] private Monster _monsterTemplate;
    [SerializeField] private Vector2 _topLeftPoint;
    [SerializeField] private Vector2 _bottomRightPoint;
    [SerializeField] private float _radiusCheckSpawn;

    private List<Monster> _monsters;
    private EventBus _eventBus;
    
    public List<Monster> Monsters => _monsters;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Subscribe<MonsterDeadSignal>(MonsterDead);
        _monsters = new List<Monster>();
    }

    public void SpawnNumberMonsters()
    {
        for (var i = 0; i < _spawnCount; i++)
        {
            var monster = CreateMonsters();
            monster.transform.position = GetFreeRandomPoint();
            monster.Init();
        }
    }

    private Vector2 GetFreeRandomPoint()
    {
        var i = 1000;
        while (--i >= 0)
        {
            var rX = Random.Range(_topLeftPoint.x, _bottomRightPoint.x);
            var rY = Random.Range(_topLeftPoint.y, _bottomRightPoint.y);

            var newPosition = new Vector2(rX, rY);
            
            if (!Physics2D.OverlapCircle(newPosition, _radiusCheckSpawn))
            {
                return new Vector2(rX, rY);
            }
        }
        return Vector2.zero;
    }
    
    private void MonsterDead(MonsterDeadSignal signal)
    {
        _monsters.Remove(signal.Value);

        if (_monsters.Count == 0)
            _eventBus.Invoke(new AllMonstersDeadSignal());
    }

    public void DeleteMonsters()
    {
        foreach (var monster in _monsters) 
            Destroy(monster.gameObject);
        
        _monsters.Clear();
    }

    public Monster CreateMonsters()
    {
        var monster = Instantiate(_monsterTemplate, transform);

        _monsters.Add(monster);

        return monster;
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<MonsterDeadSignal>(MonsterDead);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_topLeftPoint, .1f);
        Gizmos.DrawWireSphere(_bottomRightPoint, .1f);
    }
}
