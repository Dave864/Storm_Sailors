using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    // References to other game objects
    private GameObject player;
    private GameObject cloudManager;
    private GameObject shipObject;

    // Movement parameters
    public int baseSpeed = 100;              // The base speed of the ship
    public float slowTime = 1.0f;            // The time for the ship to fully slow down
    public float uTurnTime = 1.0f;           // The time for the ship to turn 180 degrees in seconds
    [HideInInspector] public float turnRate; // The turning rate in deg / sec

    // Use this for initialization
    void Start ()
    {
        turnRate = 180.0f / uTurnTime;
        // Get reference to cloud manager object
        cloudManager = GameObject.Find("Cloud Manager");
        if (!cloudManager)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Get reference to ship object object
        shipObject = GameObject.Find("Ship Object");
        if (!shipObject)
        {
            Debug.LogError("Ship Object object not found", shipObject);
        }

        // Get reference to player object
        player = GameObject.Find("Player");
        if (!player)
        {
            Debug.LogError("Player object not found", player);
        }
	}

    // Handle collision
    private void OnTriggerEnter(Collider obj)
    {
        // Go back to start if ship hit an obstacle
        if (obj.gameObject.CompareTag("Obstacle"))
        {
            cloudManager.GetComponent<CloudManager>().DispelAll();
            Vector3 startPos = shipObject.GetComponentInParent<Player>().startPos;
            player.transform.position = startPos;
        }

        // Exit the game when finish line is reached
        else if (obj.gameObject.CompareTag("Finish"))
        {
            Application.Quit();
        }
    }
}
