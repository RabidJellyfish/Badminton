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
    abstract class Bullet
    {
        protected Body bullet;

		/// <summary>
		/// The center of the bullet
		/// </summary>
		public Vector2 Position { get { return bullet.Position; } }

		/// <summary>
		/// The bullet's linear velocity
		/// </summary>
		public Vector2 Velocity { get { return bullet.LinearVelocity; } }

		/// <summary>
		/// The amount of damage the bullet causes
		/// </summary>
		public float Damage { get; set; }

		/// <summary>
		/// Whether or not the bullet should be destroyed this step
		/// </summary>
		public bool Remove { get { return bullet.UserData == null; } }

        protected World world;
		private Category collisionCat;

		/// <summary>
		/// Creates a bullet
		/// </summary>
		/// <param name="world">The world to add the body to</param>
		/// <param name="collisionCat">The collision category of the bullet</param>
		/// <param name="position">Where the bullet is created</param>
		/// <param name="velocity">The bullet's initial velocity</param>
        public Bullet(World world, Category collisionCat, Vector2 position, Vector2 velocity)
        {
            this.world = world;
			this.collisionCat = collisionCat;

			bullet = BodyFactory.CreateRectangle(world, 11.0f * MainGame.PIXEL_TO_METER, 3.2f * MainGame.PIXEL_TO_METER, 1f);
			bullet.BodyType = BodyType.Dynamic;
			bullet.Position = position;
			bullet.CollisionCategories = this.collisionCat;
			bullet.CollidesWith = Category.All & ~collisionCat;
			bullet.IsBullet = true;
			bullet.UserData = this;
			bullet.LinearVelocity = velocity;
			bullet.Rotation = (float)Math.Atan2(bullet.LinearVelocity.Y, bullet.LinearVelocity.X) + MathHelper.PiOver2;
			bullet.OnSeparation += new OnSeparationEventHandler(OnSeparation);
		}

		private void OnSeparation(Fixture f1, Fixture f2)
		{
			bullet.UserData = null;
		}

		/// <summary>
		/// Called once every step. Updates the bullet.
		/// </summary>
		public virtual void Update()
		{
			if (bullet.UserData == null && world.BodyList.Contains(bullet))
				world.RemoveBody(bullet);
		}

		/// <summary>
		/// Draws the bullet
		/// </summary>
		/// <param name="sb">The SpriteBatch used to draw the bullet</param>
		public abstract void Draw(SpriteBatch sb);
    }
}
