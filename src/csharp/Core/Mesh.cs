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
            if (positions == null) throw new ArgumentNullException(nameof(positions));
            if (faceIndices == null) throw new ArgumentNullException(nameof(faceIndices));
            if (faceIndices.GetLength(1) != 3)
                throw new ArgumentException("Mesh.Build currently supports triangle faces only.", nameof(faceIndices));

            Vertices.Clear();
            Edges.Clear();
            Faces.Clear();
            HalfEdges.Clear();
            
            // Create vertices
            for (int i = 0; i < positions.Length; i++)
            {
                Vertices.Add(new Vertex(positions[i]) { Index = i });
            }
            
            // Track halfedges by directed edge key and preserve endpoints.
            var halfedgeMap = new Dictionary<(int, int), HalfEdge>();
            var halfedgeEndpoints = new List<(int Source, int Target)>();
            
            // Create faces and halfedges
            for (int i = 0; i < faceIndices.GetLength(0); i++)
            {
                var face = new Face { Index = i };
                var faceHalfEdges = new List<HalfEdge>();
                
                for (int j = 0; j < 3; j++)
                {
                    int v0 = faceIndices[i, j];
                    int v1 = faceIndices[i, (j + 1) % 3];

                    if (v0 < 0 || v0 >= Vertices.Count || v1 < 0 || v1 >= Vertices.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(faceIndices), $"Face {i} contains invalid vertex index.");
                    }
                    if (v0 == v1)
                    {
                        throw new ArgumentException($"Face {i} contains a degenerate edge ({v0} -> {v1}).", nameof(faceIndices));
                    }
                    
                    var he = new HalfEdge
                    {
                        Vertex = Vertices[v1],
                        Face = face,
                        Index = HalfEdges.Count
                    };
                    
                    faceHalfEdges.Add(he);
                    halfedgeMap[(v0, v1)] = he;
                    halfedgeEndpoints.Add((v0, v1));
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
            
            // Set twin pointers where reverse directed edge exists.
            foreach (var (key, he) in halfedgeMap)
            {
                var (v0, v1) = key;
                var twinKey = (v1, v0);

                if (halfedgeMap.TryGetValue(twinKey, out var twin))
                {
                    he.Twin = twin;
                }
            }

            // Create undirected edges independent of winding consistency.
            var edgeMap = new Dictionary<(int, int), Edge>();
            for (int i = 0; i < HalfEdges.Count; i++)
            {
                var he = HalfEdges[i];
                var (source, target) = halfedgeEndpoints[i];
                var key = source < target ? (source, target) : (target, source);

                if (!edgeMap.TryGetValue(key, out var edge))
                {
                    edge = new Edge
                    {
                        HalfEdge = he,
                        Index = Edges.Count
                    };
                    edgeMap[key] = edge;
                    Edges.Add(edge);
                }

                he.Edge = edge;
            }
            
            // Set vertex halfedges (outgoing representative per vertex).
            for (int i = 0; i < HalfEdges.Count; i++)
            {
                var he = HalfEdges[i];
                var (source, _) = halfedgeEndpoints[i];
                if (Vertices[source].HalfEdge == null)
                {
                    Vertices[source].HalfEdge = he;
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
