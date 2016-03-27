using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transform.position += Vector3.right;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			transform.position += Vector3.left;
		}
	}
}
