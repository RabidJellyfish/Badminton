using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Badminton.Weapons
{
	class TestWeapon : Weapon
	{
		private List<Bullet> bullets;

		public TestWeapon(World world, Vector2 position)
			: base (world, position, WeaponType.Medium, 16 * MainGame.PIXEL_TO_METER, 48 * MainGame.PIXEL_TO_METER, 0.0001f)
		{
			bullets = new List<Bullet>();

			this.ammoCapacity = 100;
			this.ammo = 100;
			this.clipSize = 50;
			this.clipAmmo = 0;

			this.refireTime = 5;
			this.reloadTime = 120;
		}

		public override void Update()
		{
			base.Update();

			List<Bullet> toRemove = new List<Bullet>();
			foreach (Bullet b in bullets)
			{
				b.Update();
				if (b.Remove)
					toRemove.Add(b);
			}
			foreach (Bullet b in toRemove)
				bullets.Remove(b);
		}

		public override void Fire()
		{
			if (refireCount == refireTime && clipAmmo > 0 && !reloading)
			{
				Vector2 velocity = new Vector2((float)Math.Sin(gun.Rotation), -(float)Math.Cos(gun.Rotation));
				Vector2 firePos = gun.Position + velocity;
				velocity.Normalize();
				velocity *= 75f;
				Bullet b = new TestBullet(world, this.collisionCat, firePos, velocity);
				gun.ApplyForce(velocity * -1 * b.Mass);
				bullets.Add(b);
				if (bullets.Count > 50)
					bullets.RemoveAt(0);
			}
			base.Fire();
		}

		public override void Draw(SpriteBatch sb)
		{
			float r = gun.Rotation;
			while (r > Math.PI)
				r -= MathHelper.TwoPi;
			while (r < -Math.PI)
				r += MathHelper.TwoPi;
			sb.Draw(MainGame.tex_gun, gun.Position * MainGame.METER_TO_PIXEL * MainGame.RESOLUTION_SCALE, null, Color.White, gun.Rotation, new Vector2(16, 48), 0.6f * MainGame.RESOLUTION_SCALE, gun.Rotation > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
			foreach (Bullet b in bullets)
				b.Draw(sb);
		}

		public override void DrawHUD(SpriteBatch sb)
		{
			StringBuilder ammoString = new StringBuilder();
			ammoString.Append(ammo.ToString());
			ammoString.Append("/");
			ammoString.AppendLine(ammoCapacity.ToString());
			ammoString.Append("[");
			for (int i = 1; i <= clipSize; i++)
				if (i <= clipAmmo)
					ammoString.Append("-");
				else
					ammoString.Append(" ");
			ammoString.Append("] ");
			ammoString.Append(clipAmmo.ToString());
			ammoString.Append("/");
			ammoString.Append(clipSize.ToString());
			sb.DrawString(MainGame.fnt_basicFont, ammoString, Vector2.Zero, Color.White);
		}
	}
}
