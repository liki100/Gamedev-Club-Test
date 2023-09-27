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
        _monsters = new List<Monster>();
        for (var i = 0; i < _spawnCount; i++)
        {
            var monster = Instantiate(_monsterTemplate, GetFreeRandomPoint(), Quaternion.identity, transform);
            monster.Init();
            monster.OnDiedEvent += OnDied;
            _monsters.Add(monster);
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

    private void OnDied(Monster monster)
    {
        monster.OnDiedEvent -= OnDied;
        _monsters.Remove(monster);

        if (_monsters.Count == 0)
            _eventBus.Invoke(new AllMonstersDeadSignal());
    }

    public void DeleteMonsters()
    {
        foreach (var monster in _monsters)
        {
            monster.OnDiedEvent -= OnDied;
            Destroy(monster.gameObject);
        }
        _monsters.Clear();
    }

    public void CreateMonsters(SaveManager.MonsterData data, Character character)
    {
        var monster = Instantiate(_monsterTemplate, data.Position, Quaternion.identity, transform);
        monster.OnDiedEvent += OnDied;
        monster.SetData(data, character);
        _monsters.Add(monster);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_topLeftPoint, .1f);
        Gizmos.DrawWireSphere(_bottomRightPoint, .1f);
    }
}
