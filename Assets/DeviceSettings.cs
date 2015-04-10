using UnityEngine;
using System.Collections;

public class DeviceSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
}
