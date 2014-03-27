using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardCont : MonoBehaviour
{
	public GameObject whiteSquare;
	public GameObject blackSquare;
	public GameObject whitePawn;
	public GameObject blackPawn;
	public GameObject whiteRook;
	public GameObject blackRook;
	public GameObject whiteBishop;
	public GameObject blackBishop;
	public GameObject whiteKnight;
	public GameObject blackKnight;
	public GameObject whiteQueen;
	public GameObject blackQueen;
	public GameObject whiteKing;
	public GameObject blackKing;
	public GameObject highlight;
	public const float corner = -0.42f;
	public const float squareSize = 0.12f;
	public const float pieceScale = 0.9f;
	public const int boardSize = 8;

	public static bool whiteTurn = true;
	public static GameObject[,] pieces;
	public static GameObject selectedPiece = null;
	public static float xScale;
	public static float yScale;
	private List<GameObject> highlightSquares = new List<GameObject> ();

	void Start ()
	{
		GameObject turnText = GameObject.FindGameObjectWithTag ("TurnText");
		turnText.guiText.text = "White's Turn";
		turnText.transform.position = new Vector3 (0.02f, 0.02f, 0);
		turnText.guiText.anchor = TextAnchor.LowerLeft;
		pieces = new GameObject[boardSize,boardSize];
		for (int x = 0; x < boardSize; x++)
		{
			for (int y = 0; y < boardSize; y++)
			{
				if ((x + y) % 2 == 1)
				{
					CreateSquare (whiteSquare, x, y);
				}
				else
				{
					CreateSquare (blackSquare, x, y);
				}
			}
		}
		for (int x = 0; x < boardSize; x++)
		{
			CreatePiece (whitePawn, x, 1);
			CreatePiece (blackPawn, x, 6);
		}
		CreatePiece (whiteRook, 0, 0);
		CreatePiece (whiteBishop, 1, 0);
		CreatePiece (whiteKnight, 2, 0);
		CreatePiece (whiteQueen, 3, 0);
		CreatePiece (whiteKing, 4, 0);
		CreatePiece (whiteKnight, 5, 0);
		CreatePiece (whiteBishop, 6, 0);
		CreatePiece (whiteRook, 7, 0);
		CreatePiece (blackRook, 0, 7);
		CreatePiece (blackBishop, 1, 7);
		CreatePiece (blackKnight, 2, 7);
		CreatePiece (blackQueen, 3, 7);
		CreatePiece (blackKing, 4, 7);
		CreatePiece (blackKnight, 5, 7);
		CreatePiece (blackBishop, 6, 7);
		CreatePiece (blackRook, 7, 7);
	}
	
	void Update () {
		xScale = (float)Screen.width / Mathf.Max (new int[2] {Screen.width, Screen.height}) * Camera.main.orthographicSize * 2;
		yScale = (float)Screen.height / Mathf.Max (new int[2] {Screen.width, Screen.height}) * Camera.main.orthographicSize * 2;
		if (selectedPiece != null)
		{
			selectedPiece.transform.position = new Vector3
			(
				(Input.mousePosition.x - Screen.width / 2) / Screen.width * xScale,
				(Input.mousePosition.y - Screen.height / 2) / Screen.height * yScale,
				-3
			);
		}
	}

	static Vector3 CellLoc (int x, int y, float z)
	{
		return new Vector3 (corner + (squareSize*x), corner + (squareSize*y), z);
	}

	void CreateSquare (GameObject square, int x, int y)
	{
		GameObject clone = (GameObject)Instantiate (square);
		clone.transform.parent = this.transform;
		clone.transform.localPosition = CellLoc (x, y, -1f);
		clone.transform.localScale = new Vector3 (squareSize, squareSize, 1);
	}

	void CreatePiece (GameObject piece, int x, int y)
	{
		GameObject clone = (GameObject)Instantiate (piece);
		clone.transform.parent = this.transform;
		clone.transform.localPosition = CellLoc (x, y, -2f);
		clone.transform.localScale = new Vector3 (squareSize * pieceScale, squareSize * pieceScale, 1);
		PieceCont controller = clone.GetComponent<PieceCont> ();
		controller.SetLoc (x, y);
		pieces [x, y] = clone;
	}

	public static void PlacePiece (GameObject piece, int x, int y)
	{
		piece.transform.localPosition = CellLoc (x, y, -2f);
		pieces [x, y] = piece;
		pieces [x, y].GetComponent<PieceCont> ().SetLoc (x, y);
	}

	public void PlaceHighlights (List<Vector2> moves)
	{
		foreach (Vector2 move in moves)
		{
			GameObject clone = (GameObject)Instantiate (highlight);
			clone.transform.parent = this.transform;
			clone.transform.localPosition = CellLoc ((int)move.x, (int)move.y, -2.5f);
			clone.transform.localScale = new Vector3 (squareSize, squareSize, 1);
			highlightSquares.Add (clone);
		}
	}

	public void RemoveHighlights ()
	{
		foreach (GameObject clone in highlightSquares)
		{
			Destroy (clone);
		}
	}

	// If any elements in additional are missing from target, they're added into it.
	public static void MergeLists (List<Vector2> target, List<Vector2> additional)
	{
		foreach (Vector2 v in additional)
		{
			if (!target.Contains (v))
			{
				target.Add (v);
			}
		}
	}

	// If any elements are in target, but missing from intersection, they're removed from target.
	public static void IntersectLists (List<Vector2> target, List<Vector2> intersection)
	{
		for (int i = 0; i < target.Count; i++)
		{
			if (!intersection.Contains (target[i]))
			{
				target.Remove (target[i]);
				i--;
			}
		}
	}
	
	// If any elements are in both target and subtraction, they are removed from target.
	public static void SubtractLists (List<Vector2> target, List<Vector2> subtraction)
	{
		foreach (Vector2 v in subtraction)
		{
			if (target.Contains (v))
			{
				target.Remove (v);
			}
		}
	}
}
