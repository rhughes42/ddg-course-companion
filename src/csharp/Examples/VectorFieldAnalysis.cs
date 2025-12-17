// VectorFieldAnalysis.cs
// Discrete Differential Geometry - Vector Field Decomposition Example
// Added by Graph Technologies, 2025
// Description: Demonstrates Hodge decomposition of discrete vector fields

using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Examples
{
    public class VectorFieldAnalysis
    {
        public static void Run()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════╗");
            Console.WriteLine("║  Vector Field Decomposition Example       ║");
            Console.WriteLine("║  Hodge Decomposition of 1-Forms            ║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");
            
            // Create a simple test mesh (tetrahedron)
            var mesh = CreateTetrahedron();
            
            Console.WriteLine($"Mesh Statistics:");
            Console.WriteLine($"  Vertices: {mesh.Vertices.Count}");
            Console.WriteLine($"  Edges:    {mesh.Edges.Count}");
            Console.WriteLine($"  Faces:    {mesh.Faces.Count}");
            Console.WriteLine($"  Euler χ:  {mesh.EulerCharacteristic()}");
            
            int genus = (2 - mesh.EulerCharacteristic()) / 2;
            Console.WriteLine($"  Genus:    {genus}\n");
            
            // Create a test 1-form (edge-based vector field)
            var omega = CreateTestVectorField(mesh);
            
            Console.WriteLine("Performing Hodge Decomposition...\n");
            
            // Decompose the vector field
            var components = HodgeDecomposition.Decompose(mesh, omega);
            
            // Verify decomposition: ω = exact + coexact + harmonic
            var reconstructed = components.Exact.Add(components.Coexact).Add(components.Harmonic);
            var error = omega.Subtract(reconstructed).L2Norm();
            
            Console.WriteLine("Decomposition Results:");
            Console.WriteLine($"  Original norm:    {omega.L2Norm():F6}");
            Console.WriteLine($"  Exact norm:       {components.Exact.L2Norm():F6}");
            Console.WriteLine($"  Coexact norm:     {components.Coexact.L2Norm():F6}");
            Console.WriteLine($"  Harmonic norm:    {components.Harmonic.L2Norm():F6}");
            Console.WriteLine($"  Reconstruction error: {error:E6}\n");
            
            // Verify orthogonality
            double exactCoexactDot = components.Exact.DotProduct(components.Coexact);
            double exactHarmonicDot = components.Exact.DotProduct(components.Harmonic);
            double coexactHarmonicDot = components.Coexact.DotProduct(components.Harmonic);
            
            Console.WriteLine("Orthogonality Check:");
            Console.WriteLine($"  <exact, coexact>:   {Math.Abs(exactCoexactDot):E6}");
            Console.WriteLine($"  <exact, harmonic>:  {Math.Abs(exactHarmonicDot):E6}");
            Console.WriteLine($"  <coexact, harmonic>: {Math.Abs(coexactHarmonicDot):E6}\n");
            
            // Display component percentages
            double totalNorm = omega.L2Norm();
            if (totalNorm > 1e-10)
            {
                Console.WriteLine("Component Percentages:");
                Console.WriteLine($"  Exact:    {100.0 * components.Exact.L2Norm() / totalNorm:F2}%");
                Console.WriteLine($"  Coexact:  {100.0 * components.Coexact.L2Norm() / totalNorm:F2}%");
                Console.WriteLine($"  Harmonic: {100.0 * components.Harmonic.L2Norm() / totalNorm:F2}%\n");
            }
            
            // Test operators
            Console.WriteLine("Testing Differential Operators...");
            TestOperators(mesh);
            
            Console.WriteLine("\n✓ Vector Field Analysis Complete\n");
        }
        
        private static Mesh CreateTetrahedron()
        {
            // Regular tetrahedron vertices
            var positions = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 1)
            };
            
            // Normalize to unit sphere
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Vector3.Normalize(positions[i]);
            }
            
            // Tetrahedron faces
            var faces = new int[,]
            {
                { 0, 2, 1 },
                { 0, 1, 3 },
                { 0, 3, 2 },
                { 1, 2, 3 }
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            
            return mesh;
        }
        
        private static MathNet.Numerics.LinearAlgebra.Vector<double> CreateTestVectorField(Mesh mesh)
        {
            // Create a simple test 1-form on edges
            var omega = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(mesh.Edges.Count);
            
            // Initialize with random values (or structured pattern)
            var random = new Random(42);
            for (int i = 0; i < mesh.Edges.Count; i++)
            {
                omega[i] = random.NextDouble() * 2.0 - 1.0;
            }
            
            return omega;
        }
        
        private static void TestOperators(Mesh mesh)
        {
            // Test d0 operator
            var d0 = HodgeDecomposition.BuildD0(mesh);
            Console.WriteLine($"  d0 dimensions: {d0.RowCount} × {d0.ColumnCount}");
            
            // Test d1 operator
            var d1 = HodgeDecomposition.BuildD1(mesh);
            Console.WriteLine($"  d1 dimensions: {d1.RowCount} × {d1.ColumnCount}");
            
            // Test d∘d = 0 property
            var d1d0 = d1.Multiply(d0);
            double maxEntry = 0.0;
            for (int i = 0; i < d1d0.RowCount; i++)
            {
                for (int j = 0; j < d1d0.ColumnCount; j++)
                {
                    maxEntry = Math.Max(maxEntry, Math.Abs(d1d0[i, j]));
                }
            }
            Console.WriteLine($"  d∘d = 0 check: max|d1∘d0| = {maxEntry:E6}");
            
            // Test Hodge stars
            var star0 = HodgeDecomposition.BuildHodgeStar0(mesh);
            var star1 = HodgeDecomposition.BuildHodgeStar1(mesh);
            var star2 = HodgeDecomposition.BuildHodgeStar2(mesh);
            
            Console.WriteLine($"  ⋆0 dimensions: {star0.RowCount} × {star0.ColumnCount}");
            Console.WriteLine($"  ⋆1 dimensions: {star1.RowCount} × {star1.ColumnCount}");
            Console.WriteLine($"  ⋆2 dimensions: {star2.RowCount} × {star2.ColumnCount}");
        }
    }
}
