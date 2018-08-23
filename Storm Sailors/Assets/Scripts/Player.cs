using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start area reference
    public GameObject startArea = null;
    public Vector3 StartPos { get; set; }

    // References to components of player
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject wizard;
    private GameObject ship;

    // Movement parameters
    private readonly int defaultSpeed = 500;        // The default base speed of the ship
    private readonly float defaultRotRate = 0.1f;   // The default time it takes to change wizard position in sec
    private int baseSpeed;                          // The used base speed of the ship
    private float rotRate;                          // The used time it takes to change wizard position in sec

    // Use this for initialization
    void Start()
    {
        // Establish the references to the components
        compassCenter = gameObject.transform.Find("Compass/Compass Center").gameObject;
        wizard = gameObject.transform.Find("Compass/Compass Center/Wizard Object").gameObject;
        ship = gameObject.transform.Find("Ship Object").gameObject;
        cloudManager = gameObject.transform.Find("Compass/Cloud Manager").gameObject;

        // Initialize the movement parameters
        baseSpeed = (ship == null) ? defaultSpeed : ship.GetComponent<Ship>().baseSpeed;

        // Set initial position of player
        StartPos = (startArea == null) ? new Vector3(0, 0, 0) : startArea.transform.position;
        transform.position = new Vector3(StartPos.x, transform.position.y, StartPos.z);
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
        transform.position += ship.GetComponent<Ship>().CurHeading * baseSpeed * Time.deltaTime;
    }
}
