
//using System.Numerics;

//namespace DanIk
//{
//    public class Bone
//    {
//        public float Length { get; private set; }
//        public float LengthSquared { get; private set; }

//        public Bone Parent { get; internal set; }

//        public JointLimit JointLimit { get; internal set; }

//        private Quaternion _rotation = Quaternion.Identity;
//        /// <summary>
//        /// Get or set the rotation of this bone relative to the parent
//        /// </summary>
//        public Quaternion Rotation
//        {
//            get { return _rotation;  }
//            set { _rotation = value; _isLocalBoneTransformDirty = true; _isWorldBoneTransformDirty = true; }
//        }

//        private bool _isLocalBoneTransformDirty = true;
//        private Matrix4x4 _localBoneTransform;
//        /// <summary>
//        /// Get the transformation of this bone relative to the parent
//        /// </summary>
//        public Matrix4x4 LocalTransform
//        {
//            get
//            {
//                if (_isLocalBoneTransformDirty)
//                {
//                    _localBoneTransform = CalculateLocalTransform(_rotation);
//                    _isLocalBoneTransformDirty = false;
//                }
//                return _localBoneTransform;
//            }
//        }

//        private bool _isWorldBoneTransformDirty = true;
//        private Matrix4x4 _worldBoneTransform;
//        /// <summary>
//        /// Gets the complete world transform this bone applies
//        /// </summary>
//        public Matrix4x4 WorldTransform
//        {
//            get
//            {
//                if (_isWorldBoneTransformDirty)
//                {
//                    _worldBoneTransform = CalculateWorldTransform(LocalTransform);
//                    _isWorldBoneTransformDirty = false;
//                }
//                return _worldBoneTransform;
//            }
//        }

//        internal Bone(BoneParameters parameters)
//        {
//            Length = parameters.Length;
//            LengthSquared = Length * Length;

//            JointLimit = parameters.Limit;
//        }

//        public Matrix4x4 CalculateWorldTransform(Matrix4x4 localTransform)
//        {
//            return Parent == null ? localTransform : localTransform * Parent.WorldTransform;
//        }

//        public Matrix4x4 CalculateLocalTransform(Quaternion rotation)
//        {
//            return Matrix4x4.CreateTranslation(0, Length, 0) * Matrix4x4.CreateFromQuaternion(rotation);
//        }

//        public struct BoneParameters
//        {
//            public float Length;
//            public JointLimit Limit;
//        }
//    }
//}
