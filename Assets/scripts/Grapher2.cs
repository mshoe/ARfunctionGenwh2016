using UnityEngine;
using System.Collections;

public class Grapher2 : MonoBehaviour {

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
		points = new ParticleSystem.Particle[8*resolution * resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for (int x = -resolution; x < resolution; x++) {
			for (int z = -resolution; z < resolution; z++) {
				Vector3 p = new Vector3 (x * increment, 0f, z * increment);
				points [i].position = p;
				points [i].color = new Color (p.x, 0f, p.z);
				points [i++].size = 0.1f;
			}
		}
	}

	public enum FunctionOption {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		Sphere
	}

	public FunctionOption function;

	private delegate float FunctionDelegate (Vector3 p, float t);

	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		Sphere
	};

	void Update () {
		if (currentResolution != resolution || points == null) {
			CreatePoints ();
		}
		float t = Time.timeSinceLevelLoad;
		FunctionDelegate f = functionDelegates [(int)function];
		if ((int)function == 5) {
			for (int i = 0; i < points.Length / 2; i++) {
				Vector3 p = points [i].position;
				p.y = -f(p, t);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			
			}
			for (int i = points.Length/2; i < points.Length; i++) {
				Vector3 p = points [i].position;
				p.y = f(p, t);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			}
		}
		else {
			for (int i = 0; i < points.Length; i++) {
				Vector3 p = points [i].position;
				p.y = f(p, t);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			}
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
		p.x += p.x - 1f;
		p.z += p.z - 1f;
		return 1f - p.x * p.x * p.z * p.z;
	}

	private static float Sine (Vector3 p, float t) {
		return 0.5f + 
			0.25f * Mathf.Sin (4f * Mathf.PI * p.x + 4f * t) * Mathf.Sin(2f * Mathf.PI * p.z + t) +
			0.10f * Mathf.Cos(3f * Mathf.PI * p.z + 5f*t) * Mathf.Cos(5f * Mathf.PI * p.z + 3f*t) +
			0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}

	private static float Ripple (Vector3 p, float t) {
		p.x -= 0.5f;
		p.z -= 0.5f;
		float squareRadius = p.x * p.x + p.z * p.z;
		return 0.5f + Mathf.Sin (15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
	}

	private static float Sphere (Vector3 p, float t) {
		return Mathf.Abs( Mathf.Sqrt (1 - p.x * p.x - p.z * p.z));
	}
}
