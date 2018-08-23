using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start area reference
    public Vector3 startPos = new Vector3(0, 0, 0);

    // References to components of player
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject wizard;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default base speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time it takes to change wizard position in sec
    private int baseSpeed;                          // The used base speed of the ship

    // Use this for initialization
    void Start()
    {
        // Establish the references to the components
        compassCenter = gameObject.transform.Find("Compass/Compass Center").gameObject;
        wizard = gameObject.transform.Find("Compass/Compass Center/Wizard Object").gameObject;
        ship = gameObject.transform.Find("Ship Object").gameObject;
        cloudManager = gameObject.transform.Find("Compass/Cloud Manager").gameObject;

        // Initialize the movement parameters
        baseSpeed = (ship == null) ? defaultSpeed : ship.GetComponent<ShipObject>().baseSpeed;

        // Set initial position of player
        transform.position = new Vector3(startPos.x, transform.position.y, startPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Quit qame
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }

        // Move the player entity based off of the heading of the ship
        transform.position += ship.GetComponent<ShipObject>().CurHeading * baseSpeed * Time.deltaTime;
    }
}
