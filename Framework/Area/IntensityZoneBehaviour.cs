using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SphereCollider))]
public class IntensityZoneBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve _proximityCurve;

    private SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Get distance and evaluate on _proximityCurve
            float distance = Vector3.Distance(other.transform.position, transform.position);
            float curve = _proximityCurve.Evaluate(1 - (distance / _collider.radius));
        }
    }

    [MenuItem("GameObject/Utility/Intensity Zone", false, 10)]
    static void CreateEventTriggerArea(MenuCommand menuCommand)
    {
        // Creat a new object
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Game/Level/Prefabs/Utility/IntensityZone.prefab", typeof(GameObject));
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // Ensure it gets reparented if this was a context click
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

        // Register creation in the undo system
        Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);

        // Select this object
        Selection.activeObject = obj;
    }
}