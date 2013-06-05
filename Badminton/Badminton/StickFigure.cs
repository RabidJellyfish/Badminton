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
		public bool Aiming { get; set; }

		private float maxImpulse;
		private Vector2 aimVector;
		private int walkStage = 0;

		public StickFigure(World world, Vector2 position, Category collisionCat)
		{
			maxImpulse = 3f;
			Aiming = false;
			GenerateBody(world, position, collisionCat);
			ConnectBody(world);
			Stand();
		}

		#region Creation
		private void GenerateBody(World world, Vector2 position, Category collisionCat)
		{
			torso = BodyFactory.CreateCapsule(world, 40 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 15.0f);
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

			leftUpperLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			leftUpperLeg.Rotation = -3 * MathHelper.PiOver4;
			leftUpperLeg.Position = torso.Position + new Vector2(-25f / (float)Math.Sqrt(8) + 4, 10 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			leftUpperLeg.BodyType = BodyType.Dynamic;
			leftUpperLeg.CollisionCategories = collisionCat;
			leftUpperLeg.CollidesWith = Category.All & ~collisionCat;

			rightUpperLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			rightUpperLeg.Rotation = 3 * MathHelper.PiOver4;
			rightUpperLeg.Position = torso.Position + new Vector2(25f / (float)Math.Sqrt(8) - 4, 10 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			rightUpperLeg.BodyType = BodyType.Dynamic;
			rightUpperLeg.CollisionCategories = collisionCat;
			rightUpperLeg.CollidesWith = Category.All & ~collisionCat;

			leftLowerLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			leftLowerLeg.Position = torso.Position + new Vector2(-50f / (float)Math.Sqrt(8) + 6, 25 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			leftLowerLeg.BodyType = BodyType.Dynamic;
			leftLowerLeg.CollisionCategories = collisionCat;
			leftLowerLeg.CollidesWith = Category.All & ~collisionCat;

			rightLowerLeg = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			rightLowerLeg.Position = torso.Position + new Vector2(50f / (float)Math.Sqrt(8) - 6, 25 + 25f / (float)Math.Sqrt(8)) * MainGame.PIXEL_TO_METER;
			rightLowerLeg.BodyType = BodyType.Dynamic;
			rightLowerLeg.CollisionCategories = collisionCat;
			rightLowerLeg.CollidesWith = Category.All & ~collisionCat;
		}
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
		#endregion

		#region Stances

		public void Stand()
		{
			upright.TargetAngle = 0.0f;
			leftHip.TargetAngle = 3 * MathHelper.PiOver4;
			leftKnee.TargetAngle = -5 * MathHelper.PiOver4;
			rightHip.TargetAngle = -3 * MathHelper.PiOver4;
			rightKnee.TargetAngle = -3 * MathHelper.PiOver4;
		}
		public void Aim(Vector2 position)
		{
			this.Aiming = true;
			this.aimVector = position - (torso.Position - 15f * Vector2.UnitY * MainGame.PIXEL_TO_METER);
		}
		public void WalkRight()
		{
			upright.TargetAngle = -0.15f;
			AngleJoint[] checkThese = new AngleJoint[] { leftHip, leftKnee, rightHip, rightKnee };
			if (walkStage == 0)
			{
				leftHip.TargetAngle = (float)Math.PI - torso.Rotation;
				leftKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightHip.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 1;
			}
			else if (walkStage == 1)
			{
				leftHip.TargetAngle = 3 * MathHelper.PiOver2 - torso.Rotation;
				leftKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightHip.TargetAngle = -5 * MathHelper.PiOver4 - torso.Rotation;
				rightKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 2;
			}
			else if (walkStage == 2)
			{
				leftHip.TargetAngle = 5 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -3 * MathHelper.PiOver4 - torso.Rotation;
				rightHip.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 3;
			}
			else if (walkStage == 3)
			{
				leftHip.TargetAngle = 3 * MathHelper.PiOver4 - torso.Rotation;
				leftKnee.TargetAngle = -(float)Math.PI - torso.Rotation;
				rightHip.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				rightKnee.TargetAngle = -MathHelper.PiOver2 - torso.Rotation;
				if (JointsAreInPosition(checkThese))
					walkStage = 0;
			}
		}
		public void WalkLeft()
		{
			// wait until WalkRight() works
		}

		private bool JointsAreInPosition(AngleJoint[] joints)
		{
			foreach (AngleJoint j in joints)
			{
				if (Math.Abs(j.BodyB.Rotation - j.BodyA.Rotation - j.TargetAngle) > 0.25)
					return false;
			}
			return true;
		}

		#endregion

		public void Update()
		{
			UpdateArms();
		}

		private void UpdateArms()
		{
			if (!Aiming)
			{
				leftShoulder.TargetAngle = 3 * MathHelper.PiOver4;
				rightShoulder.TargetAngle = -3 * MathHelper.PiOver4;
				leftElbow.TargetAngle = 3 * MathHelper.PiOver4;
				rightElbow.TargetAngle = -3 * MathHelper.PiOver4;
			}
			else
			{
				float angle = -(float)Math.Atan2(aimVector.Y, aimVector.X) - MathHelper.PiOver2 + torso.Rotation;
				while (angle - leftShoulder.TargetAngle + 0.01f > Math.PI)
					angle -= MathHelper.TwoPi;
				while (angle - leftShoulder.TargetAngle + 0.01f < -Math.PI)
					angle += MathHelper.TwoPi;
				leftShoulder.TargetAngle = angle - 0.05f;
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

			sb.DrawString(MainGame.fnt_basicFont, leftShoulder.TargetAngle.ToString(), Vector2.Zero, Color.Black);
		}
	}
}
