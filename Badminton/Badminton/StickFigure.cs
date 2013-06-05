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
		private AngleJoint neck, leftShoulder, rightShoulder, leftElbow, rightElbow, leftHip, rightHip, leftKnee, rightKnee;

		public Vector2 Position { get { return torso.Position; } }

		public StickFigure(World world, Vector2 position, Category collisionCat)
		{
			GenerateBody(world, position, collisionCat);
			ConnectBody(world);
		}

		private void GenerateBody(World world, Vector2 position, Category collisionCat)
		{
			torso = BodyFactory.CreateCapsule(world, 40 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			torso.Position = position;
			torso.BodyType = BodyType.Dynamic;
			torso.CollisionCategories = collisionCat;
			torso.CollidesWith = Category.All & ~collisionCat;

			head = BodyFactory.CreateCircle(world, 12.5f * MainGame.PIXEL_TO_METER, 1.0f);
			head.Position = torso.Position - new Vector2(0, 29f) * MainGame.PIXEL_TO_METER;
			head.BodyType = BodyType.Dynamic;
			head.CollisionCategories = collisionCat;
			head.CollidesWith = Category.All & ~collisionCat;

			leftUpperArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			leftUpperArm.Rotation = -MathHelper.PiOver2;
			leftUpperArm.Position = torso.Position + new Vector2(-7.5f, -15) * MainGame.PIXEL_TO_METER;
			leftUpperArm.BodyType = BodyType.Dynamic;
			leftUpperArm.CollisionCategories = collisionCat;
			leftUpperArm.CollidesWith = Category.All & ~collisionCat;

			rightUpperArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			rightUpperArm.Rotation = MathHelper.PiOver2;
			rightUpperArm.Position = torso.Position + new Vector2(7.5f, -15) * MainGame.PIXEL_TO_METER;
			rightUpperArm.BodyType = BodyType.Dynamic;
			rightUpperArm.CollisionCategories = collisionCat;
			rightUpperArm.CollidesWith = Category.All & ~collisionCat;

			leftLowerArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
			leftLowerArm.Rotation = -MathHelper.PiOver2;
			leftLowerArm.Position = torso.Position + new Vector2(-22.5f, -15) * MainGame.PIXEL_TO_METER;
			leftLowerArm.BodyType = BodyType.Dynamic;
			leftLowerArm.CollisionCategories = collisionCat;
			leftLowerArm.CollidesWith = Category.All & ~collisionCat;

			rightLowerArm = BodyFactory.CreateCapsule(world, 25 * MainGame.PIXEL_TO_METER, 5 * MainGame.PIXEL_TO_METER, 1.0f);
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
			RevoluteJoint r_neck = JointFactory.CreateRevoluteJoint(world, head, torso, -Vector2.UnitY * 20 * MainGame.PIXEL_TO_METER);
			neck = JointFactory.CreateAngleJoint(world, head, torso);
			neck.CollideConnected = false;
			neck.MaxImpulse = 0.0f;

			RevoluteJoint r_leftShoulder = JointFactory.CreateRevoluteJoint(world, leftUpperArm, torso, -Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			leftShoulder = JointFactory.CreateAngleJoint(world, leftUpperArm, torso);
			leftShoulder.CollideConnected = false;
			leftShoulder.MaxImpulse = 0.0f;

			RevoluteJoint r_rightShoulder = JointFactory.CreateRevoluteJoint(world, rightUpperArm, torso, -Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			rightShoulder = JointFactory.CreateAngleJoint(world, rightUpperArm, torso);
			rightShoulder.CollideConnected = false;
			rightShoulder.MaxImpulse = 0.0f;

			RevoluteJoint r_leftElbow = JointFactory.CreateRevoluteJoint(world, leftLowerArm, leftUpperArm, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			leftElbow = JointFactory.CreateAngleJoint(world, leftLowerArm, leftUpperArm);
			leftElbow.CollideConnected = false;
			leftElbow.MaxImpulse = 0.0f;

			RevoluteJoint r_rightElbow = JointFactory.CreateRevoluteJoint(world, rightLowerArm, rightUpperArm, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			rightElbow = JointFactory.CreateAngleJoint(world, rightLowerArm, rightUpperArm);
			rightElbow.CollideConnected = false;
			rightElbow.MaxImpulse = 0.0f;

			RevoluteJoint r_leftHip = JointFactory.CreateRevoluteJoint(world, leftUpperLeg, torso, Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			leftHip = JointFactory.CreateAngleJoint(world, leftUpperLeg, torso);
			leftHip.CollideConnected = false;
			leftHip.MaxImpulse = 0.0f;

			RevoluteJoint r_rightHip = JointFactory.CreateRevoluteJoint(world, rightUpperLeg, torso, Vector2.UnitY * 15 * MainGame.PIXEL_TO_METER);
			rightHip = JointFactory.CreateAngleJoint(world, rightUpperLeg, torso);
			rightHip.CollideConnected = false;
			rightHip.MaxImpulse = 0.0f;

			RevoluteJoint r_leftKnee = JointFactory.CreateRevoluteJoint(world, leftLowerLeg, leftUpperLeg, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			leftKnee = JointFactory.CreateAngleJoint(world, leftUpperLeg, leftLowerLeg);
			leftKnee.CollideConnected = false;
			leftKnee.MaxImpulse = 0.0f;

			RevoluteJoint r_rightKnee = JointFactory.CreateRevoluteJoint(world, rightLowerLeg, rightUpperLeg, -Vector2.UnitY * 7.5f * MainGame.PIXEL_TO_METER);
			rightKnee = JointFactory.CreateAngleJoint(world, rightUpperLeg, rightLowerLeg);
			rightKnee.CollideConnected = false;
			rightKnee.MaxImpulse = 0.0f;
		}

		public void Move(Vector2 direction)
		{
			this.torso.ApplyForce(ref direction);
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
		}
	}
}
