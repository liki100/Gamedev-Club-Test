using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private List<DefaultItemInfo> _items;

    private readonly string PATH ="/WorldData.save";

    public class WorldData
    {
        public CharacterData CharacterData = new();
        public List<MonsterData> MonstersData = new();
        public List<InventoryData> InventoryData = new();
        public WeaponData WeaponData = new();
        public List<ItemData> ItemData = new();
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
        public string InfoId;
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
        var character = ServiceLocator.Current.Get<Character>();
        var spawner = ServiceLocator.Current.Get<Spawner>();
        var weapon = ServiceLocator.Current.Get<RangeWeapon>();
        
        if (!File.Exists(Application.persistentDataPath + PATH))
        {
            character.DefaultData();
            spawner.SpawnNumberMonsters();
            weapon.DefaultData();
            return;
        }
        
        var worldData = JsonConvert.DeserializeObject<WorldData>(File.ReadAllText(Application.persistentDataPath + PATH));

        if (worldData == null)
            return;
        
        //CHARACTER
        character.SetData(worldData.CharacterData);
        
        //SPAWNER
        spawner.DeleteMonsters();
        foreach (var monsterData in worldData.MonstersData)
        {
            var monster = spawner.CreateMonsters();
            monster.SetData(monsterData);
        }
        
        //INVENTORY
        var inventory = character.Inventory;
        inventory.Clear();
        foreach (var inventoryData in worldData.InventoryData)
        {
            var info = _items.Find(i => i.Id == inventoryData.InfoId);
            var item = new InventoryItem(info, inventoryData.Amount);
            inventory.TryAdd(item);
        }
        
        //WEAPON SLOT
        var weaponSlot = inventory.GetSlotWithType(ItemType.Weapon);
        var weaponInfo = _items.Find(i => i.Id == worldData.WeaponData.InfoId);
        var weaponItem = new InventoryItem(weaponInfo);
        weaponSlot.SetItem(weaponItem);

        //WEAPON
        weapon.SetData(worldData.WeaponData);

        //SPAWNER ITEM
        var spawnerItems = ServiceLocator.Current.Get<SpawnerItems>();
        spawnerItems.DeleteItems();
        foreach (var data in worldData.ItemData)
        {
            var info = _items.Find(i => i.Id == data.InfoId);
            var item = spawnerItems.SpawnItem();
            item.SetData(data, info);
            item.Init();
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
