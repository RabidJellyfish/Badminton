using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Badminton.Stick_Figures
{
	class LocalPlayer : StickFigure
	{
		public LocalPlayer(World world, Vector2 position, Category collisionCat, Color color)
			: base(world, position, collisionCat, color)
		{
		}

		public override void Update()
		{
			if (Keyboard.GetState().IsKeyDown(Keys.S) && touchingWeapons.Count > 0)
				PickUpWeapon(touchingWeapons[0]);

			if (Mouse.GetState().RightButton == ButtonState.Pressed)
				Aim(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER);

			if (Mouse.GetState().LeftButton == ButtonState.Pressed)
				FireWeapon();

			if (Keyboard.GetState().IsKeyDown(Keys.R))
				ReloadWeapon();

			if (Keyboard.GetState().IsKeyDown(Keys.E))
				Melee(); // Change to switch hands eventually

			if (Keyboard.GetState().IsKeyDown(Keys.Q))
				ThrowWeapon(new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * MainGame.PIXEL_TO_METER);

			bool stand = true;

			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				Jump();
				stand = false;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				WalkRight();
				stand = false;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				WalkLeft();
				stand = false;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
			{
				if (!Keyboard.GetState().IsKeyDown(Keys.W))
				{
					Crouching = true;
					stand = false;
				}
			}
			else
				Crouching = false;

			if (stand)
				Stand();

			base.Update();
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

			if (this.weapon != null)
				this.weapon.DrawHUD(sb);
		}
	}
}
