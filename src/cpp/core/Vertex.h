#pragma once
#include <vector>
#include <Eigen/Dense>

class HalfEdge;
class Edge;
class Face;

class Vertex {
public:
    Eigen::Vector3d position;
    HalfEdge* halfedge;  // Outgoing halfedge
    int index;
    
    Vertex() : halfedge(nullptr), index(-1) {}
    Vertex(const Eigen::Vector3d& pos) : position(pos), halfedge(nullptr), index(-1) {}
    
    // Compute the star (1-ring neighborhood)
    std::vector<Vertex*> star() const;
    
    // Compute degree (valence)
    int degree() const;
    
    // Check if boundary vertex
    bool isBoundary() const;
    
    // Get outgoing halfedges
    std::vector<HalfEdge*> outgoingHalfEdges() const;
    
    // Get adjacent faces
    std::vector<Face*> adjacentFaces() const;
};
