using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Badminton
{
	class StickFigure
	{
		private Body torso, head, leftUpperArm, rightUpperArm, leftLowerArm, rightLowerArm, leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg;
		private Body gyro;
		private AngleJoint neck, leftShoulder, rightShoulder, leftElbow, rightElbow, leftHip, rightHip, leftKnee, rightKnee;
		private AngleJoint upright;

		public Vector2 Position { get { return torso.Position; } }
		public Vector2 LeftHandPosition
		{
			get
			{
				return leftLowerArm.Position + new Vector2((float)-Math.Cos(leftLowerArm.Rotation), (float)-Math.Sin(leftLowerArm.Rotation)) * 7.5f * MainGame.PIXEL_TO_METER;
			}
		}
		public Vector2 RightHandPosition
		{
			get
			{
				return rightLowerArm.Position + new Vector2((float)-Math.Cos(rightLowerArm.Rotation), (float)-Math.Sin(rightLowerArm.Rotation)) * 7.5f * MainGame.PIXEL_TO_METER;
			}
		}
		
		public bool Aiming { get; set; }

		private float maxImpulse;
		private Vector2 aimVector;
		private int walkStage = 0;
		private bool crouching;
		private bool onGround;

		public StickFigure(World world, Vector2 position, Category collisionCat)
		{
			maxImpulse = 10f;
			crouching = false;
			Aiming = false;
			GenerateBody(world, position, collisionCat);
			ConnectBody(world);

			onGround = false;
			leftLowerLeg.OnCollision += new OnCollisionEventHandler(OnCollision);
			rightLowerLeg.OnCollision += new OnCollisionEventHandler(OnCollision);
			leftLowerLeg.OnSeparation += new OnSeparationEventHandler(OnSeparation);
			rightLowerLeg.OnSeparation += new OnSeparationEventHandler(OnSeparation);

			Stand();
		}

		#region Creation

		/// <summary>
		/// Generates the stick figure's limbs, torso, and head
		/// </summary>
		/// <param name="world">The physics world to add the bodies to</param>
		/// <param name="position">The position to place the center of the torso</param>
		/// <param name="collisionCat">The collision category of the stick figure</param>
		private void GenerateBody(World world, Vector2 position, Category collisionCat)
		{
			torso = BodyFactory.CreateCapsule(world, 40 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 10.0f);
			torso.Position = position;
			torso.BodyType = BodyType.Dynamic;
			torso.CollisionCategories = collisionCat;
			torso.CollidesWith = Category.All & ~collisionCat;
			gyro = BodyFactory.CreateBody(world, torso.Position);
			gyro.CollidesWith = Category.None;
			gyro.BodyType = BodyType.Dynamic;
			gyro.Mass = 0.00001f;
			gyro.FixedRotation = true;

			head = BodyFactory.CreateCircle(world, 12.5f * MainGame.PIXEL_TO_METER, 1.0f);
			head.Position = torso.Position - new Vector2(0, 29f) * MainGame.PIXEL_TO_METER;
			head.BodyType = BodyType.Dynamic;
			head.CollisionCategories = collisionCat;
			head.CollidesWith = Category.All & ~collisionCat;

			leftUpperArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 0.1f);
			leftUpperArm.Rotation = -MathHelper.PiOver2;
			leftUpperArm.Position = torso.Position + new Vector2(-7.5f, -15) * MainGame.PIXEL_TO_METER;
			leftUpperArm.BodyType = BodyType.Dynamic;
			leftUpperArm.CollisionCategories = collisionCat;
			leftUpperArm.CollidesWith = Category.All & ~collisionCat;

			rightUpperArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 0.1f);
			rightUpperArm.Rotation = MathHelper.PiOver2;
			rightUpperArm.Position = torso.Position + new Vector2(7.5f, -15) * MainGame.PIXEL_TO_METER;
			rightUpperArm.BodyType = BodyType.Dynamic;
			rightUpperArm.CollisionCategories = collisionCat;
			rightUpperArm.CollidesWith = Category.All & ~collisionCat;

			leftLowerArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 0.1f);
			leftLowerArm.Rotation = -MathHelper.PiOver2;
			leftLowerArm.Position = torso.Position + new Vector2(-22.5f, -15) * MainGame.PIXEL_TO_METER;
			leftLowerArm.BodyType = BodyType.Dynamic;
			leftLowerArm.CollisionCategories = collisionCat;
			leftLowerArm.CollidesWith = Category.All & ~collisionCat;

			rightLowerArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 0.1f);
			rightLowerArm.Rotation = MathHelper.PiOver2;
			rightLowerArm.Position = torso.Position + new Vector2(22.5f, -15) * MainGame.PIXEL_TO_METER;
			rightLowerArm.BodyType = BodyType.Dynamic;
			rightLowerArm.CollisionCategories = collisionCat;
			rightLowerArm.CollidesWith = Category.All & ~collisionCat;

			leftUpperLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 5f);
			leftUpperLeg.Rotation = -3 * MathHelper.PiOver4;
			leftUpperLeg.Position = torso.Position + new Vector2(-25f / (float)Math.Sqrt(8) + 4, 10 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			leftUpperLeg.BodyType = BodyType.Dynamic;
			leftUpperLeg.CollisionCategories = collisionCat;
			leftUpperLeg.CollidesWith = Category.All & ~collisionCat;
			leftUpperLeg.Restitution = 0.15f;

			rightUpperLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 5f);
			rightUpperLeg.Rotation = 3 * MathHelper.PiOver4;
			rightUpperLeg.Position = torso.Position + new Vector2(25f / (float)Math.Sqrt(8) - 4, 10 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			rightUpperLeg.BodyType = BodyType.Dynamic;
			rightUpperLeg.CollisionCategories = collisionCat;
			rightUpperLeg.CollidesWith = Category.All & ~collisionCat;
			rightUpperLeg.Restitution = 0.15f;

			leftLowerLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 10.0f);
			leftLowerLeg.Position = torso.Position + new Vector2(-50f / (float)Math.Sqrt(8) + 6, 25 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			leftLowerLeg.BodyType = BodyType.Dynamic;
			leftLowerLeg.CollisionCategories = collisionCat;
			leftLowerLeg.CollidesWith = Category.All & ~collisionCat;
			leftLowerLeg.Restitution = 0.15f;
			leftLowerLeg.Friction = 3.0f;

			rightLowerLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 10.0f);
			rightLowerLeg.Position = torso.Position + new Vector2(50f / (float)Math.Sqrt(8) - 6, 25 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			rightLowerLeg.BodyType = BodyType.Dynamic;
			rightLowerLeg.CollisionCategories = collisionCat;
			rightLowerLeg.CollidesWith = Category.All & ~collisionCat;
			rightLowerLeg.Restitution = 0.15f;
			rightLowerLeg.Friction = 3.0f;
		}

		/// <summary>
		/// Connects the figure's body parts
		/// </summary>
		/// <param name="world">The physics world to add the joints to</param>
		private void ConnectBody(World world)
		{
			upright = JointFactory.CreateAngleJoint(world, torso, gyro);
			upright.MaxImpulse = maxImpulse;
			upright.TargetAngle = 0.0f;
			upright.CollideConnected = false;

			RevoluteJoint r_neck = JointFactory.CreateRevoluteJoint(world, head, torso, -Vector2.UnitY * 20 * MainGame.PIXEL_TO_METER);
			neck = JointFactory.CreateAngleJoint(world, head, torso);
			neck.CollideConnected = false;
			neck.MaxImpulse = maxImpulse;

			RevoluteJoint r_leftShoulder = JointFactory.CreateRevoluteJoint(world, leftUpperArm, torso, -Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			leftShoulder = JointFactory.CreateAngleJoint(world, leftUpperArm, torso);
			leftShoulder.CollideConnected = false;
			leftShoulder.MaxImpulse = maxImpulse;

			RevoluteJoint r_rightShoulder = JointFactory.CreateRevoluteJoint(world, rightUpperArm, torso, -Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			rightShoulder = JointFactory.CreateAngleJoint(world, rightUpperArm, torso);
			rightShoulder.CollideConnected = false;
			rightShoulder.MaxImpulse = maxImpulse;

			RevoluteJoint r_leftElbow = JointFactory.CreateRevoluteJoint(world, leftLowerArm, leftUpperArm, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			leftElbow = JointFactory.CreateAngleJoint(world, leftLowerArm, leftUpperArm);
			leftElbow.CollideConnected = false;
			leftElbow.MaxImpulse = maxImpulse;

			RevoluteJoint r_rightElbow = JointFactory.CreateRevoluteJoint(world, rightLowerArm, rightUpperArm, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			rightElbow = JointFactory.CreateAngleJoint(world, rightLowerArm, rightUpperArm);
			rightElbow.CollideConnected = false;
			rightElbow.MaxImpulse = maxImpulse;

			RevoluteJoint r_leftHip = JointFactory.CreateRevoluteJoint(world, leftUpperLeg, torso, Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			leftHip = JointFactory.CreateAngleJoint(world, leftUpperLeg, torso);
			leftHip.CollideConnected = false;
			leftHip.MaxImpulse = maxImpulse;

			RevoluteJoint r_rightHip = JointFactory.CreateRevoluteJoint(world, rightUpperLeg, torso, Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			rightHip = JointFactory.CreateAngleJoint(world, rightUpperLeg, torso);
			rightHip.CollideConnected = false;
			rightHip.MaxImpulse = maxImpulse;

			RevoluteJoint r_leftKnee = JointFactory.CreateRevoluteJoint(world, leftLowerLeg, leftUpperLeg, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			leftKnee = JointFactory.CreateAngleJoint(world, leftUpperLeg, leftLowerLeg);
			leftKnee.CollideConnected = false;
			leftKnee.MaxImpulse = maxImpulse;

			RevoluteJoint r_rightKnee = JointFactory.CreateRevoluteJoint(world, rightLowerLeg, rightUpperLeg, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			rightKnee = JointFactory.CreateAngleJoint(world, rightUpperLeg, rightLowerLeg);
			rightKnee.CollideConnected = false;
			rightKnee.MaxImpulse = maxImpulse;
		}

		/// <summary>
		/// Event handler for collisions between two fixtures
		/// </summary>
		/// <param name="fixtureA">Object 1</param>
		/// <param name="fixtureB">Object 2</param>
		/// <param name="contact">The collision</param>
		/// <returns></returns>
		bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			onGround = true;
			return contact.IsTouching();
		}

		/// <summary>
		/// Event handler for separation of a collision
		/// </summary>
		/// <param name="fixtureA">Object 1</param>
		/// <param name="fixtureB">Object 2</param>
		void OnSeparation(Fixture fixtureA, Fixture fixtureB)
		{
			onGround = false;
		}

		#endregion

		#region Stances

		/// <summary>
		/// Sends the stick figure to its default pose
		/// </summary>
		public void Stand()
		{
			crouching = false;
			upright.TargetAngle = 0.0f;
			walkStage = 0;
			leftHip.TargetAngle = 3 * MathHelper.PiOver4;
			leftKnee.TargetAngle = -5 * MathHelper.PiOver4;
			rightHip.TargetAngle = -3 * MathHelper.PiOver4;
			rightKnee.TargetAngle = -3 * MathHelper.PiOver4;
			leftLowerLeg.Friction = 0f;
			rightLowerLeg.Friction = 0f;
			AngleJoint[] checkThese = new AngleJoint[] { leftHip, leftKnee, rightHip, rightKnee };
			if (JointsAreInPosition(checkThese))
			{
				leftLowerLeg.Friction = 100f;
				rightLowerLeg.Friction = 100f;
			}
		}

		/// <summary>
		/// Causes the stick figure to point its arms in the direction of "position"
		/// </summary>
		/// <param name="position">The point at which the figure will direct its arms</param>
		public void Aim(Vector2 position)
		{
			this.Aiming = true;
			this.aimVector = position - (torso.Position - 15f * Vector2.UnitY * MainGame.PIXEL_TO_METER);
		}

		/// <summary>
		/// Makes figure walk the the right (place in Update method)
		/// </summary>
		public void WalkRight()
		{
			upright.TargetAngle = -0.1f;
			if (torso.LinearVelocity.X < 4 && !crouching)
				torso.ApplyForce(new Vector2(30, 0));
			AngleJoint[] checkThese = new AngleJoint[] { leftHip, rightHip };
			if (walkStage == 0)
			{
				leftHip.TargetAngle = (float)Math.PI - torso.Rotation;
				leftKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightHip.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.MaxImpulse = maxImpulse * 3;
				leftLowerLeg.Friction = 0.0f;
				rightLowerLeg.Friction = 1000f;
				if (JointsAreInPosition(checkThese))
					walkStage = 1;
			}
			else if (walkStage == 1)
			{
				leftHip.TargetAngle = 3 * MathHelper.PiOver2 - torso.Rotation;
				leftKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightHip.TargetAngle = -5 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightKnee.MaxImpulse = maxImpulse;
				if (JointsAreInPosition(checkThese))
					walkStage = 2;
			}
			else if (walkStage == 2)
			{
				leftHip.TargetAngle = 5 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.MaxImpulse = maxImpulse * 3;
				rightHip.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightLowerLeg.Friction = 0.0f;
				leftLowerLeg.Friction = 1000f;
				if (JointsAreInPosition(checkThese))
					walkStage = 3;
			}
			else if (walkStage == 3)
			{
				leftHip.TargetAngle = 3 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				leftKnee.MaxImpulse = maxImpulse;
				rightHip.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 0;
			}
		}
	
		/// <summary>
		/// Makes figure walk to the left (place in Update method)
		/// </summary>
		public void WalkLeft()
		{
			upright.TargetAngle = 0.1f;
			if (torso.LinearVelocity.X > -4 && !crouching)
				torso.ApplyForce(new Vector2(-30, 0));
			AngleJoint[] checkThese = new AngleJoint[] { leftHip, rightHip };
			if (walkStage == 0)
			{
				rightHip.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightKnee.TargetAngle = -3 * MathHelper.PiOver2 - torso.Rotation;
				leftHip.TargetAngle = 3 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -5 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.MaxImpulse = maxImpulse * 3;
				leftLowerLeg.Friction = 1000.0f;
				rightLowerLeg.Friction = 0f;
				if (JointsAreInPosition(checkThese))
					walkStage = 1;
			}
			else if (walkStage == 1)
			{
				rightHip.TargetAngle = -3 * MathHelper.PiOver2 - torso.Rotation;
				rightKnee.TargetAngle = -3 * MathHelper.PiOver2 - torso.Rotation;
				leftHip.TargetAngle = 5 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				leftKnee.MaxImpulse = maxImpulse;
				if (JointsAreInPosition(checkThese))
					walkStage = 2;
			}
			else if (walkStage == 2)
			{
				rightHip.TargetAngle = -5 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -5 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.MaxImpulse = maxImpulse * 3;
				leftHip.TargetAngle = (float)Math.PI - torso.Rotation;
				leftKnee.TargetAngle = -3 * MathHelper.PiOver2 - torso.Rotation;
				leftLowerLeg.Friction = 0.0f;
				rightLowerLeg.Friction = 1000f;
				if (JointsAreInPosition(checkThese))
					walkStage = 3;
			}
			else if (walkStage == 3)
			{
				rightHip.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightKnee.MaxImpulse = maxImpulse;
				leftHip.TargetAngle = MathHelper.PiOver2 - torso.Rotation;
				leftKnee.TargetAngle = -3 * MathHelper.PiOver2 - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 0;
			}
		}

		/// <summary>
		/// Makes the figure jump
		/// </summary>
		public void Jump()
		{
			leftHip.MaxImpulse = 100.0f;
			rightHip.MaxImpulse = 100.0f;
			leftKnee.MaxImpulse = 100.0f;
			rightKnee.MaxImpulse = 100.0f;
			upright.TargetAngle = 0.0f;
			leftHip.TargetAngle = MathHelper.Pi;
			leftKnee.TargetAngle = -MathHelper.Pi;
			rightHip.TargetAngle = -MathHelper.Pi;
			rightKnee.TargetAngle = -MathHelper.Pi;
			leftLowerLeg.Friction = 100.0f;
			rightLowerLeg.Friction = 100.0f;

			if (onGround)
				torso.ApplyLinearImpulse(Vector2.UnitY * (crouching ? -12 : -1));
		}

		/// <summary>
		/// Makes the figure crouch
		/// </summary>
		public void Squat()
		{
			crouching = true;
			leftLowerLeg.Friction = 0.1f;
			rightLowerLeg.Friction = 0.1f;
			leftHip.TargetAngle = MathHelper.PiOver4;
			leftKnee.TargetAngle = -7 * MathHelper.PiOver4;
			rightHip.TargetAngle = -MathHelper.PiOver4;
			rightKnee.TargetAngle = -MathHelper.PiOver4;
		}

		/// <summary>
		/// Checks if all the joints in a list are close to their target angle
		/// </summary>
		/// <param name="joints">The array of joints to check</param>
		/// <returns>True if the joints are at their target angles, false if not</returns>
		private bool JointsAreInPosition(AngleJoint[] joints)
		{
			foreach (AngleJoint j in joints)
			{
				if (Math.Abs(j.BodyB.Rotation - j.BodyA.Rotation - j.TargetAngle) > 0.20)
					return false;
			}
			return true;
		}

		#endregion

		public void Update()
		{
			UpdateArms();
		}

		/// <summary>
		/// Orients arms in necessary position
		/// </summary>
		private void UpdateArms()
		{
			if (!Aiming)
			{
				// Rest arms at side
				float angle = 3 * MathHelper.PiOver4;
				while (angle - leftShoulder.TargetAngle + 0.01f > Math.PI)
					angle -= MathHelper.TwoPi;
				while (angle - leftShoulder.TargetAngle + 0.01f < -Math.PI)
					angle += MathHelper.TwoPi;
				leftShoulder.TargetAngle = angle;

				angle = -3 * MathHelper.PiOver4;
				while (angle - rightShoulder.TargetAngle + 0.01f > Math.PI)
					angle -= MathHelper.TwoPi;
				while (angle - rightShoulder.TargetAngle + 0.01f < -Math.PI)
					angle += MathHelper.TwoPi;
				rightShoulder.TargetAngle = angle;

				leftElbow.TargetAngle = MathHelper.PiOver4;
				rightElbow.TargetAngle = -MathHelper.PiOver4;
			}
			else
			{
				// Change for carrying light, mid, or heavy weapons?
				float angle = -(float)Math.Atan2(aimVector.Y, aimVector.X) - MathHelper.PiOver2 + torso.Rotation;
				while (angle - leftShoulder.TargetAngle + 0.01f > Math.PI)
					angle -= MathHelper.TwoPi;
				while (angle - leftShoulder.TargetAngle + 0.01f < -Math.PI)
					angle += MathHelper.TwoPi;
				leftShoulder.TargetAngle = angle - 0.05f;

				angle = -(float)Math.Atan2(aimVector.Y, aimVector.X) - MathHelper.PiOver2 + torso.Rotation;
				while (angle - rightShoulder.TargetAngle + 0.01f > Math.PI)
					angle -= MathHelper.TwoPi;
				while (angle - rightShoulder.TargetAngle + 0.01f < -Math.PI)
					angle += MathHelper.TwoPi;
				rightShoulder.TargetAngle = angle + 0.05f;

				leftElbow.TargetAngle = 0.05f;
				rightElbow.TargetAngle = -0.05f;

				Aiming = false;
			}
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(MainGame.tex_torso, torso.Position * MainGame.METER_TO_PIXEL, null, Color.White, torso.Rotation, new Vector2(5f, 20f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, leftUpperArm.Position * MainGame.METER_TO_PIXEL, null, Color.White, leftUpperArm.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, rightUpperArm.Position * MainGame.METER_TO_PIXEL, null, Color.White, rightUpperArm.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, leftLowerArm.Position * MainGame.METER_TO_PIXEL, null, Color.White, leftLowerArm.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, rightLowerArm.Position * MainGame.METER_TO_PIXEL, null, Color.White, rightLowerArm.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, leftUpperLeg.Position * MainGame.METER_TO_PIXEL, null, Color.White, leftUpperLeg.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, rightUpperLeg.Position * MainGame.METER_TO_PIXEL, null, Color.White, rightUpperLeg.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, leftLowerLeg.Position * MainGame.METER_TO_PIXEL, null, Color.White, leftLowerLeg.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_limb, rightLowerLeg.Position * MainGame.METER_TO_PIXEL, null, Color.White, rightLowerLeg.Rotation, new Vector2(5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);
			sb.Draw(MainGame.tex_head, head.Position * MainGame.METER_TO_PIXEL, null, Color.White, head.Rotation, new Vector2(12.5f, 12.5f), 1.0f, SpriteEffects.None, 0.0f);

			// Debug
//			sb.DrawString(MainGame.fnt_basicFont, "x", LeftHandPosition * MainGame.METER_TO_PIXEL, Color.Red);
//			sb.DrawString(MainGame.fnt_basicFont, "x", RightHandPosition * MainGame.METER_TO_PIXEL, Color.Red);
		}
	}
}
