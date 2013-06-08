using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Badminton.Stick_Figures;

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
		List<Bullet> bulletList;

		LocalPlayer testFigure;
		StickFigure dummyFigure;

		public SingleMap()
		{
			world = new World(new Vector2(0, 9.8f)); // That'd be cool to have gravity as a map property, so you could play 0G levels
			world.ContactManager.PreSolve += new PreSolveDelegate(PreSolve);

			testFigure = new LocalPlayer(world, new Vector2(480 * MainGame.PIXEL_TO_METER, 480 * MainGame.PIXEL_TO_METER), Category.Cat1, Color.Red);
			dummyFigure = new StickFigure(world, new Vector2(150 * MainGame.PIXEL_TO_METER, 900 * MainGame.PIXEL_TO_METER), Category.Cat2, Color.Green);

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

			bulletList = new List<Bullet>();
			i = 0;
		}

		// Solves for bullet damage
		public void PreSolve(Contact contact, ref Manifold manifold)
		{
			// Check which fixture is a bullet
			Body bullet;
			if (MainGame.BodyIsBullet(contact.FixtureA.Body))
				bullet = contact.FixtureA.Body;
			else if (MainGame.BodyIsBullet(contact.FixtureB.Body))
				bullet = contact.FixtureB.Body;
			else
				return;

			Body other = bullet == contact.FixtureA.Body ? contact.FixtureB.Body : contact.FixtureA.Body;

			Vector2 normal = contact.Manifold.LocalNormal;
			float damage = Math.Abs(Vector2.Dot(bullet.LinearVelocity * bullet.Mass, normal));
			bullet.UserData = new Tuple<string, float>(((Tuple<string, float>)bullet.UserData).Item1, damage);
		}

		private int i = 0;
		public GameScreen Update(GameTime gameTime)
		{
			//////////////////////////////////////////////
			// Put in Weapon class whenever that exists //
			//////////////////////////////////////////////
			if ((Mouse.GetState().LeftButton == ButtonState.Pressed))
			{
				//i is used for fire rate. The bullet will fire once every 60/5 seconds in this case
				if (i % 5 == 0)
				{
					Vector2 velocity = new Vector2(Mouse.GetState().X * MainGame.PIXEL_TO_METER, Mouse.GetState().Y * MainGame.PIXEL_TO_METER) - testFigure.RightHandPosition;
					velocity.Normalize();
					velocity *= 75f;
					Bullet g = new Bullet(world, Category.Cat1, testFigure.RightHandPosition, velocity, 1f);
					bulletList.Add(g);
					if (bulletList.Count > 50)
						bulletList.RemoveAt(0);
				}
				i++;
			}
			else
				i = 0;
			//////////////////////////////////////////////

			testFigure.Update();
			dummyFigure.Update();

			foreach (Bullet b in bulletList)
				b.Update();

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
			dummyFigure.Draw(sb);

			foreach (Wall w in walls)
				w.Draw(sb);
			foreach (Bullet b in bulletList)
				b.Draw(sb);

			if (bulletList.Count > 0)
				sb.DrawString(MainGame.fnt_basicFont, bulletList[bulletList.Count - 1].Velocity.ToString(), Vector2.Zero, Color.White);
		}
	}
}
