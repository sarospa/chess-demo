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

	void OnMouseDown ()
	{
		if ((color == "white") == BoardCont.whiteTurn)
		{
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
			List<Vector2> moves = CheckValidMoves ();
			if (moves.Contains (new Vector2 (x, y)))
			{
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
		List<Vector2> moves = new List<Vector2> ();
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
			if (CheckSpace (xLoc, yLoc+dir) == "empty")
			{
				moves.Add (new Vector2 (xLoc, yLoc+dir));
				if (!moved && CheckSpace (xLoc, yLoc+dir*2) == "empty")
				{
					moves.Add (new Vector2 (xLoc, yLoc+dir*2));
				}
			}
			break;
		}
		return moves;
	}
}
