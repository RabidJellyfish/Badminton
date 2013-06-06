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
		Body floor;
		Body wall;

		StickFigure testFigure;

		public SingleMap()
		{
			world = new World(new Vector2(0, 9.8f)); // That'd be cool to have gravity as a map property, so you could play 0G levels

			testFigure = new StickFigure(world, new Vector2(480 * MainGame.PIXEL_TO_METER, 480 * MainGame.PIXEL_TO_METER), Category.Cat1);

			floor = BodyFactory.CreateRectangle(world, 960 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 1f);
			floor.Position = new Vector2(480 * MainGame.PIXEL_TO_METER, 700 * MainGame.PIXEL_TO_METER);
			floor.BodyType = BodyType.Static;
			floor.CollisionCategories = Category.All & ~Category.Cat1;

			wall = BodyFactory.CreateRectangle(world, 32 * MainGame.PIXEL_TO_METER, 720 * MainGame.PIXEL_TO_METER, 1f);
			wall.Position = new Vector2(16 * MainGame.PIXEL_TO_METER, 360 * MainGame.PIXEL_TO_METER);
			wall.BodyType = BodyType.Static;
			wall.CollisionCategories = Category.All & ~Category.Cat1;
		}

		public GameScreen Update(GameTime gameTime)
		{
			if (Mouse.GetState().RightButton == ButtonState.Pressed)
				testFigure.Aim(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER);

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

			sb.Draw(MainGame.tex_box, new Rectangle((int)(floor.Position.X * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE.X),
													(int)(floor.Position.Y * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE.Y),
													(int)(960 * MainGame.RESOLUTION_SCALE.X), (int)(32 * MainGame.RESOLUTION_SCALE.Y)), null,
							 Color.White, floor.Rotation, new Vector2(16, 16), SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_box, new Rectangle((int)(wall.Position.X * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE.X),
													(int)(wall.Position.Y * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE.Y),
													(int)(32 * MainGame.RESOLUTION_SCALE.X), (int)(720 * MainGame.RESOLUTION_SCALE.Y)), null,
							 Color.White, wall.Rotation, new Vector2(16, 16), SpriteEffects.None, 0.0f);
		}
	}
}
