using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Checkers99GAME
{
	public sealed class Player
	{
		public enum PlayerColor
		{
			NONE = 0,
			WHITE,
			RED
		};
		public enum PlayerType
		{
			HUMAN,
			COMPUTER
		};
		public PlayerColor Color {get; private set;}
		public PlayerType Type {get; private set;}
		public List<Move> LegalMoves { get; set; }
		public List<Move> LegalJumps { get; set; }

		public Player(PlayerColor color, PlayerType type)
		{
			Color = color;
			Type = type;
			LegalJumps = new List<Move>();
			LegalMoves = new List<Move>();
		}
		public void SetColor(PlayerColor color)
		{
			Color = color;
		}
		public static Player.PlayerColor GetCheckerColor(Engine.Piece checker)
		{
			PlayerColor result = PlayerColor.NONE;
			switch (checker)
			{
				case Engine.Piece.WhiteKing:
				case Engine.Piece.WhiteChecker:
					result = PlayerColor.WHITE;
					break;
				case Engine.Piece.RedKing:
				case Engine.Piece.RedChecker:
					result = PlayerColor.RED;
					break;
			}
			return result;
		}
				
	}
}
