using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Grapher2 : MonoBehaviour {

	public Dropdown myDropdown;

    int[] resArray = {40,
        30,
        40,
        40, //Sinusoidal
        40, //Ripple
        40, //Ellipsoid
        40, //EllipticParaboloid
        40, //HyperbolicParaboloid
        40, //Cone
        40, //HyperboloidOfOneSheet
        40, //HyperboloidOfTwoSheets
        40, //Torus
        40 }; //SineXY


	void Destroy() {
		myDropdown.onValueChanged.RemoveAllListeners ();
	}

	private void myDropdownValueChangedHandler(Dropdown target) {
		Debug.Log ("selected: " + target.value);
		selection = target.value;
	}

	public void SetDropdownIndex(int index) {
		myDropdown.value = index;
        resolution = resArray[index];
        
	}

	public GameObject sliderA;
	public GameObject sliderB;
	public GameObject sliderC;
	public GameObject sliderD;
	public GameObject DropDown;

	Text textA;
	Text textB;
	Text textC;
	Text textD;
	Text dropDown;

	void Start() {
		myDropdown.onValueChanged.AddListener (delegate {
			myDropdownValueChangedHandler (myDropdown);
		});
		textA = sliderA.GetComponent<Text> ();
		textB = sliderB.GetComponent<Text> ();
		textC = sliderC.GetComponent<Text> ();
		textD = sliderD.GetComponent<Text> ();
		dropDown = DropDown.GetComponent<Text> ();
	}

	[Range(0.1f,10f)]
	public float A = 1f;

	[Range(0.1f,10f)]
	public float B = 1f;

	[Range(0.1f,10f)]
	public float C = 1f;

	[Range(0.1f,10f)]
	public float D = 1f;

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
		points = new ParticleSystem.Particle[8 * resolution * resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for (int x = - resolution; x < resolution; x++) {
			for (int z = - resolution; z < resolution; z++) {
				Vector3 p = new Vector3 (x * increment, 0f, z * increment);
				points [i].position = p;
				points [i].color = new Color (p.x, 0f, p.z);
				points [i++].size = 0.1f;
			}
		}
		for (int x = -resolution; x < resolution; x++) {
			for (int z = -resolution; z < resolution; z++) {
				Vector3 p = new Vector3 (x * increment, -1f, z * increment);
				points [i].position = p;
				points [i].color = new Color (p.x, -1f, p.z);
				points [i++].size = 0.1f;
			}
		}
	}

	public enum FunctionOption {
		Linear,
		Quadratic,
		Parabola,
		Sinusoidal,
		Ripple,
		Ellipsoid,
		EllipticParaboloid,
		HyperbolicParaboloid,
		Cone,
		HyperboloidOfOneSheet,
		HyperboloidOfTwoSheets,
		Torus,
		SineXY,
	}

	public FunctionOption function;
	public int selection = 0;
	private int currentSelection;

	private delegate float FunctionDelegate (Vector3 p, float t, float A, float B, float C, float D);

	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Quadratic,
		Parabola,
		Sinusoidal,
		Ripple,
		Ellipsoid,//5
		EllipticParaboloid,
		HyperbolicParaboloid,
		Cone,//8
		HyperboloidOfOneSheet,//9
		HyperboloidOfTwoSheets,//10
		Torus,//11
		SineXY,
	};

	void Update () {

		if (currentResolution != resolution ||
			points == null) {
			CreatePoints ();
		}

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (Canvas.activeSelf)
            {
                Application.Quit();
            }

            else
            {
                displayMenu();
            }
        }

        textA.text = "A: "+ A.ToString ();
		textB.text = "B: "+ B.ToString ();
		textC.text = "C: "+ C.ToString ();
		textD.text = "D: "+ D.ToString ();
		float t = Time.timeSinceLevelLoad;
		FunctionDelegate f = functionDelegates [selection];
		if (selection == 5 ||
			selection == 8 ||
			selection == 9 ||
			selection == 10 ||
			selection == 11) {
			for (int i = 0; i < points.Length / 2; i++) {
				Vector3 p = points [i].position;
				p.y = -f(p, t, A, B, C, D);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			}
			for (int i = points.Length/2 + 1; i < points.Length; i++) {
				Vector3 p = points [i].position;
				p.y = f(p, t, A, B, C, D);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			}
		}
		else {
			for (int i = 0; i < points.Length; i++) {
				Vector3 p = points [i].position;
				p.y = f(p, t, A, B, C, D);
				points [i].position = p;
				Color c = points [i].color;
				c.g = p.y;
				points [i].color = c;
			}
		}
		GetComponent<ParticleSystem>().SetParticles (points, points.Length);
		printFunction (selection);
	}

	public void adjustA (float newA) {
		A = newA;
	}

	public void adjustB (float newB) {
		B = newB;
	}

	public void adjustC (float newC) {
		C = newC;
	}

	public void adjustD (float newD) {
		D = newD;
	}

	public void adjustRes (int newRes) {
		resolution = newRes;
	}

	private static float Linear (Vector3 p, float t, float A, float B, float C, float D) {
		return A * p.x;
	}

	private static float Quadratic (Vector3 p, float t, float A, float B, float C, float D) {
		return A * p.x * p.x;
	}

	private static float Parabola (Vector3 p, float t, float A, float B, float C, float D) {
		p.x = p.x - A;
		p.z = p.z - B;
		return D - C * p.x * p.x * p.z * p.z;
	}

	private static float Sinusoidal (Vector3 p, float t, float A, float B, float C, float D) {
		return D + 
			A * Mathf.Sin (4f * Mathf.PI * p.x + 4f * t) * Mathf.Sin(2f * Mathf.PI * p.z + t) +
			B * Mathf.Cos(3f * Mathf.PI * p.z + 5f*t) * Mathf.Cos(5f * Mathf.PI * p.z + 3f*t) +
			C * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}

	private static float Ripple (Vector3 p, float t, float A, float B, float C, float D) {
		float squareRadius = p.x * p.x + p.z * p.z;
		return Mathf.Sin (15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
	}

	private static float Ellipsoid (Vector3 p, float t, float A, float B, float C, float D) {
		return C * Mathf.Abs( Mathf.Sqrt (D*D - p.x * p.x /(A * A) - p.z * p.z / (B * B)));
	}

	private static float HyperbolicParaboloid (Vector3 p, float t, float A, float B, float C, float D) {
		return C * (- p.x * p.x / (A * A) + p.z * p.z / (B * B));
	}

	private static float EllipticParaboloid (Vector3 p, float t, float A, float B, float C, float D) {
		return C * (p.x * p.x / (A*A) + p.z * p.z / (B * B));
	}

	private static float Cone (Vector3 p, float t, float A, float B, float C, float D) {
		return C * C * Mathf.Abs (Mathf.Sqrt (p.x * p.x / (A * A) + p.z * p.z / (B * B)));
	}

	private static float HyperboloidOfOneSheet (Vector3 p, float t, float A, float B, float C, float D) {
		return C * C * Mathf.Abs (Mathf.Sqrt (-D + p.x * p.x / (A * A) + p.z * p.z / (B * B)));
	}

	private static float HyperboloidOfTwoSheets (Vector3 p, float t, float A, float B, float C, float D) {
		return C * C * Mathf.Abs (Mathf.Sqrt (D + p.x * p.x / (A * A) + p.z * p.z / (B * B)));
	}

	private static float Torus (Vector3 p, float t, float A, float B, float C, float D) {
		return C * C * Mathf.Abs (Mathf.Sqrt (-(Mathf.Sqrt ( p.x * p.x / (A * A) + p.z * p.z / (B * B)) - 2 * D) * (Mathf.Sqrt ( p.x * p.x / (A * A) + p.z * p.z / (B * B)) - 2 * D) + D * D));
	}

	private static float SineXY (Vector3 p, float t, float A, float B, float C, float D) {
		return C * Mathf.Sin (A * p.x * p.z + B * t);
	}

	public GameObject Canvas;

	public void displayGraph () {
		Canvas.SetActive(false);
	}

	public void displayMenu() {
		Canvas.SetActive (true);
	}

	public void resetValues() {
		A = 1f;
		B = 1f;
		C = 1f;
		D = 1f;
	}

	void printFunction (int selection) {
		if (currentSelection != selection) {
			resetValues ();
			currentSelection = selection;
		}
		if (selection == 0) {
			dropDown.text = "z = A*x";
		} else if (selection == 1) {
			dropDown.text = "z = A * x^2";
		} else if (selection == 2) {
			dropDown.text = "z = D - C * (x - A)^2 (z - B)^2";
		} else if (selection == 3) {
			dropDown.text = "z = D + Asin(4*pi*x + 4t)sin(2*pi*y + t) + " +
			"Bcos(3*pi*y + 5t)cos(5*pi*y + 3t) + " +
			"Csin(pi*x + 0.6t)";
		} else if (selection == 4) {
			dropDown.text = "z = sin(15*pi*(x^2)*(y^2) - 2t)/(2 + 100*(x^2)*(y^2))";
		} else if (selection == 5) {
			dropDown.text = "x^2/A^2 + y^2/B^2 + z^2/C^2 = D^2";
		} else if (selection == 6) {
			dropDown.text = "z/C = x^2/A^2 - y^2/B^2";
		} else if (selection == 7) {
			dropDown.text = "z/C = x^2/A^2 + y^2/B^2";
		} else if (selection == 8) {
			dropDown.text = "z^2/C^2 = x^2/A62 + y^2/B^2";
		} else if (selection == 9) {
			dropDown.text = "x^2/A^2 + y^2/B^2 - z^2/C^2 = 1";
		} else if (selection == 10) {
			dropDown.text = "-x^2/A^2 - y^2/B^2 + z^2/C^2 = 1";
		} else if (selection == 11) {
			dropDown.text = "(sqrt(x^2/A^2 + y^2/B^2) - 2*D)^2 + z^2/C^2 = D^2";
		} else if (selection == 12) {
			dropDown.text = "z/C = sin(Axy + Bt)";
		}
	}
}
