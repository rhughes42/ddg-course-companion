// GeodesicDistance.cs
// Example: Computing geodesic distances using heat method
// Added by Graph Technologies, 2025

using System;
using System.Linq;
using System.Numerics;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Examples
{
    public class GeodesicDistance
    {
        public static void Run()
        {
            Console.WriteLine("DDG Example: Geodesic Distance (Heat Method)");
            Console.WriteLine("============================================\n");
            
            // Create simple planar mesh
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0),
                new(0.5f, 1, 0),
                new(1.5f, 1, 0)
            };
            
            var faces = new int[,]
            {
                { 0, 1, 3 },
                { 1, 4, 3 },
                { 1, 2, 4 }
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            
            Console.WriteLine("Computing geodesic distance from vertex 0...\n");
            
            // Compute distances
            var distances = HeatMethod.Compute(mesh, 0);
            
            Console.WriteLine("Geodesic distances:");
            for (int i = 0; i < distances.Length; i++)
            {
                Console.WriteLine($"  d(v0, v{i}) = {distances[i]:F6}");
            }
            
            // Verify properties
            Console.WriteLine("\nProperties:");
            Console.WriteLine($"  d(v0, v0) = {distances[0]:F6} (should be 0)");
            Console.WriteLine($"  All distances non-negative: {(distances.All(d => d >= 0) ? "✓" : "✗")}");
            
            // Compare to Euclidean distance
            Console.WriteLine("\nComparison to Euclidean distance:");
            for (int i = 1; i < mesh.Vertices.Count; i++)
            {
                float euclidean = Vector3.Distance(
                    mesh.Vertices[i].Position,
                    mesh.Vertices[0].Position
                );
                double ratio = distances[i] / euclidean;
                Console.WriteLine($"  v{i}: geodesic = {distances[i]:F4}, " +
                                 $"Euclidean = {euclidean:F4} (ratio: {ratio:F4})");
            }
        }
    }
}
