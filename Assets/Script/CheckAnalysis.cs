using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckAnalysis : MonoBehaviour
{
	PieceCont controller;

	void Start ()
	{
		controller = this.gameObject.GetComponent<PieceCont> ();
	}

	// If forThreatDisplay is true, adds the king's location to the list and not
	// the attacking piece's.
	public List<List<Vector2>> findThreats (bool forThreatDisplay)
	{
		Vector2 loc = controller.GetLoc ();
		List<List<Vector2>> list = new List<List<Vector2>> ();
		seekPawns (loc, list, forThreatDisplay);
		seekKnights (loc, list, forThreatDisplay);
		seekRooks (loc, list, forThreatDisplay);
		seekBishops (loc, list, forThreatDisplay);
		if (forThreatDisplay && list.Count > 0)
		{
			List<Vector2> kingLoc = new List<Vector2> ();
			kingLoc.Add (loc);
			list.Add (kingLoc);
		}
		return list;
	}
	
	private void seekPawns (Vector2 loc, List<List<Vector2>> list, bool forThreatDisplay)
	{
		int yDir;
		if (controller.color == "white")
		{
			yDir = 1;
		}
		else
		{
			yDir = -1;
		}
		Vector2[] pawnMoves = {new Vector2 (1, yDir), new Vector2 (-1, yDir)};
		foreach (Vector2 move in pawnMoves)
		{
			int x = (int)loc.x + (int)move.x;
			int y = (int)loc.y + (int)move.y;
			string cell = controller.CheckSpace (x, y);
			if (cell == "enemy" && BoardCont.pieces[x, y].GetComponent<PieceCont> ().pieceType == "pawn")
			{
				List<Vector2> pawn = new List<Vector2> ();
				if (!forThreatDisplay)
				{
					pawn.Add (new Vector2 (x, y));
				}
				list.Add (pawn);
			}
		}
	}
	
	private void seekKnights (Vector2 loc, List<List<Vector2>> list, bool forThreatDisplay)
	{
		Vector2[] knightMoves =  {new Vector2 (1, 2), new Vector2 (2, 1), new Vector2 (-1, 2), new Vector2 (-2, 1)
									, new Vector2(1, -2), new Vector2 (2, -1), new Vector2 (-1, -2), new Vector2 (-2, -1)};
		foreach (Vector2 move in knightMoves)
		{
			int x = (int)loc.x + (int)move.x;
			int y = (int)loc.y + (int)move.y;
			string cell = controller.CheckSpace (x, y);
			if (cell == "enemy" && BoardCont.pieces[x, y].GetComponent<PieceCont> ().pieceType == "knight")
			{
				List<Vector2> knight = new List<Vector2> ();
				if (!forThreatDisplay)
				{
					knight.Add (new Vector2 (x, y));
				}
				list.Add (knight);
			}
		}
	}
	
	private void seekRooks (Vector2 loc, List<List<Vector2>> list, bool forThreatDisplay)
	{
		List<Vector2> rook1 = seekRook (loc, true, true, forThreatDisplay);
		List<Vector2> rook2 = seekRook (loc, true, false, forThreatDisplay);
		List<Vector2> rook3 = seekRook (loc, false, true, forThreatDisplay);
		List<Vector2> rook4 = seekRook (loc, false, false, forThreatDisplay);
		if (rook1 != null) list.Add (rook1);
		if (rook2 != null) list.Add (rook2);
		if (rook3 != null) list.Add (rook3);
		if (rook4 != null) list.Add (rook4);
	}
	
	private List<Vector2> seekRook (Vector2 loc, bool horizontal, bool positive, bool forThreatDisplay)
	{
		int dir;
		int xDir;
		int yDir;
		List<Vector2> threatMoves = new List<Vector2> ();
		if (positive)
		{
			dir = 1;
		}
		else
		{
			dir = -1;
		}
		if (horizontal)
		{
			xDir = dir;
			yDir = 0;
		}
		else
		{
			xDir = 0;
			yDir = dir;
		}
		for (int i = 1; ; i++)
		{
			int checkX = (int)loc.x + xDir*i;
			int checkY = (int)loc.y + yDir*i;
			threatMoves.Add (new Vector2 (checkX, checkY));
			string cell = controller.CheckSpace (checkX, checkY);
			if (cell == "enemy" || cell == "ally")
			{
				PieceCont otherCont = BoardCont.pieces[checkX, checkY].GetComponent<PieceCont> ();
				if (cell == "enemy" && (otherCont.pieceType == "rook" || otherCont.pieceType == "queen"))
				{
					if (forThreatDisplay)
					{
						threatMoves.Remove (otherCont.GetLoc ());
					}
					return threatMoves;
				}
				else
				{
					return null;
				}
			}
			else if (cell == "invalid")
			{
				return null;
			}
		}
	}
	
	private void seekBishops (Vector2 loc, List<List<Vector2>> list, bool forThreatDisplay)
	{
		List<Vector2> bishop1 = seekBishop(loc, true, true, forThreatDisplay);
		List<Vector2> bishop2 = seekBishop(loc, true, false, forThreatDisplay);
		List<Vector2> bishop3 = seekBishop(loc, false, true, forThreatDisplay);
		List<Vector2> bishop4 = seekBishop(loc, false, false, forThreatDisplay);
		if (bishop1 != null) list.Add (bishop1);
		if (bishop2 != null) list.Add (bishop2);
		if (bishop3 != null) list.Add (bishop3);
		if (bishop4 != null) list.Add (bishop4);
	}

	private List<Vector2> seekBishop (Vector2 loc, bool up, bool right, bool forThreatDisplay)
	{
		int xDir;
		int yDir;
		List<Vector2> threatMoves = new List<Vector2> ();
		if (right)
		{
			xDir = 1;
		}
		else
		{
			xDir = -1;
		}
		if (up)
		{
			yDir = 1;
		}
		else
		{
			yDir = -1;
		}
		for (int i = 1; ; i++)
		{
			int checkX = (int)loc.x + xDir*i;
			int checkY = (int)loc.y + yDir*i;
			threatMoves.Add (new Vector2 (checkX, checkY));
			string cell = controller.CheckSpace (checkX, checkY);
			if (cell == "enemy" || cell == "ally")
			{
				PieceCont otherCont = BoardCont.pieces[checkX, checkY].GetComponent<PieceCont> ();
				if (cell == "enemy" && (otherCont.pieceType == "bishop" || otherCont.pieceType == "queen"))
				{
					if (forThreatDisplay)
					{
						threatMoves.Remove (otherCont.GetLoc ());
					}
					return threatMoves;
				}
				else
				{
					return null;
				}
			}
			else if (cell == "invalid")
			{
				return null;
			}
		}
	}
}
