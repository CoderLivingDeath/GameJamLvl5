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
}

public class Meanings
{
    public Meaning cult { get; set; } = new Meaning();

    public Meaning doctor { get; set; } = new Meaning();

    public Meaning island { get; set; } = new Meaning();

    public Meaning fail { get; set; } = new Meaning();

    public string GetTone(string tone)
    {
        return tone switch
        {
            "cult" => cult.tone_cult,
            "doctor" => doctor.tone_doctor,
            "island" => island.tone_island,
            "fail" => throw new ArgumentException($"Unavalibe tone: {tone}"),
            _ => throw new ArgumentException($"Unknown tone: {tone}")
        };
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

