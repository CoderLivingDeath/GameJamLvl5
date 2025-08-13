using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContactDirectionProvider : MonoBehaviour
{
    private const int MAX_CONTACTS = 8;

    [SerializeField]
    private float _expansion;
    private Collider2D _ownCollider;

    [Flags]
    public enum CollisionDirection
    {
        None = 0,
        Up = 1 << 0,  // 1
        Down = 1 << 1,  // 2
        Left = 1 << 2,  // 4
        Right = 1 << 3,  // 8
    }

    private CollisionDirection GetCollisionDirection(Vector2 normal)
    {
        float threshold = 0.5f;
        CollisionDirection direction = CollisionDirection.None;

        if (normal.y > threshold) direction |= CollisionDirection.Up;
        else if (normal.y < -threshold) direction |= CollisionDirection.Down;

        if (normal.x > threshold) direction |= CollisionDirection.Right;
        else if (normal.x < -threshold) direction |= CollisionDirection.Left;

        return direction;
    }

    public IEnumerable<ContactPoint2D> GetContacts()
    {
        ContactPoint2D[] points = new ContactPoint2D[MAX_CONTACTS];

        int count = _ownCollider.GetContacts(points);

        return points.Take(count);
    }

    public IEnumerable<(ContactPoint2D, CollisionDirection)> GetContactsWithDirection()
    {
        return GetContacts().Select(h => (h, GetCollisionDirection(-h.normal)));
    }

    public bool HasContactDirection(CollisionDirection directions)
    {
        foreach (var (_, contactDirection) in GetContactsWithDirection())
        {
            // Проверяем, есть ли хоть один флаг из directions в contactDirection
            if ((directions & contactDirection) != 0)
                return true;
        }
        return false;
    }

    public IEnumerable<ContactPoint2D> GetContactsByDirection(CollisionDirection directions, bool strict = false)
    {
        var contacts = GetContactsWithDirection();
        if (strict)
        {
            return contacts
                .Where(h => h.Item2 == directions)
                .Select(h => h.Item1);
        }
        else
        {
            return contacts
                .Where(h => (directions & h.Item2) != 0)
                .Select(h => h.Item1);
        }
    }

    private void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
    }
}
