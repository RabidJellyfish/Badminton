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

		protected int refireTime; // In frames
		private int refireCount;

		protected int reloadTime; // In frames
		private int reloadCount;
		protected bool reloading;
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
		public Weapon(World world, Vector2 position, WeaponType type, float width, float height, float mass)
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

			gun = BodyFactory.CreateRectangle(world, width, height, 1f);
			gun.BodyType = BodyType.Dynamic;
			gun.Position = position;
			gun.UserData = this;
		}

		/// <summary>
		/// Updates the weapon. Called once every frame.
		/// </summary>
		public virtual void Update()
		{
			if (clipSize > 0)
			{
				if (reloading)
				{
					reloadCount++;
					if ((int)(reloadTime / clipSize) % reloadCount == 0)
						clipAmmo++;
					if (clipAmmo == clipSize || ammo - clipAmmo == 0)
					{
						reloading = false;
						ammo -= clipAmmo;
					}
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
			if (ammo > 0)
			{
				if (clipAmmo > 0)
				{
					refireCount = 0;
					clipAmmo--;
				}
				else
					Reload();
			}
		}

		/// <summary>
		/// Reloads the weapon
		/// </summary>
		public void Reload()
		{
			if (clipAmmo < clipSize && ammo > 0)
			{
				reloading = true;
				reloadCount = 0;
			}
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
