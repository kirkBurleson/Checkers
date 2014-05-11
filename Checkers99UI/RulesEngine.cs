using System;
using System.Windows.Forms;
using Checkers99GAME;

namespace Checkers99UI
{
    public class RulesEngine : IEngine
    {
        private Engine engine = new Engine();

        public Boolean IsMoveLegal(Int32 startSquare, Int32 endSquare, Player player)
        {
            return false;
        }

        public Move GetMove(Int32 startSquare, Int32 endSquare, Player player)
        {
          return null;
        }

        public void UpdateBoardState(Byte startSquare, Byte endSquare, Control.ControlCollection formControls)
        {

        }

		public Boolean CanPlayerMove(String playerColor)
		{
			return false;
		}
    }
}
