using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class testNevmesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Add a NavMeshObstacle component
        NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();

        // Set obstacle properties
        obstacle.carving = true; // Allows real-time updating of the NavMesh
        obstacle.shape = NavMeshObstacleShape.Box; // Choose Box or Capsule
        obstacle.size = new Vector3(2, 2, 2); // Adjust size based on your object

        // Add a Collider (BoxCollider in this case)
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();

        // Adjust collider size (must match or be slightly bigger than obstacle)
        collider.size = new Vector3(2, 2, 2);

        // Make sure the collider is enabled
        collider.enabled = true;
    }
}
