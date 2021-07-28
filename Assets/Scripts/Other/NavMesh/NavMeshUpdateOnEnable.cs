using UnityEngine;
using UnityEngine.AI;


public class NavMeshUpdateOnEnable : MonoBehaviour {

    public NavMeshData data;
    private NavMeshDataInstance dataInstance;

    void OnEnable() {
        dataInstance = NavMesh.AddNavMeshData( data, this.transform.position, this.transform.rotation );
    }

    void OnDisable() {
        NavMesh.RemoveNavMeshData( dataInstance );
    }
}