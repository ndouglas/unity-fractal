using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

  [SerializeField]
	Mesh mesh;

  [SerializeField]
	Material material;
	
  [SerializeField,Range(1,8)]
  int maxDepth = 4;

  [SerializeField,Range(0,1)]
  float childScale = 0.5f;

	private int depth;

  private Material[] materials;

	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back,
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f),

	};

  private void InitializeMaterials () {
		materials = new Material[maxDepth + 1];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i] = new Material(material);
			materials[i].color =
				Color.Lerp(Color.blue, Color.yellow, (float)i / maxDepth);
		}
    materials[maxDepth].color = Color.magenta;
	}

	private void Start () {
		if (materials == null) {
			InitializeMaterials();
		}
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = materials[depth];
		if (depth < maxDepth) {
      StartCoroutine(CreateChildren());
		}
	}

	private IEnumerator CreateChildren () {
		for (int i = 0; i < childDirections.Length; i++) {
			yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
			new GameObject("Fractal Child").AddComponent<Fractal>().
				Initialize(this, i);
		}
	}

	private void Initialize (Fractal parent, int childIndex) {
		mesh = parent.mesh;
    materials = parent.materials;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition =
			childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
	}

	private void Update () {
		transform.Rotate(0f, 30f * Time.deltaTime, 0f);
	}

}
