#include "CotanLaplacian.h"
#include <Eigen/SparseLU>
#include <cmath>

Eigen::SparseMatrix<double> CotanLaplacian::build(const Mesh& mesh) {
    int n = mesh.numVertices();
    Eigen::SparseMatrix<double> L(n, n);
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& v : mesh.vertices) {
        int i = v->index;
        double sumW = 0;
        HalfEdge* he = v->halfedge;
        do {
            int j = he->next->vertex->index;
            double w = 0;
            if (he->face) {
                auto e0 = he->vector();
                auto e1 = he->next->vector();
                w += 0.5 * (-e0.dot(e1)) / e0.cross(e1).norm();
            }
            if (he->twin->face) {
                auto e0 = he->twin->vector();
                auto e1 = he->twin->next->vector();
                w += 0.5 * (-e0.dot(e1)) / e0.cross(e1).norm();
            }
            triplets.push_back({i, j, w});
            sumW += w;
            he = he->twin->next;
        } while (he != v->halfedge);
        triplets.push_back({i, i, -sumW});
    }
    L.setFromTriplets(triplets.begin(), triplets.end());
    return L;
}

Eigen::SparseMatrix<double> CotanLaplacian::buildMassMatrix(const Mesh& mesh) {
    auto areas = computeVertexAreas(mesh);
    Eigen::SparseMatrix<double> M(mesh.numVertices(), mesh.numVertices());
    std::vector<Eigen::Triplet<double>> triplets;
    for (int i = 0; i < areas.size(); i++)
        triplets.push_back({i, i, areas(i)});
    M.setFromTriplets(triplets.begin(), triplets.end());
    return M;
}

Eigen::VectorXd CotanLaplacian::computeVertexAreas(const Mesh& mesh) {
    Eigen::VectorXd areas = Eigen::VectorXd::Zero(mesh.numVertices());
    for (const auto& f : mesh.faces) {
        double A = f->area() / 3.0;
        for (auto v : f->vertices())
            areas(v->index) += A;
    }
    return areas;
}

Eigen::MatrixXd CotanLaplacian::solvePoisson(const Mesh& mesh, const Eigen::MatrixXd& F) {
    auto L = build(mesh);
    auto M = buildMassMatrix(mesh);
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(L);
    return solver.solve(M * F);
}
