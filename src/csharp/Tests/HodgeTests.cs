// HodgeTests.cs
// Unit tests for Hodge Decomposition implementation
// Added by Graph Technologies, 2025

using System;
using System.Numerics;
using Xunit;
using MathNet.Numerics.LinearAlgebra;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Tests
{
    public class HodgeTests
    {
        private Mesh CreateTetrahedron()
        {
            var positions = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 1)
            };
            
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Vector3.Normalize(positions[i]);
            }
            
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
        
        [Fact]
        public void TestD0Dimensions()
        {
            var mesh = CreateTetrahedron();
            var d0 = HodgeDecomposition.BuildD0(mesh);
            
            Assert.Equal(mesh.Edges.Count, d0.RowCount);
            Assert.Equal(mesh.Vertices.Count, d0.ColumnCount);
        }
        
        [Fact]
        public void TestD1Dimensions()
        {
            var mesh = CreateTetrahedron();
            var d1 = HodgeDecomposition.BuildD1(mesh);
            
            Assert.Equal(mesh.Faces.Count, d1.RowCount);
            Assert.Equal(mesh.Edges.Count, d1.ColumnCount);
        }
        
        [Fact]
        public void TestDCompositionIsZero()
        {
            // Test that d∘d = 0 (fundamental property of exterior derivative)
            var mesh = CreateTetrahedron();
            var d0 = HodgeDecomposition.BuildD0(mesh);
            var d1 = HodgeDecomposition.BuildD1(mesh);
            
            var d1d0 = d1.Multiply(d0);
            
            // Check all entries are close to zero
            for (int i = 0; i < d1d0.RowCount; i++)
            {
                for (int j = 0; j < d1d0.ColumnCount; j++)
                {
                    Assert.True(Math.Abs(d1d0[i, j]) < 1e-10, 
                        $"d∘d should be zero but d1d0[{i},{j}] = {d1d0[i, j]}");
                }
            }
        }
        
        [Fact]
        public void TestHodgeStarDimensions()
        {
            var mesh = CreateTetrahedron();
            
            var star0 = HodgeDecomposition.BuildHodgeStar0(mesh);
            Assert.Equal(mesh.Vertices.Count, star0.RowCount);
            Assert.Equal(mesh.Vertices.Count, star0.ColumnCount);
            
            var star1 = HodgeDecomposition.BuildHodgeStar1(mesh);
            Assert.Equal(mesh.Edges.Count, star1.RowCount);
            Assert.Equal(mesh.Edges.Count, star1.ColumnCount);
            
            var star2 = HodgeDecomposition.BuildHodgeStar2(mesh);
            Assert.Equal(mesh.Faces.Count, star2.RowCount);
            Assert.Equal(mesh.Faces.Count, star2.ColumnCount);
        }
        
        [Fact]
        public void TestHodgeStarPositivity()
        {
            // Hodge stars should be positive definite (diagonal entries > 0)
            var mesh = CreateTetrahedron();
            
            var star0 = HodgeDecomposition.BuildHodgeStar0(mesh);
            for (int i = 0; i < star0.RowCount; i++)
            {
                Assert.True(star0[i, i] > 0, $"star0[{i},{i}] should be positive");
            }
            
            var star1 = HodgeDecomposition.BuildHodgeStar1(mesh);
            for (int i = 0; i < star1.RowCount; i++)
            {
                Assert.True(star1[i, i] > 0, $"star1[{i},{i}] should be positive");
            }
            
            var star2 = HodgeDecomposition.BuildHodgeStar2(mesh);
            for (int i = 0; i < star2.RowCount; i++)
            {
                Assert.True(star2[i, i] > 0, $"star2[{i},{i}] should be positive");
            }
        }
        
        [Fact]
        public void TestDecompositionReconstruction()
        {
            // Test that ω = exact + coexact + harmonic
            var mesh = CreateTetrahedron();
            
            // Create a test 1-form
            var omega = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(mesh.Edges.Count);
            var random = new Random(42);
            for (int i = 0; i < mesh.Edges.Count; i++)
            {
                omega[i] = random.NextDouble();
            }
            
            // Decompose
            var components = HodgeDecomposition.Decompose(mesh, omega);
            
            // Reconstruct
            var reconstructed = components.Exact.Add(components.Coexact).Add(components.Harmonic);
            
            // Check reconstruction error
            var error = omega.Subtract(reconstructed).L2Norm();
            Assert.True(error < 1e-6, $"Reconstruction error {error} should be small");
        }
        
        [Fact]
        public void TestDecompositionOnZeroField()
        {
            // Test decomposition of zero field
            var mesh = CreateTetrahedron();
            var omega = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(mesh.Edges.Count);
            
            var components = HodgeDecomposition.Decompose(mesh, omega);
            
            Assert.True(components.Exact.L2Norm() < 1e-10);
            Assert.True(components.Coexact.L2Norm() < 1e-10);
            Assert.True(components.Harmonic.L2Norm() < 1e-10);
        }
        
        [Fact]
        public void TestHarmonicBasesGenus()
        {
            // For a tetrahedron (sphere), genus = 0, so no harmonic bases
            var mesh = CreateTetrahedron();
            var bases = HodgeDecomposition.HarmonicBases(mesh);
            
            int genus = (2 - mesh.EulerCharacteristic()) / 2;
            int expectedDim = 2 * genus;
            
            Assert.Equal(mesh.Edges.Count, bases.RowCount);
            Assert.Equal(expectedDim, bases.ColumnCount);
        }
        
        [Fact]
        public void TestMeshTopology()
        {
            // Verify tetrahedron topology
            var mesh = CreateTetrahedron();
            
            Assert.Equal(4, mesh.Vertices.Count);
            Assert.Equal(6, mesh.Edges.Count);
            Assert.Equal(4, mesh.Faces.Count);
            
            // Euler characteristic for sphere: χ = V - E + F = 2
            int chi = mesh.EulerCharacteristic();
            Assert.Equal(2, chi);
            
            // Genus for sphere: g = (2 - χ) / 2 = 0
            int genus = (2 - chi) / 2;
            Assert.Equal(0, genus);
        }
    }
}
