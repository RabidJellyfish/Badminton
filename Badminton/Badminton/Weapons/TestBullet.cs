using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Badminton.Weapons
{
	class TestBullet : Bullet
	{
		public TestBullet(World world, Category collisionCat, Vector2 position, Vector2 velocity)
			: base(world, collisionCat, position, velocity)
		{
			bullet.Restitution = 0.5f;
			bullet.FixedRotation = true;
			bullet.Mass = 1f;
			this.Damage = 100f;
		}

		public override void Update()
		{
			base.Update();

			if (!world.BodyList.Contains(bullet))
				return;

			bullet.ApplyForce(new Vector2(0, -0.05f * bullet.Mass));
			bullet.Rotation = (float)Math.Atan2(bullet.LinearVelocity.Y, bullet.LinearVelocity.X) + MathHelper.PiOver2;
		}

		public override void Draw(SpriteBatch sb)
		{
			// Change origin to center of texture
			if (world.BodyList.Contains(bullet) && bullet.UserData != null)
				sb.Draw(MainGame.tex_bullet, bullet.Position * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE, null, Color.White, bullet.Rotation, new Vector2(16f, 56f), 0.1f * MainGame.RESOLUTION_SCALE, SpriteEffects.None, 0.0f);
		}
	}
}
