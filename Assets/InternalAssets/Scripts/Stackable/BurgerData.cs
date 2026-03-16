using System.Collections.Generic;
using UnityEngine;

public class BurgerData : MonoBehaviour
{
    [SerializeField] private List<StackableObject> _stackableObjects = new();

    public List<StackableObject> StackableObjects => _stackableObjects;

    public void Add(StackableObject stackableObject)
    {
        if (_stackableObjects.Contains(stackableObject)) return;

        _stackableObjects.Add(stackableObject);
    }

    public void Remove(StackableObject stackableObject)
    {
        _stackableObjects.Remove(stackableObject);
    }
}
