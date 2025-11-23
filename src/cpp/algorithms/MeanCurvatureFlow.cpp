#include "MeanCurvatureFlow.h"
#include "CotanLaplacian.h"
#include <Eigen/SparseLU>

void MeanCurvatureFlow::step(Mesh& mesh, double timestep) {
    auto L = CotanLaplacian::build(mesh);
    auto M = CotanLaplacian::buildMassMatrix(mesh);
    
    // System: (M - t*L) * X_new = M * X_old
    Eigen::SparseMatrix<double> A = M - timestep * L;
    Eigen::MatrixXd X = mesh.vertexPositions();
    Eigen::MatrixXd b = M * X;
    
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(A);
    Eigen::MatrixXd X_new = solver.solve(b);
    
    mesh.setVertexPositions(X_new);
}

void MeanCurvatureFlow::flow(Mesh& mesh, double timestep, int numSteps) {
    for (int i = 0; i < numSteps; i++) {
        step(mesh, timestep);
    }
}
