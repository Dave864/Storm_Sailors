using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShip : MonoBehaviour
{
    // Movement parameters
    public int baseSpeed = 100;
    public float uTurnTime = 0.5f;  // The time for the ship to turn 180 degrees in sec
    private float turnRate;         // The turning rate in deg / sec

	// Use this for initialization
	void Start () {
        turnRate = 180.0f / uTurnTime;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
