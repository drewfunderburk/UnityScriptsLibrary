using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of GameObjects and provides functionality for retrieving one at random
/// </summary>
[CreateAssetMenu(fileName = "New Object Container", menuName = "ScriptableObjects/ObjectContainer")]
public class ObjectContainerSO : ScriptableObject
{
    [SerializeField] private GameObject[] _objects;

    public GameObject[] Objects { get => _objects; private set => _objects = value; }

    public GameObject GetRandomObject()
    {
        int index = Random.Range(0, Objects.Length);

        return Objects[index];
    }
}