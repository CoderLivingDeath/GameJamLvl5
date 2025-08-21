using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Unity.Mathematics;

public class DataService
{
    public Root Root => _root;
    private Root _root;
    public ItemsRoot ItemsData => _itemsData;
    private ItemsRoot _itemsData;

    public DataService()
    {
        TextAsset dataAsset = Resources.Load<TextAsset>("LocationsData"); // загрузка текстового файла из Resources
        if (dataAsset == null)
        {
            Debug.LogError("Не удалось загрузить LocationsData из Resources");
            return;
        }

        string json = dataAsset.text; // получение строки JSON из TextAsset

        _itemsData = JsonConvert.DeserializeObject<ItemsRoot>(json);
    }

    public string GetMaxTon()
    {
        if (Root.Cult >= Root.Doc && Root.Cult >= Root.Island)
            return "cult";
        else if (Root.Doc >= Root.Cult && Root.Doc >= Root.Island)
            return "doctor";
        else if (Root.Island >= Root.Doc && Root.Island >= Root.Cult)
            return "island";
        else
        {
            return "neutral";
        }
    }

    public List<RootEnum> GetMaxTones()
    {
        int maxValue = Math.Max(_root.Cult, Math.Max(_root.Doc, _root.Island));

        var result = new List<RootEnum>();

        if (_root.Cult == maxValue) result.Add(RootEnum.Cult);
        if (_root.Doc == maxValue) result.Add(RootEnum.Doctor);
        if (_root.Island == maxValue) result.Add(RootEnum.Island);

        return result;
    }

    public void AddRootTag(RootEnum rootEnum)
    {
        switch (rootEnum)
        {
            case RootEnum.Cult:
                _root.Cult += 1;
                break;
            case RootEnum.Doctor:
                _root.Doc += 1;
                break;
            case RootEnum.Island:
                _root.Island += 1;
                break;
        }
    }
}

public struct Root
{
    public int Cult;
    public int Doc;
    public int Island;

    public override string ToString()
    {
        return $"Cult:{Cult}, Doc:{Doc}, Island:{Island}";
    }
}

public enum RootEnum
{
    Cult,
    Doctor,
    Island
}

public class ItemsRoot
{
    public List<Item> items { get; set; }
}

public class Item
{
    public string id { get; set; }

    public string name { get; set; }

    public Meanings meanings { get; set; }

    public string GetTone(string root, string tone)
    {
        Meaning meaning;

        // выбираем значение в зависимости от root
        switch (root.ToLower())
        {
            case "cult":
                meaning = meanings.cult;
                break;
            case "doctor":
                meaning = meanings.doctor;
                break;
            case "island":
                meaning = meanings.island;
                break;
            case "fail":
                meaning = meanings.fail;
                break;
            default:
                throw new ArgumentException($"Unknown root: {root}");
        }

        // возвращаем соответствующий тон по параметру tone
        switch (tone.ToLower())
        {
            case "cult":
                return meaning.tone_cult;
            case "doctor":
                return meaning.tone_doctor;
            case "island":
                return meaning.tone_island;
            default:
                throw new ArgumentException($"Unknown tone: {tone}");
        }
    }

}

public class Meanings
{
    public Meaning cult { get; set; } = new Meaning();

    public Meaning doctor { get; set; } = new Meaning();

    public Meaning island { get; set; } = new Meaning();

    public Meaning fail { get; set; } = new Meaning();

    // TODO: тоне получается неверно
    public string GetTone(string tone)
    {
        switch (tone)
        {
            case "cult":
                Debug.Log("GET TONE" + cult.tone_cult);
                return cult.tone_cult;
                break;
            case "doctor":
                Debug.Log("GET TONE" + cult.tone_doctor);
                return cult.tone_doctor;
                break;
            case "island":
                Debug.Log("GET TONE" + cult.tone_island);
                return cult.tone_island;
                break;
            case "fail":
                throw new ArgumentException($"Unavalibe tone: {tone}");
                break;
            default:
                throw new ArgumentException($"Unavalibe tone: {tone}");

        }
    }
}

public class Meaning
{
    public string tag { get; set; }

    public string text { get; set; }

    public string tone_cult { get; set; }

    public string tone_doctor { get; set; }
    public string tone_island { get; set; }
}

