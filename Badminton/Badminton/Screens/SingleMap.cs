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

		/*/ -- Upright torso test --
		Body capsule, gyro;
		AngleJoint joint;
		// ------------------------*/

		StickFigure testFigure;

		public SingleMap()
		{
			world = new World(new Vector2(0, 9.8f)); // That'd be cool to have gravity as a map property, so you could play 0G levels

			testFigure = new StickFigure(world, new Vector2(480 * MainGame.PIXEL_TO_METER, 480 * MainGame.PIXEL_TO_METER), Category.Cat1);

			floor = BodyFactory.CreateRectangle(world, 960 * MainGame.PIXEL_TO_METER, 32 * MainGame.PIXEL_TO_METER, 1f);
			floor.Position = new Vector2(480 * MainGame.PIXEL_TO_METER, 700 * MainGame.PIXEL_TO_METER);
			floor.BodyType = BodyType.Static;
			floor.CollisionCategories = Category.All & ~Category.Cat1;

			/*/ ------ Upright torso test ---------------
			capsule = BodyFactory.CreateCapsule(world, 96 * MainGame.PIXEL_TO_METER, 16 * MainGame.PIXEL_TO_METER, 1f);
			capsule.Position = new Vector2(480 * MainGame.PIXEL_TO_METER, 480 * MainGame.PIXEL_TO_METER);
			capsule.BodyType = BodyType.Dynamic;
			gyro = BodyFactory.CreateBody(world, capsule.Position);
			gyro.CollidesWith = Category.None;
			gyro.BodyType = BodyType.Dynamic;
			gyro.Mass = 0.00001f;

			joint = JointFactory.CreateAngleJoint(world, capsule, gyro);
			joint.MaxImpulse = 0.0f;
			joint.TargetAngle = 0.0f;
			joint.CollideConnected = false;

			capsule.ApplyTorque(10f);
			// -----------------------------------------*/
		}

		public GameScreen Update(GameTime gameTime)
		{
			/*/ ------ Upright torso test ---------------
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				joint.MaxImpulse = 0.01f;
			}
			else
			{
				joint.MaxImpulse = 0.0f;
				capsule.ApplyTorque(0.1f);
			}
			// -----------------------------------------*/

			if (Mouse.GetState().RightButton == ButtonState.Pressed)
				testFigure.Aim(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER);

			if (Keyboard.GetState().IsKeyDown(Keys.Space))
				testFigure.Jump();
			else if (Keyboard.GetState().IsKeyDown(Keys.D))
				testFigure.WalkRight();
			else if (Keyboard.GetState().IsKeyDown(Keys.A))
				testFigure.WalkLeft();
			else
				testFigure.Stand();

			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && !Keyboard.GetState().IsKeyDown(Keys.Space))
				testFigure.Squat();

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

			sb.Draw(MainGame.tex_box, new Rectangle((int)(floor.Position.X * MainGame.METER_TO_PIXEL), (int)(floor.Position.Y * MainGame.METER_TO_PIXEL), 960, 32), null,
							 Color.White, 0.0f, new Vector2(16, 16), SpriteEffects.None, 0.0f);
			/*/ ------ Upright torso test ---------------
			sb.Draw(MainGame.tex_box, new Rectangle((int)(capsule.Position.X * MainGame.METER_TO_PIXEL), (int)(capsule.Position.Y * MainGame.METER_TO_PIXEL), 32, 96), null,
							 Color.White, capsule.Rotation, new Vector2(16, 16), SpriteEffects.None, 0.0f);
			// -----------------------------------------*/
		}
	}
}
