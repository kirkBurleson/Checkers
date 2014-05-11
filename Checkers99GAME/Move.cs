using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers99GAME
{
	public sealed class Move
	{
		public Single Score { get; private set; }
		public Byte StartSquare { get; private set; }
		public Byte EndSquare { get; private set; }
		public Boolean IsJump { get; private set; } //if true, use JumpLandingSquares instead of EndSquare
		public List<Byte> JumpLandingSquares { get; private set; }
		public List<Move> Jumps { get; internal set; }
		public Boolean WillKing { get; internal set; }

		public Move(Byte startSquare, Byte endSquare, Boolean isJump)
		{
			StartSquare = startSquare;
			EndSquare = endSquare;
			IsJump = isJump;
			Score = 0f;
			Jumps = new List<Move>();
			JumpLandingSquares = new List<Byte>();
			WillKing = false;
		}
		public void SetScore(Single score)
		{
			Score = score;
		}
		public void AddToScore(Single score)
		{
			Score += score;
		}
		public void AddJump(Move move)
		{
			Jumps.Add(move);
		}
		public void AddJumpList(List<Move> moveList)
		{
			Jumps = moveList;
		}
		public void AddJumpLandingSquare(Byte jump)
		{
			JumpLandingSquares.Add(jump);
		}
		public void AddJumpLandingSquareList(List<Byte> jumpList)
		{
			JumpLandingSquares = jumpList;
		}
		public Boolean HasChildMoves
		{
			get { return Jumps.Count > 0; }
		}
		public override string ToString()
		{
			String data = "Score: " + Score.ToString() + Environment.NewLine +
										"Start Square: " + StartSquare.ToString() + Environment.NewLine +
										"End Square: " + EndSquare.ToString() + Environment.NewLine +
										"Is a Jump: " + IsJump.ToString() + Environment.NewLine;
			return data;
		}		

    private Boolean KingCheck(Byte endSquare, Byte checker)
    {
      switch (checker)
      {
        case 1:
          return endSquare < 8;
        case 2:
          return endSquare > 56;
        default:
          return false;
      }
    }
	}
}
