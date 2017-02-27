//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Numerics;

//namespace DanIk
//{
//    /// <summary>
//    /// A series of bones connected together
//    /// </summary>
//    public class Armature
//    {
//        private readonly List<Bone> _bones = new List<Bone>();

//        /// <summary>
//        /// Store the length of the armature after each bone.
//        /// Why not store this as a field of bone? Because a single bone could be in several armatures
//        /// </summary>
//        private readonly Dictionary<Bone, float> _armatureSubsequentLengthsSquared = new Dictionary<Bone, float>();

//        private readonly ReadOnlyCollection<Bone> _readonlyBones;
//        public ReadOnlyCollection<Bone> Bones
//        {
//            get { return _readonlyBones; }
//        }

//        //Animation
//        private bool _animating = false;
//        private float _accumulatedAnimationTime;
//        private float _animationDuration;
//        private Quaternion[] _animationStart;
//        private Quaternion[] _animationEnd;

//        public Armature(params Bone.BoneParameters[] bones)
//        {
//            _readonlyBones = new ReadOnlyCollection<Bone>(_bones);

//            for (int i = 0; i < bones.Length; i++)
//                Add(new Bone(bones[i]));
//            UpdateChain();
//        }

//        private void Add(Bone bone)
//        {
//            var last = _bones.LastOrDefault();
//            if (last != null)
//                bone.Parent = last;
//            _bones.Add(bone);
//        }

//        private void UpdateChain()
//        {
//            //TODO: Set all the joint to either no rotation (with no joint limit) or the largest possible rotation on the most constrained axis (with a joint limit)
//            for (int i = 0; i < _bones.Count; i++)
//            {
//                var b = _bones[i];
//                //if (b.JointLimit == null)
//                    b.Rotation = Quaternion.Identity;
//                //else
//                //{
//                //    throw new NotImplementedException();
//                //}
//            }

//            Vector3 chainEnd = Vector3.Transform(Vector3.Zero, _bones.Last().WorldTransform);

//            for (int i = _bones.Count - 1; i >= 0; i--)
//            {
//                var b = _bones[i];
//                var boneEnd = Vector3.Transform(Vector3.Zero, b.WorldTransform);

//                _armatureSubsequentLengthsSquared[b] = (chainEnd - boneEnd).LengthSquared();
//            }

//            _animationStart = new Quaternion[_bones.Count];
//            _animationEnd = new Quaternion[_bones.Count];
//        }

//        public void SetAnimationTarget(IEnumerable<Quaternion> boneRotations, float animationDuration)
//        {
//            for (int i = 0; i < _bones.Count; i++)
//                _animationStart[i] = _bones[i].Rotation;

//            var index = 0;
//            foreach (Quaternion boneRotation in boneRotations)
//            {
//                _animationEnd[index++] = boneRotation;
//                if (index > _bones.Count)
//                    throw new InvalidOperationException("Attempted to set more bone rotations into animation than bones");
//            }

//            _animationDuration = animationDuration;
//            _accumulatedAnimationTime = 0;
//            _animating = true;
//        }

//        public void Update(float dt)
//        {
//            if (!_animating)
//                return;

//            _accumulatedAnimationTime += dt;
//            if (_accumulatedAnimationTime >= _animationDuration)
//            {
//                _animating = false;
//                for (int i = 0; i < _animationEnd.Length; i++)
//                    _bones[i].Rotation = _animationEnd[i];
//            }
//            else
//            {
//                var t = _accumulatedAnimationTime / _animationDuration;
//                for (int i = 0; i < _animationEnd.Length; i++)
//                    _bones[i].Rotation = Quaternion.Slerp(_animationStart[i], _animationEnd[i], t);
//            }
//        }

//        internal float GetSubsequentArmatureLengthSquared(Bone b)
//        {
//            return _armatureSubsequentLengthsSquared[b];
//        }
//    }
//}
