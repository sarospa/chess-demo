using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceCont : MonoBehaviour {
	private bool selected;
	private Vector3 loc;
	private int xLoc;
	private int yLoc;
	public string pieceType;
	public string color;
	private bool moved = false;
	private List<Vector2> moves = new List<Vector2> ();
	private GameObject board;

	void Start ()
	{
		board = GameObject.FindWithTag ("GameController");
	}

	void OnMouseDown ()
	{
		if ((color == "white") == BoardCont.whiteTurn)
		{
			moves = CheckValidMoves ();
			board.GetComponent<BoardCont> ().PlaceHighlights (moves);
			xLoc = (int)((this.gameObject.transform.localPosition.x + BoardCont.squareSize * BoardCont.boardSize / 2) / BoardCont.squareSize);
			yLoc = (int)((this.gameObject.transform.localPosition.y + BoardCont.squareSize * BoardCont.boardSize / 2) / BoardCont.squareSize);
			loc = this.gameObject.transform.localPosition;
			BoardCont.selectedPiece = this.gameObject;
			selected = true;
		}
	}

	void OnMouseUp ()
	{
		if (selected)
		{
			int x = Mathf.FloorToInt ((this.gameObject.transform.localPosition.x + BoardCont.squareSize * 4) / BoardCont.squareSize);
			int y = Mathf.FloorToInt ((this.gameObject.transform.localPosition.y + BoardCont.squareSize * 4) / BoardCont.squareSize);
			if (moves.Contains (new Vector2 (x, y)))
			{
				if (BoardCont.pieces[x, y] != null)
				{
					Destroy (BoardCont.pieces[x, y]);
				}
				BoardCont.pieces[x, y] = this.gameObject;
				BoardCont.pieces[xLoc, yLoc] = null;
				BoardCont.PlacePiece (this.gameObject, x, y);
				moved = true;
				BoardCont.whiteTurn = (color != "white");
			}
			else
			{
				this.gameObject.transform.localPosition = loc;
			}
			BoardCont.selectedPiece = null;
			selected = false;
			moves.Clear();
			board.GetComponent<BoardCont> ().RemoveHighlights ();
		}
	}

	public void SetLoc (int x, int y)
	{
		if (x >= 0 && x < BoardCont.boardSize && y >= 0 && y < BoardCont.boardSize)
		{
			xLoc = x;
			yLoc = y;
		}
	}

	string CheckSpace (int x, int y)
	{
		if (x < 0 || x >= BoardCont.boardSize || y < 0 || y >= BoardCont.boardSize)
		{
			return "invalid";
		}
		if (BoardCont.pieces[x, y] == null)
		{
			return "empty";
		}
		if (BoardCont.pieces[x, y].GetComponent<PieceCont>().color == this.color)
		{
			return "ally";
		}
		else
		{
			return "enemy";
		}
	}

	public List<Vector2> CheckValidMoves ()
	{
		moves = new List<Vector2> ();
		switch (pieceType)
		{
		case "pawn":
			int dir;
			if (color == "white")
			{
				dir = 1;
			}
			else
			{
				dir = -1;
			}
			if (TryMove (xLoc, yLoc+dir, false) && !moved)
			{
				TryMove (xLoc, yLoc+dir*2, false);
			}
			if (CheckSpace (xLoc+1, yLoc+dir) == "enemy")
			{
				moves.Add (new Vector2 (xLoc+1, yLoc+dir));
			}
			if (CheckSpace (xLoc-1, yLoc+dir) == "enemy")
			{
				moves.Add (new Vector2 (xLoc-1, yLoc+dir));
			}
			break;
		case "rook":
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc+i, yLoc, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc-i, yLoc, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc, yLoc+i, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc, yLoc-i, true))
				{
					break;
				}
			}
			break;
		case "bishop":
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc+i, yLoc+i, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc+i, yLoc-i, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc-i, yLoc+i, true))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (xLoc-i, yLoc-i, true))
				{
					break;
				}
			}
			break;
		case "knight":
			TryMove (xLoc+1, yLoc+2, true);
			TryMove (xLoc+2, yLoc+1, true);
			TryMove (xLoc+1, yLoc-2, true);
			TryMove (xLoc+2, yLoc-1, true);
			TryMove (xLoc-1, yLoc+2, true);
			TryMove (xLoc-2, yLoc+1, true);
			TryMove (xLoc-1, yLoc-2, true);
			TryMove (xLoc-2, yLoc-1, true);
			break;
		}
		return moves;
	}

	// Returns true if the space is empy, and false if it is invalid or occupied.
	// If capture is true and the space is occupied by an enemy, it returns false but still adds the move to the valid list.
	private bool TryMove (int x, int y, bool capture)
	{
		string cell = CheckSpace (x, y);
		if (cell == "invalid" || cell == "ally")
		{
			return false;
		}
		if (cell == "enemy")
		{
			if (capture)
			{
				moves.Add (new Vector2 (x, y));
			}
			return false;
		}
		moves.Add (new Vector2 (x, y));
		return true;
	}
}
