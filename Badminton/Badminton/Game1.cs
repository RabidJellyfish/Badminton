using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Badminton
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		World world;
		Texture2D box;

		float meterToPixel = 100f;
		float pixelToMeter = 1f / 100f; 

		Body floor, capsule, gyro;
		AngleJoint joint;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferredBackBufferWidth = 960;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			world = new World(new Vector2(0, 9.8f));

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			box = Content.Load<Texture2D>("box");

			floor = BodyFactory.CreateRectangle(world, 960 * pixelToMeter, 32 * pixelToMeter, 1f);
			floor.Position = new Vector2(480 * pixelToMeter, 700 * pixelToMeter);
			floor.BodyType = BodyType.Static;

			capsule = BodyFactory.CreateCapsule(world, 96 * pixelToMeter, 16 * pixelToMeter, 1f);
			capsule.Position = new Vector2(480 * pixelToMeter, 480 * pixelToMeter);
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


		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				joint.MaxImpulse = 0.01f;
			}
			else
			{
				joint.MaxImpulse = 0.0f;
				capsule.ApplyTorque(0.1f);
			}

			world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			spriteBatch.Draw(box, new Rectangle((int)(floor.Position.X * meterToPixel), (int)(floor.Position.Y * meterToPixel), 960, 32), null,
							 Color.White, 0.0f, new Vector2(16, 16), SpriteEffects.None, 0.0f);
			spriteBatch.Draw(box, new Rectangle((int)(capsule.Position.X * meterToPixel), (int)(capsule.Position.Y * meterToPixel), 32, 96), null,
							 Color.White, capsule.Rotation, new Vector2(16, 16), SpriteEffects.None, 0.0f);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
