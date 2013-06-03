using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Badminton.Screens
{
	/// <summary>
	/// A map select screen for single player games
	/// </summary>
	class SingleMapSelect : GameScreen
	{
		private bool enterPressed;

		public SingleMapSelect()
		{
			enterPressed = true;
		}

		public GameScreen Update()
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				if (!enterPressed)
				{
					enterPressed = true;
					return LoadMap(); // Change this when there are actual choices
				}
			}
			else
				enterPressed = false;

			return this;
		}

		private GameScreen LoadMap()
		{
			return new SingleMap();
		}

		public void Draw(SpriteBatch sb)
		{
			sb.DrawString(MainGame.basicFont, "This'll be a level select eventually. Press enter to go to test level.", Vector2.Zero, Color.Black);
		}
	}
}
