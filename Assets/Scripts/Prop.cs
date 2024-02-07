using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ColliderType
{
    Sphere,
    Box
}

public class Prop : MonoBehaviour
{
    [SerializeField] private float volume = 0;
    [SerializeField] private bool isRigidbody = true;
    [SerializeField] private bool hasRigidbody = false;

    [SerializeField] private ColliderType colliderType;
    [SerializeField] private string propName = "prop";
    private Rigidbody rb;

    private static float UNIT_SPHERE_VOL = 4.19f;
    
    
    public string PropName
    {
        get { return propName; }
    }

    public void AddRigidbody()
    {
        if (!isRigidbody || hasRigidbody) return;

        rb = gameObject.AddComponent<Rigidbody>();
        hasRigidbody = true;
    }

    public void DestroyRigidbody()
    {
        if (!isRigidbody || !hasRigidbody) return;
        Destroy(rb);
    }
    

    public float Volume
    {
        get { return volume; }
    }
    
    void Start()
    {
        // Get the mesh filter component attached to the game object
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // Check if a mesh filter is attached
        if (meshFilter != null)
        {
            // Get the mesh from the mesh filter
            Mesh mesh = meshFilter.mesh;

            // Calculate the volume of the mesh
            volume = SimpleCalculateVolume(mesh);

            // Print the volume to the console
            // Debug.Log("Mesh Volume: " + volume);
        }
        else
        {
            Debug.LogError("Mesh filter not found!");
        }
    }

    float SimpleCalculateVolume(Mesh mesh)
    {
        float value = 0;
        switch (colliderType)
        {
            case ColliderType.Sphere:
                value = CalculateSphereVolume(mesh.bounds.size);
                break;
            case ColliderType.Box:
                value = CalculateBoxVolume(mesh.bounds.size);
                break;
            default:
                value = transform.localScale.x
                        * transform.localScale.y
                        * transform.localScale.z;
                break;
        }

        return value;
    }

    float CalculateSphereVolume(Vector3 bounds)
    {
        return UNIT_SPHERE_VOL
               * CalculateBoxVolume(bounds * 0.5f);
    }

    float CalculateBoxVolume(Vector3 bounds)
    {
        return bounds.x * transform.localScale.x
                        * bounds.y * transform.localScale.y
                        * bounds.z * transform.localScale.z;
    }

    // Function to calculate the volume of the mesh
    float CalculateVolume(Mesh mesh)
    {
        float value = 0;

        Collider temp = GetComponent<Collider>();
        var type = temp.GetType();
        
        
        
        if (!mesh.isReadable) return UNIT_SPHERE_VOL
                                     * transform.localScale.x 
                                     * transform.localScale.y 
                                     * transform.localScale.z;

        // Get the mesh vertices
        Vector3[] vertices = mesh.vertices;

        // Get the mesh triangles
        int[] triangles = mesh.triangles;

        // Iterate through each triangle in the mesh
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the vertices of the triangle
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            // Calculate the signed volume of the tetrahedron formed by the triangle and the origin
            value += Vector3.Dot(Vector3.Cross(v1, v2), v3) / 6.0f;
        }

        value *= transform.localScale.x * transform.localScale.y * transform.localScale.z;
        

        // Return the absolute value of the volume (volume may be negative due to triangle winding order)
        return Mathf.Abs(value);
    }
}
