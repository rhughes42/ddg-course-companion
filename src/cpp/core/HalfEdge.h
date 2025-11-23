#pragma once

class Vertex;
class Edge;
class Face;

class HalfEdge {
public:
    Vertex* vertex;    // Target vertex
    Edge* edge;        // Associated edge
    Face* face;        // Left face (can be null for boundary)
    
    HalfEdge* next;    // Next halfedge in face
    HalfEdge* twin;    // Opposite halfedge
    
    int index;
    
    HalfEdge() : vertex(nullptr), edge(nullptr), face(nullptr),
                 next(nullptr), twin(nullptr), index(-1) {}
    
    // Get the source vertex (vertex of previous halfedge)
    Vertex* source() const;
    
    // Get the target vertex
    Vertex* target() const { return vertex; }
    
    // Check if boundary halfedge
    bool isBoundary() const { return face == nullptr; }
    
    // Get vector along the edge
    Eigen::Vector3d vector() const;
};
