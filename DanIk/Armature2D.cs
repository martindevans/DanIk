using System.Collections.Generic;

namespace DanIk
{
    public class Armature2D
    {
        private readonly Bone[] _bones;

        public IReadOnlyList<Bone> Bones => _bones;

        public Armature2D(params Bone[] bones)
        {
            _bones = bones;

            for (var i = 1; i < bones.Length; i++)
                bones[i].Parent = bones[i - 1];
        }
    }

    public class Bone
    {
        public float Length { get; }

        public float LengthSq { get; }

        public float LimitMin { get; }

        public float LimitMax { get; }

        public Bone Parent { get; internal set; }

        public Bone(float length, float limitMin, float limitMax)
        {
            Length = length;
            LengthSq = length * length;

            LimitMin = limitMin;
            LimitMax = limitMax;
        }
    }
}
