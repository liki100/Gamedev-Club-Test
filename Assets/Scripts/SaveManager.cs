using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private List<InventoryItemInfo> _items;

    private readonly string PATH ="/jsonWorld.json";

    public class WorldData
    {
        public CharacterData CharacterData = new CharacterData();
        public List<MonsterData> MonstersData = new List<MonsterData>();
        public List<InventoryData> InventoryData = new List<InventoryData>();
        public WeaponData WeaponData = new WeaponData();
        public List<ItemData> ItemData = new List<ItemData>();
    }

    public class CharacterData
    {
        public Vector3 Position;
        public float Health;
    }

    public class MonsterData
    {
        public Vector3 Position;
        public bool Target;
        public float Health;
    }
    
    public class InventoryData
    {
        public string InfoId;
        public int Amount;
    }
    
    public class WeaponData
    {
        public int Ammo;
        public float FireRateTime;
    }
    
    public class ItemData
    {
        public string InfoId;
        public int Amount;
        public Vector3 Position;
    }

    public void Load()
    {
        if (!File.Exists(Application.persistentDataPath + PATH))
            return;
        
        var worldData = JsonConvert.DeserializeObject<WorldData>(File.ReadAllText(Application.persistentDataPath + PATH));

        if (worldData == null)
            return;

        var character = ServiceLocator.Current.Get<Character>();
        character.SetData(worldData.CharacterData);

        var spawner = ServiceLocator.Current.Get<Spawner>();
        spawner.DeleteMonsters();
        foreach (var monsterData in worldData.MonstersData)
        {
            var monster = spawner.CreateMonsters();
            monster.SetData(monsterData);
        }
        
        var inventory = character.Inventory;
        inventory.Clear();
        foreach (var inventoryData in worldData.InventoryData)
        {
            var info = _items.Find(i => i.Id == inventoryData.InfoId);
            var item = new InventoryItem(info);
            item.State.Amount = inventoryData.Amount;
            inventory.TryAdd(item);
        }

        var uiInventory = ServiceLocator.Current.Get<UIInventory>();
        uiInventory.Clear();
        uiInventory.Init();

        var weapon = ServiceLocator.Current.Get<RangeWeapon>();
        weapon.SetData(worldData.WeaponData);

        var spawnerItems = ServiceLocator.Current.Get<SpawnerItems>();
        spawnerItems.DeleteItems();
        foreach (var data in worldData.ItemData)
        {
            var info = _items.Find(i => i.Id == data.InfoId);
            var item = spawnerItems.SpawnItem();
            item.SetData(data, info);
        }
    }
    
    public void Save()
    {
        var character = ServiceLocator.Current.Get<Character>();
        var characterData = character.GetData();

        var monsters = ServiceLocator.Current.Get<Spawner>().Monsters;
        var monsterData = new List<MonsterData>();
        
        foreach (var monster in monsters)
        {
            monsterData.Add(monster.GetData());
        }

        var inventorySlots = character.Inventory.GetAllSlotIsNotEmpty();
        var inventoryData = new List<InventoryData>(); 

        foreach (var slot in inventorySlots)
        {
            inventoryData.Add(slot.GetData());
        }

        var weapon = ServiceLocator.Current.Get<RangeWeapon>();
        var weaponData = weapon.GetData();

        var items = ServiceLocator.Current.Get<SpawnerItems>().Items;
        var itemData = new List<ItemData>();

        foreach (var item in items)
        {
            itemData.Add(item.GetData());
        }

        var data = new WorldData()
        {
            CharacterData = characterData,
            MonstersData = monsterData,
            InventoryData = inventoryData,
            WeaponData = weaponData,
            ItemData = itemData
        };
        
        File.WriteAllText(Application.persistentDataPath + PATH, 
            JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
        );
    }

    public void Delete()
    {
        if (!File.Exists(Application.persistentDataPath + PATH))
            return;
        
        File.Delete(Application.persistentDataPath + PATH);
    }
}
