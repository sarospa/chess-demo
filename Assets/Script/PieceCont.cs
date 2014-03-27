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
			moves = CheckValidMoves (false);
			// Enforces the king not moving into check and implements castling
			if (this.pieceType == "king")
			{
				List<Vector2> castleLeft = new List<Vector2> ();
				List<Vector2> castleRight = new List<Vector2> ();
				if (!this.moved)
				{
					string cell1 = CheckSpace (xLoc-1, yLoc);
					string cell2 = CheckSpace (xLoc-2, yLoc);
					string cell3 = CheckSpace (xLoc-3, yLoc);
					string cell4 = CheckSpace (xLoc-4, yLoc);
					if (cell1 == "empty" && cell2 == "empty" && cell3 == "empty" && cell4 == "ally")
					{
						PieceCont otherCont = BoardCont.pieces[xLoc-4, yLoc].GetComponent<PieceCont> ();
						if (otherCont.pieceType == "rook" && !otherCont.moved)
						{
							castleLeft.Add (new Vector2 (xLoc, yLoc));
							castleLeft.Add (new Vector2 (xLoc-1, yLoc));
							castleLeft.Add (new Vector2 (xLoc-2, yLoc));
						}
					}
					cell1 = CheckSpace (xLoc+1, yLoc);
					cell2 = CheckSpace (xLoc+2, yLoc);
					cell3 = CheckSpace (xLoc+3, yLoc);
					if (cell1 == "empty" && cell2 == "empty" && cell3 == "ally")
					{
						PieceCont otherCont = BoardCont.pieces[xLoc+3, yLoc].GetComponent<PieceCont> ();
						if (otherCont.pieceType == "rook" && !otherCont.moved)
						{
							castleRight.Add (new Vector2 (xLoc, yLoc));
							castleRight.Add (new Vector2 (xLoc+1, yLoc));
							castleRight.Add (new Vector2 (xLoc+2, yLoc));
						}
					}
				}
				BoardCont.pieces[xLoc, yLoc] = null;
				foreach (GameObject piece in BoardCont.pieces)
				{
					if (piece != null)
					{
						PieceCont otherCont = piece.GetComponent<PieceCont> ();
						if (otherCont.color != this.color)
						{
							BoardCont.SubtractLists (moves, otherCont.CheckValidMoves (true));
							BoardCont.SubtractLists (castleLeft, otherCont.CheckValidMoves (true));
							BoardCont.SubtractLists (castleRight, otherCont.CheckValidMoves (true));
						}
					}
				}
				if (castleLeft.Count == 3)
				{
					moves.Add (new Vector2 (xLoc-2, yLoc));
				}
				if (castleRight.Count == 3)
				{
					moves.Add (new Vector2 (xLoc+2, yLoc));
				}
				BoardCont.pieces[xLoc, yLoc] = this.gameObject;
			}
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
			//Debug.Log (xLoc + "," + yLoc + " to " + x + "," + y);
			if (moves.Contains (new Vector2 (x, y)))
			{
				if (BoardCont.pieces[x, y] != null)
				{
					Destroy (BoardCont.pieces[x, y]);
				}
				// Moves rook on queen side castle.
				if (x == xLoc-2 && y == yLoc && this.pieceType == "king")
				{
					BoardCont.PlacePiece (BoardCont.pieces[xLoc-4, yLoc], xLoc-1, yLoc);
					BoardCont.pieces[xLoc-4, yLoc] = null;
				}
				// Moves rook on king side castle.
				else if (x == xLoc+2 && y == yLoc && this.pieceType == "king")
				{
					BoardCont.PlacePiece (BoardCont.pieces[xLoc+3, yLoc], xLoc+1, yLoc);
					BoardCont.pieces[xLoc+3, yLoc] = null;
				}
				BoardCont.pieces[x, y] = this.gameObject;
				BoardCont.pieces[xLoc, yLoc] = null;
				BoardCont.PlacePiece (this.gameObject, x, y);
				moved = true;
				BoardCont.whiteTurn = (color != "white");
				if (BoardCont.whiteTurn)
				{
					GameObject turnText = GameObject.FindGameObjectWithTag ("TurnText");
					turnText.guiText.text = "White's Turn";
					turnText.transform.position = new Vector3 (0.02f, 0.02f, 0);
					turnText.guiText.anchor = TextAnchor.LowerLeft;
				}
				else
				{
					GameObject turnText = GameObject.FindGameObjectWithTag ("TurnText");
					turnText.guiText.text = "Black's Turn";
					turnText.transform.position = new Vector3 (0.02f, 0.98f, 0);
					turnText.guiText.anchor = TextAnchor.UpperLeft;
				}
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

	public Vector2 GetLoc ()
	{
		return new Vector2 (xLoc, yLoc);
	}

	public string CheckSpace (int x, int y)
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

	public List<Vector2> CheckValidMoves (bool protect)
	{
		return CheckValidMoves (pieceType, protect);
	}

	// Protect is used to check which squares are protected by the piece.
	// If protect is true, spaces with allied pieces and empty spaces the piece
	// could capture to are included, while non-capturing moves are left out.
	public List<Vector2> CheckValidMoves (string type, bool protect)
	{
		List<Vector2> nextMoves = new List<Vector2> ();
		switch (type)
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
			if (TryMove (nextMoves, xLoc, yLoc+dir, false, protect) && !moved)
			{
				TryMove (nextMoves, xLoc, yLoc+dir*2, false, protect);
			}
			string cell = CheckSpace (xLoc+1, yLoc+dir);
			if (cell == "enemy" || ((cell == "empty" || cell == "ally") && protect))
			{
				nextMoves.Add (new Vector2 (xLoc+1, yLoc+dir));
			}
			cell = CheckSpace (xLoc-1, yLoc+dir);
			if (cell == "enemy" || ((cell == "empty" || cell == "ally") && protect))
			{
				nextMoves.Add (new Vector2 (xLoc-1, yLoc+dir));
			}
			break;
		case "rook":
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc+i, yLoc, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc-i, yLoc, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc, yLoc+i, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc, yLoc-i, true, protect))
				{
					break;
				}
			}
			break;
		case "bishop":
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc+i, yLoc+i, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc+i, yLoc-i, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc-i, yLoc+i, true, protect))
				{
					break;
				}
			}
			for (int i = 1; ; i++)
			{
				if (!TryMove (nextMoves, xLoc-i, yLoc-i, true, protect))
				{
					break;
				}
			}
			break;
		case "knight":
			TryMove (nextMoves, xLoc+1, yLoc+2, true, protect);
			TryMove (nextMoves, xLoc+2, yLoc+1, true, protect);
			TryMove (nextMoves, xLoc+1, yLoc-2, true, protect);
			TryMove (nextMoves, xLoc+2, yLoc-1, true, protect);
			TryMove (nextMoves, xLoc-1, yLoc+2, true, protect);
			TryMove (nextMoves, xLoc-2, yLoc+1, true, protect);
			TryMove (nextMoves, xLoc-1, yLoc-2, true, protect);
			TryMove (nextMoves, xLoc-2, yLoc-1, true, protect);
			break;
		case "queen":
			nextMoves = CheckValidMoves ("rook", protect);
			BoardCont.MergeLists (nextMoves, CheckValidMoves ("bishop", protect));
			break;
		case "king":
			TryMove (nextMoves, xLoc+1, yLoc+1, true, protect);
			TryMove (nextMoves, xLoc+1, yLoc, true, protect);
			TryMove (nextMoves, xLoc+1, yLoc-1, true, protect);
			TryMove (nextMoves, xLoc, yLoc-1, true, protect);
			TryMove (nextMoves, xLoc-1, yLoc-1, true, protect);
			TryMove (nextMoves, xLoc-1, yLoc, true, protect);
			TryMove (nextMoves, xLoc-1, yLoc+1, true, protect);
			TryMove (nextMoves, xLoc, yLoc+1, true, protect);
			break;
		}
		if (this.pieceType != "king")
		{
			GameObject[] kings = GameObject.FindGameObjectsWithTag ("King");
			//Debug.Log (kings);
			BoardCont.pieces[xLoc, yLoc] = null;
			foreach (GameObject king in kings)
			{
				if (king.GetComponent<PieceCont> ().color == this.color)
				{
					List<List<Vector2>> threats = king.GetComponent<CheckAnalysis> ().findThreats();
					foreach (List<Vector2> threat in threats)
					{
						BoardCont.IntersectLists (nextMoves, threat);
					}
				}
			}
			BoardCont.pieces[xLoc, yLoc] = this.gameObject;
		}
		return nextMoves;
	}

	// Returns true if the space is empy, and false if it is invalid or occupied.
	// If capture is true and the space is occupied by an enemy, it returns false but still adds the move to the valid list.
	// If protect is also true, does the same for allies.
	private bool TryMove (List<Vector2> list, int x, int y, bool capture, bool protect)
	{
		string cell = CheckSpace (x, y);
		if (cell == "invalid")
		{
			return false;
		}
		else if (cell == "enemy")
		{
			if (capture)
			{
				list.Add (new Vector2 (x, y));
			}
			return false;
		}
		else if (cell == "ally")
		{
			if (capture && protect)
			{
				list.Add (new Vector2 (x, y));
			}
			return false;
		}
		else // if (cell == "empty")
		{
			if (capture || !protect)
			{
				list.Add (new Vector2 (x, y));
			}
			return true;
		}
	}
}
