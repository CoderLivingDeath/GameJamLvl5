using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class DataService
{
    public enum Root
    {
        SoftCult, HardCult,
        SoftDoc, HardDoc,
        SoftIsland, HardIsland,
        neutral
    }

    public ItemsRoot ItemsData => _itemsData;
    private ItemsRoot _itemsData;

    private Root _currentRoot = Root.neutral;

    public event Action<Root> RootChanged;

    // public DataService()
    // {
    //     TextAsset dataAsset = Resources.Load<TextAsset>("LocationsData"); // загрузка текстового файла из Resources
    //     if (dataAsset == null)
    //     {
    //         Debug.LogError("Не удалось загрузить LocationsData из Resources");
    //         return;
    //     }

    //     string json = dataAsset.text; // получение строки JSON из TextAsset

    //     Debug.Log(json);
    //     _itemsData = JsonConvert.DeserializeObject<ItemsRoot>(json);
    // }


    public void SetRoot(Root root)
    {
        if (root != _currentRoot)
        {
            _currentRoot = root;
            RootChanged?.Invoke(root);
        }
    }

    // private Dictionary<string, Item> GetAllItems()
    // {
    //     return LocationsData.GetAllItems().ToDictionary((item) => item.name);
    // }

    public Root GetRoot() => _currentRoot;
}

public class StringToRootConverter
{
    // Использование Enum.Parse (с выбросом исключения при ошибке)
    public static DataService.Root ConvertUsingParse(string input)
    {
        try
        {
            return (DataService.Root)Enum.Parse(typeof(DataService.Root), input, true); // true для игнорирования регистра
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"Недопустимое значение '{input}' для перечисления Root");
        }
    }

    // Использование Enum.TryParse (безопасный способ)
    public static bool TryConvertUsingTryParse(string input, out DataService.Root result)
    {
        return Enum.TryParse<DataService.Root>(input, true, out result) && Enum.IsDefined(typeof(DataService.Root), result);
    }
}

public class ItemsRoot
{
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    public string? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Meanings Meanings { get; set; } = new Meanings();
}

public class Meanings
{
    public Meaning Cult { get; set; } = new Meaning();

    public Meaning Doctor { get; set; } = new Meaning();

    public Meaning Island { get; set; } = new Meaning();

    public Meaning Fail { get; set; } = new Meaning();
}

public class Meaning
{
    public string Tag { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string ToneCult { get; set; } = string.Empty;

    public string ToneDoc { get; set; } = string.Empty;
    public string ToneIsland { get; set; } = string.Empty;
}

