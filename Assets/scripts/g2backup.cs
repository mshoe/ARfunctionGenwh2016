using UnityEngine;
using System.Collections;

public class g2backup : MonoBehaviour {

	[Range(10, 100)]
	public int resolution = 10;

	private int currentResolution;
	private ParticleSystem.Particle[] points;

	private void CreatePoints () {
		if (resolution < 10 || resolution > 100) {
			Debug.LogWarning ("Grapher resolution out of bounds, resetting to minimum.", this);
			resolution = 10;
		}
		currentResolution = resolution;
		points = new ParticleSystem.Particle[resolution * resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for (int x = 0; x < resolution; x++) {
			for (int z = 0; z < resolution; z++) {
				Vector3 p = new Vector3 (x * increment, 0f, z * increment);
				points [i].position = p;
				points [i].color = new Color (p.x, 0f, p.z);
				points [i++].size = 0.1f;
			}
		}
	}

	private delegate float FunctionDelegate (Vector3 p, float t);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine
	};

	void Update () {
		if (currentResolution != resolution || points == null) {
			CreatePoints ();
		}
		float t = Time.timeSinceLevelLoad;
		FunctionDelegate f = functionDelegates [(int)function];
		for (int i = 0; i < points.Length; i++) {
			Vector3 p = points [i].position;
			p.y = f(p, t);
			points [i].position = p;
			Color c = points [i].color;
			c.g = p.y;
			points [i].color = c;
		}
		GetComponent<ParticleSystem>().SetParticles (points, points.Length);
	}

	private static float Linear (Vector3 p, float t) {
		return p.x;
	}

	private static float Exponential (Vector3 p, float t) {
		return p.x * p.x;
	}

	private static float Parabola (Vector3 p, float t) {
		p.x = 2f * p.x - 1f;
		return p.x * p.x;
	}

	private static float Sine (Vector3 p, float t) {
		return 0.5f + 0.5f * Mathf.Sin (2 * Mathf.PI * p.x + t);
	}

	public enum FunctionOption {
		Linear,
		Exponential,
		Parabola,
		Sine
	}

	public FunctionOption function;
}
