using System;
using System.Collections.Generic;
using System.Numerics;

namespace DanIk.InverseKinematics
{
    public class TriangulationSolver2D
        : IIKSolver
    {
        private readonly Armature2D _arm;

        private readonly Dictionary<Bone, float> _armatureSubsequentLength = new Dictionary<Bone, float>();

        public TriangulationSolver2D(Armature2D arm)
        {
            _arm = arm;

            var length = 0f;
            for (var i = arm.Bones.Count - 1; i >= 0; i--)
            {
                _armatureSubsequentLength[arm.Bones[i]] = length;
                length += arm.Bones[i].Length;
            }
        }

        public IEnumerable<float> Solve(Vector2 target)
        {
            var solution = new List<float>();

            for (var i = 0; i < _arm.Bones.Count; i++)
            {
                var bone = _arm.Bones[i];

                //Calculate rotation in degrees and limit by this joint limit
                var rotDegrees = (CalculateBoneRotation(target, bone) / ((float)Math.PI * 2)) * 360;
                rotDegrees = Math.Min(bone.LimitMax, Math.Max(bone.LimitMin, rotDegrees));
                solution.Add(rotDegrees);

                var rotRadians = (float)(rotDegrees / 360 * (Math.PI * 2));

                var transform = CalculateLocalTransform(bone, rotRadians);

                Matrix4x4 inv;
                Matrix4x4.Invert(transform, out inv);
                var t3 = Vector3.Transform(new Vector3(target, 0), inv);
                target = new Vector2(t3.X, t3.Y);
            }

            return solution;
        }

        private float CalculateBoneRotation(Vector2 target, Bone bone)
        {
            //Cosine rule: http://www.bbc.co.uk/schools/gcsebitesize/maths/geometry/furthertrigonometryhirev2.shtml
            var a = _armatureSubsequentLength[bone];
            var c = bone.Length;
            var b = target.Length();

            //If this is the last bone then the length of the arm after this point will be zero
            //Fake the lengths to make the last bone point at the target
            if (Math.Abs(a) < float.Epsilon)
            {
                a = 0.1f;
                c = b;
            }

            var cosineAngle = (b * b + c * c - a * a) / (2 * b * c);
            var acosRule = (float)Math.Acos(Math.Max(-1, Math.Min(1, cosineAngle)));

            var normalizedTargetVector = target / target.Length();
            var acosDot = (float)Math.Acos(Dot(Vector2.UnitY, normalizedTargetVector));
            var angle = acosDot - acosRule;
            if (Math.Abs(angle) < 0.01f)
                return 0;

            var axis = Vector3.Cross(Vector3.UnitY, new Vector3(normalizedTargetVector, 0));
            if (axis.Length() < 0.01f)
                return 0;

            if (axis.Z > 0)
                return -angle;
            return angle;
        }

        //private float CalculateBoneRotation(Vector2 target, Bone bone)
        //{
        //    var targetDistance = target.Length();
        //    var normalizedTargetVector = target / targetDistance;

        //    var acosDot = (float)Math.Acos(Dot(Vector2.UnitY, normalizedTargetVector));

        //    var consineRule = -(_armatureSubsequentLength[bone] - bone.LengthSq - targetDistance * targetDistance) / (2 * bone.Length * targetDistance);
        //    var acosRule = (float)Math.Acos(Math.Max(-1, Math.Min(1, consineRule)));

        //    var angle = acosDot - acosRule;
        //    if (Math.Abs(angle) < 0.01f)
        //        return 0;

        //    var axis = Vector3.Cross(Vector3.UnitY, new Vector3(normalizedTargetVector, 0));

        //    if (axis.Length() < 0.01f)
        //        return 0;

        //    if (axis.Z > 0)
        //        return -angle;
        //    return angle;
        //}

        private static float Dot(Vector2 a, Vector2 b)
        {
            var dot = Vector2.Dot(a, b);
            return Math.Max(-1, Math.Min(1, dot));
        }

        public static Matrix4x4 CalculateLocalTransform(Bone bone, float rotation)
        {
            return Matrix4x4.CreateTranslation(0, bone.Length, 0)
                 * Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, rotation));
        }
    }
}
