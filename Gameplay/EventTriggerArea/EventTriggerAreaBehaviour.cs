using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class EventTriggerAreaBehaviour : MonoBehaviour
{
    [Tooltip("Layers to interact with")]
    [SerializeField] private LayerMask _validLayers = ~0;

    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;

    private void Awake()
    {
        // Set all colliders on this object to triggers
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
            collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If other was not in _validLayers, return;
        if (((1 << other.gameObject.layer) & _validLayers) == 0) return;

        OnEnter.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        // If other was not in _validLayers, return;
        if (((1 << other.gameObject.layer) & _validLayers) == 0) return;

        OnStay.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        // If other was not in _validLayers, return;
        if (((1 << other.gameObject.layer) & _validLayers) == 0) return;

        OnExit.Invoke();
    }

    [MenuItem("GameObject/Utility/Event Trigger Area", false, 10)]
    static void CreateEventTriggerArea(MenuCommand menuCommand)
    {
        // Creat a new object
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Game/Level/Prefabs/Utility/EventTriggerArea.prefab", typeof(GameObject));
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // Ensure it gets reparented if this was a context click
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

        // Register creation in the undo system
        Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);

        // Select this object
        Selection.activeObject = obj;
    }
}