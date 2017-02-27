using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DanIk.InverseKinematics;
using PrimitiveSvgBuilder;

namespace DanIk
{
    public class Program
    {
        private static readonly string[] _defaultArgs = { "0", "0", "0" };

        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("You should call this program with 3 arguments.");
                Console.WriteLine("Optionally also attach '--svg' as an additional argument to show an SVG visualisation of the arm");
                Console.WriteLine("They should be `X Y Z` of the target marker (in centimeters).");
                Console.WriteLine("Running with default args: " + string.Join(" ", _defaultArgs));
                args = _defaultArgs;
            }

            //Parse target position args
            var x = float.Parse(args[0]);
            var y = float.Parse(args[1]);
            var z = float.Parse(args[1]);
            x = Math.Max(1, x);

            //Create the armature (hardcoded, yay)
            var arm = new Armature2D(
                new Bone(7,   0, 80),
                new Bone(7, -80, 80),
                new Bone(7, -80, 80)
            );

            //Calculate the pose (2D)
            var target2D = new Vector2(Vector2.Distance(new Vector2(x, z), Vector2.Zero), y);
            var pose = new TriangulationSolver2D(arm).Solve(target2D).ToArray();

            //Calculate the rotation value of the base
            var dot = Vector2.Dot(new Vector2(1, 0), Vector2.Normalize(new Vector2(x, z)));
            var cross = Vector3.Cross(new Vector3(1, 0, 0), Vector3.Normalize(new Vector3(x, 0, z)));
            var angle = (Math.Sign(cross.Y) * Math.Acos(dot)) / (Math.PI * 2) * 360;

            //Write out the results
            Console.WriteLine("    base_angle,motor_angle_1,motor_angle_2,motor_angle_3");
            Console.WriteLine($"    {angle},{pose[0]},{pose[1]},{pose[2]}");

            var svg = args.Any(a => a == "--svg");
            var usedDefaults = args == _defaultArgs;

            if (svg)
            {
                Console.WriteLine("Ran with '--svg', dumping svg visualisation");
                DrawArmature(arm, pose, target2D);
            }

            if (svg || usedDefaults)
            {
                Console.WriteLine("Press any key to escape");
                Console.ReadKey();
            }
        }

        private static float ToRadians(float degrees)
        {
            return (degrees / 360f) * ((float)Math.PI * 2);
        }

        public static void DrawArmature(Armature2D armature, IReadOnlyList<float> solution, Vector2 target)
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
    }
}
