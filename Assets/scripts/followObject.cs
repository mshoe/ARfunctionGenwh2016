using UnityEngine;
using System.Collections;

public class followObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public GameObject target;
	public float offset;
	// Update is called once per frame
	void Update () {
		var wantedPos = Camera.main.WorldToScreenPoint (target.transform.position);
		wantedPos.y += offset;
		transform.position = wantedPos;
	}
}
