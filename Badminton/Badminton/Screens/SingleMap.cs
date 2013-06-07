using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Badminton.Screens
{
	class SingleMap : GameScreen
	{
		// TODO: Level loader from xml/text file or something
		// Pretty much, a map will be defined in some kind of external file
		// Read in the file with either IO or an XML parser and add objects to a list in constructor
		// Iterate through that list in ever Update/Draw call
		// But for now, this class can be used for hardcoding tests

		World world;
		List<Wall> walls;

		StickFigure testFigure;

		public SingleMap()
		{
			world = new World(new Vector2(0, 9.8f)); // That'd be cool to have gravity as a map property, so you could play 0G levels

			testFigure = new StickFigure(world, new Vector2(480 * MainGame.PIXEL_TO_METER, 480 * MainGame.PIXEL_TO_METER), Category.Cat1, Color.Red);

			walls = new List<Wall>();
			walls.Add(new Wall(world, 480 * MainGame.PIXEL_TO_METER, 700 * MainGame.PIXEL_TO_METER, 960 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 16 * MainGame.PIXEL_TO_METER, 540 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 1080 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 960 * MainGame.PIXEL_TO_METER, 1040 * MainGame.PIXEL_TO_METER, 1920 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1500 * MainGame.PIXEL_TO_METER, 960 * MainGame.PIXEL_TO_METER, 870 * MainGame.PIXEL_TO_METER, 120 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1450 * MainGame.PIXEL_TO_METER, 865 * MainGame.PIXEL_TO_METER, 248 * MainGame.PIXEL_TO_METER, 98 * MainGame.PIXEL_TO_METER, -(float)Math.PI / 6));
			walls.Add(new Wall(world, 1735 * MainGame.PIXEL_TO_METER, 840 * MainGame.PIXEL_TO_METER, 402 * MainGame.PIXEL_TO_METER, 176 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1904 * MainGame.PIXEL_TO_METER, 540 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 1080 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1859 * MainGame.PIXEL_TO_METER, 717 * MainGame.PIXEL_TO_METER, 109 * MainGame.PIXEL_TO_METER, 123 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1600 * MainGame.PIXEL_TO_METER, 570 * MainGame.PIXEL_TO_METER, 122 * MainGame.PIXEL_TO_METER, 104 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 1320 * MainGame.PIXEL_TO_METER, 487 * MainGame.PIXEL_TO_METER, 113 * MainGame.PIXEL_TO_METER, 107 * MainGame.PIXEL_TO_METER, 0.0f));
			walls.Add(new Wall(world, 180 * MainGame.PIXEL_TO_METER, 300 * MainGame.PIXEL_TO_METER, 41 * MainGame.PIXEL_TO_METER, 370 * MainGame.PIXEL_TO_METER, 0.0f));
		}

		public GameScreen Update(GameTime gameTime)
		{
			if (Mouse.GetState().RightButton == ButtonState.Pressed)
				testFigure.Aim(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER);
			if (Mouse.GetState().LeftButton == ButtonState.Pressed)
				testFigure.ApplyForce(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER - testFigure.Position);

			bool stand = true;

			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				testFigure.Jump();
				stand = false;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				testFigure.WalkRight();
				stand = false;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				testFigure.WalkLeft();
				stand = false;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
			{
				if (!Keyboard.GetState().IsKeyDown(Keys.W))
				{
					testFigure.Crouching = true;
					stand = false;
				}
			}
			else
				testFigure.Crouching = false;

			if (stand)
				testFigure.Stand();

			testFigure.Update();

			// These two lines stay here, even after we delete testing stuff
			world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
			return this;
		}

		public GameScreen Exit()
		{
			return new MainMenu(); // Change this to show confirmation dialog later
		}

		public void Draw(SpriteBatch sb)
		{
			testFigure.Draw(sb);

			foreach (Wall w in walls)
				w.Draw(sb);
		}
	}
}
