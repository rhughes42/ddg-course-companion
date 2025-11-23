#include "Edge.h"
#include "HalfEdge.h"
#include "Vertex.h"
#include "Face.h"
#include <cmath>

Vertex* Edge::v0() const {
    return halfedge->vertex;
}

Vertex* Edge::v1() const {
    return halfedge->twin->vertex;
}

bool Edge::isBoundary() const {
    return !halfedge->face || !halfedge->twin->face;
}

double Edge::length() const {
    Eigen::Vector3d v = halfedge->vector();
    return v.norm();
}

double Edge::cotan() const {
    // Sum cotangents from both adjacent triangles
    double cotSum = 0.0;
    
    // Cotan from first triangle
    if (halfedge->face) {
        Eigen::Vector3d e1 = halfedge->vector();
        Eigen::Vector3d e2 = halfedge->next->vector();
        double cosAngle = e1.dot(e2) / (e1.norm() * e2.norm());
        double sinAngle = e1.cross(e2).norm() / (e1.norm() * e2.norm());
        cotSum += cosAngle / sinAngle;
    }
    
    // Cotan from second triangle
    if (halfedge->twin->face) {
        Eigen::Vector3d e1 = halfedge->twin->vector();
        Eigen::Vector3d e2 = halfedge->twin->next->vector();
        double cosAngle = e1.dot(e2) / (e1.norm() * e2.norm());
        double sinAngle = e1.cross(e2).norm() / (e1.norm() * e2.norm());
        cotSum += cosAngle / sinAngle;
    }
    
    return cotSum / 2.0;
}

Eigen::Vector3d Edge::midpoint() const {
    return 0.5 * (halfedge->vertex->position + halfedge->twin->vertex->position);
}
