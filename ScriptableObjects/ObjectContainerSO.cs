using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of GameObjects and provides functionality for retrieving one at random
/// </summary>
[CreateAssetMenu(fileName = "New Object Container", menuName = "ScriptableObjects/ObjectContainer")]
public class ObjectContainerSO : ScriptableObject
{
    [Tooltip("List of objects held by this ObjectContainer")]
    [SerializeField] private GameObject[] _objects;

    [Tooltip("List of objects held by this ObjectContainer")]
    public GameObject[] Objects { get => _objects; private set => _objects = value; }

    /// <summary>
    /// Retrieve a random object from this container
    /// </summary>
    public GameObject GetRandomObject() => Objects[Random.Range(0, Objects.Length)];
}