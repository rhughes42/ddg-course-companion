#pragma once
#include <Eigen/Dense>

class HalfEdge;
class Vertex;

class Edge {
public:
    HalfEdge* halfedge;  // One of the two halfedges
    int index;
    
    Edge() : halfedge(nullptr), index(-1) {}
    
    // Get both vertices
    Vertex* v0() const;
    Vertex* v1() const;
    
    // Check if boundary edge
    bool isBoundary() const;
    
    // Compute edge length
    double length() const;
    
    // Compute cotan weight (for Laplacian)
    double cotan() const;
    
    // Compute edge midpoint
    Eigen::Vector3d midpoint() const;
};
