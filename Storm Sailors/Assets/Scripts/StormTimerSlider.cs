using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTimerSlider : MonoBehaviour
{
    private readonly float heightScale = 1.2f;

	// Use this for initialization
	void Start()
    {
        GameObject compassCenter = GameObject.Find("Compass Center");
        if (!compassCenter)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }
        else
        {
            Vector3 timerPosition = compassCenter.transform.position;
            timerPosition.y += heightScale * compassCenter.GetComponent<CompassCenter>().stormHeight;
            transform.position = Camera.main.WorldToScreenPoint(timerPosition);
        }
	}
}
