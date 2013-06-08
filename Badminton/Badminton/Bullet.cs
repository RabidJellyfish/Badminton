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

namespace Badminton
{
    class Bullet
    {
        protected Body bullet;

        //The position of the bullet
		public Vector2 Position { get { return bullet.Position; } }
		public Vector2 Velocity { get { return bullet.LinearVelocity; } }

        private World world;
		private Category collisionCat;

		//Makes a bullet
        public Bullet(World world, Category collisionCat, Vector2 position, Vector2 velocity, float mass)
        {
            this.world = world;
			this.collisionCat = collisionCat;
			MakeBullet(position, velocity, mass);
        }

        //Actually, this makes the bullet
        private void MakeBullet(Vector2 position, Vector2 velocity, float mass)
        {
            bullet = BodyFactory.CreateRectangle(world, 11.0f * MainGame.PIXEL_TO_METER, 3.2f * MainGame.PIXEL_TO_METER, 10.0f);
            bullet.BodyType = BodyType.Dynamic;
            bullet.Position = position;
            bullet.Restitution = 0.5f;
			bullet.CollisionCategories = this.collisionCat;
			bullet.CollidesWith = Category.All & ~collisionCat;
			bullet.FixedRotation = true;
			bullet.IsBullet = true;
			bullet.UserData = new Tuple<string, float>("bullet", 0.0f);
            bullet.LinearVelocity = velocity;
			
			bullet.Mass = mass;
		}

		public void Update()
		{
			if (world.BodyList.Contains(bullet) && bullet.UserData != null)
			{
				//Counteracts gravity
				bullet.ApplyForce(new Vector2(0, -0.05f * bullet.Mass));
				Vector2 direction = bullet.GetLinearVelocityFromLocalPoint(bullet.Position);
				bullet.Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
//				bullet.UserData = bullet.LinearVelocity;
			}
		}

        public void Draw(SpriteBatch sb)
        {
			// Change origin to center of texture
			if (world.BodyList.Contains(bullet) && bullet.UserData != null)
				sb.Draw(MainGame.tex_bullet, bullet.Position * MainGame.METER_TO_PIXEL, null, Color.White, bullet.Rotation, new Vector2(0.0f, 0.0f), 0.1f, SpriteEffects.None, 0.0f);   
        }
    }
}
