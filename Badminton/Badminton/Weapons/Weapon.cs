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
	abstract class Weapon
	{
		protected World world;
		protected Body gun;

		/// <summary>
		/// The amount of ammo the weapon is currently carrying
		/// </summary>
		public int Ammo 
		{
			get 
			{ 
				return ammo; 
			}
			set
			{
				ammo = value;
				if (ammo > ammoCapacity)
					ammo = ammoCapacity;
			}
		}
		protected int ammo;
		
		/// <summary>
		/// The total carrying capacity of the weapon
		/// </summary>
		public int AmmoCapacity { get { return ammoCapacity; } }
		protected int ammoCapacity;
		
		/// <summary>
		/// The amount of ammo in the current clip
		/// </summary>
		public int ClipAmmo
		{
			get
			{
				return clipAmmo;
			}
			set
			{
				clipAmmo = value;
				if (clipAmmo > clipSize)
					clipAmmo = clipSize;
			}
		}
		protected int clipAmmo;

		/// <summary>
		/// The ammount of ammo a clip can carry
		/// </summary>
		public int ClipSize 
		{ 
			get 
			{
				if (clipSize > ammoCapacity)
					clipSize = ammoCapacity;
				return clipSize; 
			} 
		}
		protected int clipSize;

		/// <summary>
		/// Returns whether or not the gun is already in possesion of another player
		/// </summary>
		public bool BeingHeld { get; set; }

		/// <summary>
		/// Gets and sets the position of the gun
		/// </summary>
		public Vector2 Position
		{
			get { return this.gun.Position; }
			set { this.gun.Position = value; }
		}

		/// <summary>
		/// Gets the body of the gun
		/// </summary>
		public Body Body { get { return gun; } }

		protected int refireTime; // In frames
		protected int refireCount;

		protected int reloadTime; // In frames
		protected int reloadCount;
		protected bool reloading;
		protected Category collisionCat;
		WeaponType type;

		/// <summary>
		/// Specifies a type of weapon
		/// </summary>
		public enum WeaponType
		{
			Melee,
			Light,
			Medium,
			Heavy,
			Explosive
		}

		/// <summary>
		/// Creates a new weapon
		/// </summary>
		/// <param name="world">The world to add the weapon to</param>
		/// <param name="position">The position to create the weapon at</param>
		/// <param name="type">The type of weapon</param>
		/// <param name="width">The width of the weapon</param>
		/// <param name="height">The height of the weapon</param>
		/// <param name="mass">The mass of the weapon</param>
		public Weapon(World world, Vector2 position, WeaponType type, float width, float height, float density)
		{
			this.world = world;
			this.ammoCapacity = 0;
			this.ammo = 0;
			this.clipSize = 0;
			this.clipAmmo = 0;

			this.refireTime = 0;
			refireCount = 0;

			this.reloadTime = 0;
			reloadCount = 0;
			reloading = false;

			this.type = type;

			gun = BodyFactory.CreateRectangle(world, width, height, density);
			gun.BodyType = BodyType.Dynamic;
			gun.Position = position;
			gun.Restitution = 0.3f;
			gun.UserData = this;
			gun.CollisionCategories = Category.All;
			gun.CollidesWith = Category.All;
			gun.OnCollision += new OnCollisionEventHandler(ResetCollisionCategory);
		}

		private bool ResetCollisionCategory(Fixture fixA, Fixture fixB, Contact contact)
		{
			gun.CollisionCategories = Category.All & ~Category.Cat1;
			gun.CollidesWith = Category.All & ~Category.Cat1;
			return contact.IsTouching();
		}

		/// <summary>
		/// Updates the weapon. Called once every frame.
		/// </summary>
		public virtual void Update()
		{
			if (!BeingHeld)
			{
				this.gun.CollisionCategories = Category.All;
				this.gun.CollidesWith = Category.All;
			}

			if (clipSize > 0)
			{
				if (reloading)
				{
					reloadCount++;
					if (reloadCount % (int)(reloadTime / clipSize) == 0)
					{
						clipAmmo++;
						ammo--;
					}
					if (clipAmmo == clipSize || ammo == 0)
						reloading = false;
				}
			}

			if (refireCount < refireTime)
				refireCount++;
		}

		/// <summary>
		/// Fires the weapon if it has ammo. Should be overridden to actually shoot things
		/// </summary>
		public virtual void Fire()
		{
			if (!reloading && refireCount == refireTime)
			{
				if (clipAmmo > 0)
				{
					refireCount = 0;
					clipAmmo--;
				}
				else if (ammo > 0)
					Reload();
			}
		}

		/// <summary>
		/// Reloads the weapon
		/// </summary>
		public void Reload()
		{
			if (clipAmmo < clipSize && ammo > 0 && !reloading)
			{
				reloading = true;
				reloadCount = 0;
			}
		}

		/// <summary>
		/// Sets the weapon up to be held
		/// </summary>
		/// <param name="collisionCat">The collision category of the player to be held by</param>
		public void PickUp(Category collisionCat)
		{
			this.collisionCat = collisionCat;
			this.gun.CollisionCategories = collisionCat;
			this.gun.CollidesWith = Category.None;
			this.BeingHeld = true;
			this.Reload();
		}

		/// <summary>
		/// Draws the weapon
		/// </summary>
		/// <param name="sb">The SpriteBatch to draw the weapon with</param>
		public abstract void Draw(SpriteBatch sb);

		/// <summary>
		/// Draws information about the gun (such as ammo) to the screen
		/// </summary>
		/// <param name="sb"></param>
		public abstract void DrawHUD(SpriteBatch sb);
	}
}
