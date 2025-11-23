#include "HalfEdge.h"
#include "Vertex.h"
#include <Eigen/Dense>

Vertex* HalfEdge::source() const {
    return twin->vertex;
}

Eigen::Vector3d HalfEdge::vector() const {
    return vertex->position - twin->vertex->position;
}
