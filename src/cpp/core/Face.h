#pragma once
#include <Eigen/Dense>
#include <vector>

class HalfEdge;
class Vertex;

class Face {
public:
    HalfEdge* halfedge;
    int index;
    
    Face() : halfedge(nullptr), index(-1) {}
    
    std::vector<Vertex*> vertices() const;
    std::vector<HalfEdge*> halfedges() const;
    double area() const;
    Eigen::Vector3d normal() const;
    bool isTriangle() const;
};
