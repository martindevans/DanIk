//using System;
//using System.Collections.Generic;
//using System.Numerics;

//namespace DanIk.InverseKinematics
//{
//    public class TriangulationSolver
//    : IIKSolver
//    {
//        public IEnumerable<Quaternion> Solve(Armature armature, Vector3 target)
//        {
//            for (int i = 0; i < armature.Bones.Count; i++)
//            {
//                var bone = armature.Bones[i];

//                var rotation = CalculateBoneRotation(armature, target, armature.Bones[i]);
//                yield return rotation;

//                var transform = bone.CalculateLocalTransform(rotation);

//                Matrix4x4 inv;
//                Matrix4x4.Invert(transform, out inv);
//                target = Vector3.Transform(target, inv);
//            }
//        }

//        private static Quaternion CalculateBoneRotation(Armature armature, Vector3 target, Bone bone)
//        {
//            float targetDistance = target.Length();
//            var normalizedTargetVector = target / targetDistance;

//            var acosDot = (float)Math.Acos(Dot(Vector3.UnitY, normalizedTargetVector));

//            var consineRule = -(armature.GetSubsequentArmatureLengthSquared(bone) - bone.LengthSquared - targetDistance * targetDistance) / (2 * bone.Length * targetDistance);
//            var acosRule = (float)Math.Acos(Math.Max(-1, Math.Min(1, consineRule)));

//            var angle = acosDot - acosRule;
//            if (Math.Abs(angle) < 0.01f)
//                return Quaternion.Identity;

//            Vector3 axis = Vector3.Cross(Vector3.UnitY, normalizedTargetVector);

//            if (axis.Length() < 0.01f)
//                axis = Vector3.UnitX;

//            return LimitAngle(bone, Vector3.Normalize(axis), angle);
//        }

//        private static Quaternion LimitAngle(Bone bone, Vector3 axis, float angle)
//        {
//            if (bone.JointLimit != null)
//                return bone.JointLimit.Limit(axis, angle);

//            return Quaternion.CreateFromAxisAngle(axis, angle);
//        }

//        private static float Dot(Vector3 a, Vector3 b)
//        {
//            var dot = Vector3.Dot(a, b);
//            return Math.Max(-1, Math.Min(1, dot));
//        }
//    }
//}
