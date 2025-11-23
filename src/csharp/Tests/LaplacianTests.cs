// LaplacianTests.cs
// Unit tests for Laplacian operator
// Added by Graph Technologies, 2025

using System;
using System.Numerics;
using Xunit;
using MathNet.Numerics.LinearAlgebra;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Tests
{
    public class LaplacianTests
    {
        private Mesh CreateSimpleMesh()
        {
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(0.5f, 0.866f, 0),
                new(0.5f, 0.289f, 0.816f)
            };
            
            var faces = new int[,]
            {
                { 0, 1, 2 },
                { 0, 1, 3 },
                { 0, 2, 3 },
                { 1, 2, 3 }
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            return mesh;
        }
        
        [Fact]
        public void TestLaplacianDimensions()
        {
            var mesh = CreateSimpleMesh();
            var L = CotanLaplacian.Build(mesh);
            
            Assert.Equal(mesh.Vertices.Count, L.RowCount);
            Assert.Equal(mesh.Vertices.Count, L.ColumnCount);
        }
        
        [Fact]
        public void TestZeroRowSum()
        {
            var mesh = CreateSimpleMesh();
            var L = CotanLaplacian.Build(mesh);
            
            // Each row should sum to zero
            for (int i = 0; i < L.RowCount; i++)
            {
                double rowSum = L.Row(i).Sum();
                Assert.True(Math.Abs(rowSum) < 1e-9);
            }
        }
        
        [Fact]
        public void TestMassMatrixDiagonal()
        {
            var mesh = CreateSimpleMesh();
            var M = CotanLaplacian.BuildMassMatrix(mesh);
            
            // Mass matrix should be diagonal
            for (int i = 0; i < M.RowCount; i++)
            {
                for (int j = 0; j < M.ColumnCount; j++)
                {
                    if (i != j)
                    {
                        Assert.Equal(0.0, M[i, j], 10);
                    }
                    else
                    {
                        Assert.True(M[i, i] > 0);
                    }
                }
            }
        }
    }
}
