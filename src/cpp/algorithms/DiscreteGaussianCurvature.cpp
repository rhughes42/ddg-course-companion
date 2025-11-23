#include "DiscreteGaussianCurvature.h"
#include <cmath>

Eigen::VectorXd DiscreteGaussianCurvature::compute(const Mesh& mesh) {
    Eigen::VectorXd K = Eigen::VectorXd::Zero(mesh.numVertices());
    
    for (const auto& v : mesh.vertices) {
        if (v->isBoundary()) continue;
        
        double angleSum = 0.0;
        HalfEdge* he = v->halfedge;
        do {
            if (he->face) {
                // Compute angle at this vertex in the face
                Eigen::Vector3d e1 = -he->twin->vector();
                Eigen::Vector3d e2 = he->next->vector();
                e1.normalize();
                e2.normalize();
                double angle = std::acos(std::max(-1.0, std::min(1.0, e1.dot(e2))));
                angleSum += angle;
            }
            he = he->twin->next;
        } while (he != v->halfedge);
        
        // Angle defect: K = 2π - sum of angles
        K(v->index) = 2.0 * M_PI - angleSum;
    }
    
    return K;
}

double DiscreteGaussianCurvature::totalCurvature(const Mesh& mesh) {
    return compute(mesh).sum();
}
