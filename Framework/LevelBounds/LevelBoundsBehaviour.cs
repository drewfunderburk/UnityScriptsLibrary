using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBoundsBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 _bounds = new Vector3(100, 100, 100);

    [SerializeField] private Transform _playerTransform;

    public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
    public Vector3 Bounds { get => _bounds; set => _bounds = value; }

    private void Update()
    {
        if (!PlayerTransform) return;
        Vector3 position = PlayerTransform.position;

        position.x = Mathf.Clamp(position.x, -Bounds.x / 2, Bounds.x / 2);
        position.y = Mathf.Clamp(position.y, -Bounds.y / 2, Bounds.y / 2);
        position.z = Mathf.Clamp(position.z, -Bounds.y / 2, Bounds.z / 2);

        if (position != PlayerTransform.position)
            PlayerTransform.position = position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Bounds);
    }

    [MenuItem("GameObject/Level Bounds", false, 10)]
    static void CreateLevelBounds(MenuCommand menuCommand)
    {
        // Creat a new object
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/LevelBounds/LevelBounds.prefab", typeof(GameObject));
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // Ensure it gets reparented if this was a context click
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

        // Register creation in the undo system
        Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);

        // Select this object
        Selection.activeObject = obj;
    }
}