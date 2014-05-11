using System;
using System.Windows.Forms;
using Checkers99GAME;

namespace Checkers99UI
{
    public interface IEngine
    {
        Boolean IsMoveLegal(Int32 startSquare, Int32 endSquare, Player player);
        Move GetMove(Int32 startSquare, Int32 endSquare, Player player);
        void UpdateBoardState(Byte startSquare, Byte endSquare, Control.ControlCollection formControls);
		Boolean CanPlayerMove(String playerColor);
    }
}
