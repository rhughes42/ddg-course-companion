// Mesh.cs
// Discrete Differential Geometry - Halfedge Mesh Data Structure
// Added by Graph Technologies, 2025
// Description: Complete halfedge mesh implementation for .NET

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace DDGCompanion.Core
{
    public class Mesh
    {
        public List<Vertex> Vertices { get; set; } = new();
        public List<Edge> Edges { get; set; } = new();
        public List<Face> Faces { get; set; } = new();
        public List<HalfEdge> HalfEdges { get; set; } = new();
        
        public void Build(Vector3[] positions, int[,] faceIndices)
        {
            Vertices.Clear();
            Edges.Clear();
            Faces.Clear();
            HalfEdges.Clear();
            
            // Create vertices
            for (int i = 0; i < positions.Length; i++)
            {
                Vertices.Add(new Vertex(positions[i]) { Index = i });
            }
            
            // Track halfedges by edge key
            var halfedgeMap = new Dictionary<(int, int), HalfEdge>();
            
            // Create faces and halfedges
            for (int i = 0; i < faceIndices.GetLength(0); i++)
            {
                var face = new Face { Index = i };
                var faceHalfEdges = new List<HalfEdge>();
                
                for (int j = 0; j < 3; j++)
                {
                    int v0 = faceIndices[i, j];
                    int v1 = faceIndices[i, (j + 1) % 3];
                    
                    var he = new HalfEdge
                    {
                        Vertex = Vertices[v1],
                        Face = face,
                        Index = HalfEdges.Count
                    };
                    
                    faceHalfEdges.Add(he);
                    halfedgeMap[(v0, v1)] = he;
                    HalfEdges.Add(he);
                }
                
                // Set next pointers
                for (int j = 0; j < 3; j++)
                {
                    faceHalfEdges[j].Next = faceHalfEdges[(j + 1) % 3];
                }
                
                face.HalfEdge = faceHalfEdges[0];
                Faces.Add(face);
            }
            
            // Set twin pointers and create edges
            foreach (var (key, he) in halfedgeMap)
            {
                var (v0, v1) = key;
                var twinKey = (v1, v0);
                
                if (halfedgeMap.ContainsKey(twinKey))
                {
                    he.Twin = halfedgeMap[twinKey];
                    
                    if (he.Edge == null)
                    {
                        var edge = new Edge
                        {
                            HalfEdge = he,
                            Index = Edges.Count
                        };
                        he.Edge = edge;
                        he.Twin.Edge = edge;
                        Edges.Add(edge);
                    }
                }
            }
            
            // Set vertex halfedges
            foreach (var he in HalfEdges)
            {
                if (he.Vertex!.HalfEdge == null)
                {
                    he.Vertex.HalfEdge = he;
                }
            }
        }
        
        public int EulerCharacteristic() => Vertices.Count - Edges.Count + Faces.Count;
        
        public Vector3[] GetPositions()
        {
            return Vertices.Select(v => v.Position).ToArray();
        }
        
        public void SetPositions(Vector3[] positions)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Position = positions[i];
            }
        }
        
        public void Center()
        {
            var centroid = Vector3.Zero;
            foreach (var v in Vertices)
                centroid += v.Position;
            centroid /= Vertices.Count;
            
            foreach (var v in Vertices)
                v.Position -= centroid;
        }
        
        public void Normalize()
        {
            Center();
            
            float maxDist = 0;
            foreach (var v in Vertices)
                maxDist = Math.Max(maxDist, v.Position.Length());
            
            if (maxDist > 0)
            {
                foreach (var v in Vertices)
                    v.Position /= maxDist;
            }
        }
    }
}
