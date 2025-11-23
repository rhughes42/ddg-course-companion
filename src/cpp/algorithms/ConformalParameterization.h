// ConformalParameterization.h
// Discrete Differential Geometry - Conformal Parameterization
// Added by Graph Technologies, 2025
// Description: Spectral and boundary-constrained conformal surface flattening

#pragma once
#include <Eigen/Dense>
#include <Eigen/Sparse>
#include "../core/Mesh.h"

namespace ddg {

class ConformalParameterization {
public:
    // Spectral conformal parameterization using eigendecomposition
    // Returns 2D coordinates (u,v) for each vertex
    static Eigen::MatrixXd spectral(const Mesh& mesh);
    
    // Boundary-constrained conformal parameterization
    // Maps boundary to unit circle, minimizes Dirichlet energy
    static Eigen::MatrixXd boundaryCircle(const Mesh& mesh);
    
    // Least squares conformal maps (LSCM)
    // Free boundary parameterization
    static Eigen::MatrixXd lscm(const Mesh& mesh, 
                                 const std::vector<int>& fixedVertices,
                                 const Eigen::MatrixXd& fixedPositions);
    
    // Compute Dirichlet energy (conformal distortion measure)
    static double dirichletEnergy(const Mesh& mesh, const Eigen::MatrixXd& uv);
    
private:
    // Helper: find and sort eigenvalues/vectors
    static void solveEigenProblem(const Eigen::SparseMatrix<double>& L,
                                   const Eigen::SparseMatrix<double>& M,
                                   Eigen::MatrixXd& eigenvectors,
                                   Eigen::VectorXd& eigenvalues,
                                   int numEigs = 3);
};

} // namespace ddg
