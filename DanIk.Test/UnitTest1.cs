using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DanIk.InverseKinematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrimitiveSvgBuilder;

namespace DanIk.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static float ToRadians(float degrees)
        {
            return (degrees / 360f) * ((float)Math.PI * 2);
        }

        private static float ToDegrees(float radians)
        {
            return (radians / ((float)Math.PI * 2)) * 360f;
        }

        private static void DrawArmature(Armature2D armature, IReadOnlyList<float> solution, Vector2 target)
        {
            var builder = new SvgBuilder(10);
            builder.Outline(new[] {
                new Vector2(-50, -50),
                new Vector2(50, -50),
                new Vector2(50, 50),
                new Vector2(-50, 50),
            });

            var index = 0;
            var pos = Vector2.Zero;
            var parentTransform = Matrix4x4.Identity;

            foreach (var bone in armature.Bones)
            {
                builder.Circle(pos, 0.5f, "yellow");

                var localTransform = TriangulationSolver2D.CalculateLocalTransform(bone, ToRadians(solution[index]));
                parentTransform = localTransform * parentTransform;
                var end = Vector2.Transform(new Vector2(0, 0), parentTransform);
                index++;

                builder.Line(pos, end, 2, "red");

                pos = end;
            }

            builder.Circle(target, 0.5f, "blue");

            var str = Regex.Replace(builder.ToString(), "g transform=\"translate\\((.+?), (.+?)\\)\"", "g transform=\"translate(${1}, 100) scale(1, -1)\"");

            Console.WriteLine(str);
        }

        [TestMethod]
        public void SingleBonePointsToRightTarget()
        {
            var arm = new Armature2D(new Bone(10, -90, 90));
            var solver = new TriangulationSolver2D(arm);

            //Target point is off to the right (and too close to actually reach).
            //Should point almost at the target
            var solution = solver.Solve(new Vector2(5, 0)).ToArray();
            DrawArmature(arm, solution, new Vector2(5, 0));

            Assert.AreEqual(90, solution.Single(), 2);
        }

        [TestMethod]
        public void SingleBonePointsToLeftTarget()
        {
            var arm = new Armature2D(new Bone(10, -90, 90));
            var solver = new TriangulationSolver2D(arm);

            //Target point is off to the left (and too close to actually reach).
            //Should point almost at the target
            var solution = solver.Solve(new Vector2(-5, 0)).ToArray();
            DrawArmature(arm, solution, new Vector2(-5, 0));

            Assert.AreEqual(-90, solution.Single(), 2);
        }

        [TestMethod]
        public void TwoBonesPointToRightTarget()
        {
            var arm = new Armature2D(new Bone(10, -180, 180), new Bone(10, -180, 180));
            var solver = new TriangulationSolver2D(arm);

            //Target point is off to the right (and too close to actually reach).
            //Should point almost at the target
            var solution = solver.Solve(new Vector2(5, 0)).ToArray();
            DrawArmature(arm, solution, new Vector2(5, 0));

            Assert.AreEqual(14, solution[0], 2);
            Assert.AreEqual(150, solution[1], 2);
        }

        [TestMethod]
        public void TwoBonesPointToLeftTarget()
        {
            var arm = new Armature2D(new Bone(10, -180, 180), new Bone(10, -180, 180));
            var solver = new TriangulationSolver2D(arm);

            //Target point is off to the right (and too close to actually reach).
            //Should point almost at the target
            var solution = solver.Solve(new Vector2(-5, 0)).ToArray();
            DrawArmature(arm, solution, new Vector2(-5, 0));

            Assert.AreEqual(-14, solution[0], 2);
            Assert.AreEqual(-150, solution[1], 2);
        }

        [TestMethod]
        public void TheFinalTest()
        {
            var arm = new Armature2D(
                new Bone(7, 0, 80),
                new Bone(7, -80, 80),
                new Bone(7, -80, 80)
            );
            var solver = new TriangulationSolver2D(arm);

            //Change this target point, grab the output from the test runner console
            var target = new Vector2(-15, 0);

            var solution = solver.Solve(target).ToArray();
            DrawArmature(arm, solution, target);
        }
    }
}
