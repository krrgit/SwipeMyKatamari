#if UNITY_EDITOR
using UnityEngine;

public class SpawnRandomObject : MonoBehaviour
{
    [SerializeField] private Transform parent;
    public GameObject[] objectsToSpawn;
    public Vector3 gridSize = new Vector3(5f, 5f, 5f);
    public float gridSpacing = 2f;
    public float openSpaceYBuffer = 0.67f;

    [Header("Debug")]
    public bool showGrid = true;

    void OnDrawGizmos()
    {
        if (!showGrid) return;

        Gizmos.color = Color.gray;
        for (float x = 0; x < gridSize.x; x += gridSpacing)
        {
            for (float y = 0; y < gridSize.y; y += gridSpacing)
            {
                for (float z = 0; z < gridSize.z; z += gridSpacing)
                {
                    Gizmos.DrawWireCube(transform.position + (new Vector3(x, y+0.5f, z) * gridSpacing), Vector3.one * gridSpacing);
                }
            }
        }
    }

    public void SpawnRandomObjectInEditMode()
    {
        if (objectsToSpawn.Length == 0)
        {
            Debug.LogError("No objects to spawn!");
            return;
        }

        for (float x = 0; x < gridSize.x; x += gridSpacing)
        {
            for (float y = 0; y < gridSize.y; y += gridSpacing)
            {
                for (float z = 0; z < gridSize.z; z += gridSpacing)
                {
                    GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
                    Vector3 position = new Vector3(x, y, z) * gridSpacing;
                    position.y += objectToSpawn.transform.position.y;
                    
                    
                    if (!IsColliderInGridSpace(transform.position + position))
                    {
                        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f) * gridSpacing, 0, Random.Range(-0.5f, 0.5f) * gridSpacing);
                        position += randomOffset;
                        
                        GameObject spawnedObject = Instantiate(objectToSpawn, transform.position + position, objectToSpawn.transform.rotation);
                        spawnedObject.transform.parent = parent;
                        UnityEditor.Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawned Object");
                    }
                    
                }
            }
        }
    }
    
    bool IsColliderInGridSpace(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position + (gridSpacing * openSpaceYBuffer * Vector3.up),  Vector3.one * gridSpacing / 2f);
        return colliders.Length > 0;
    }
}

#endif
