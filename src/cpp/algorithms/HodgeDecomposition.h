// HodgeDecomposition.h
// Discrete Differential Geometry - Hodge Decomposition for Vector Fields
// Added by Graph Technologies, 2025
// Description: Decompose discrete 1-forms into exact, coexact, and harmonic components

#pragma once
#include <Eigen/Dense>
#include <Eigen/Sparse>
#include "../core/Mesh.h"

namespace ddg {

struct VectorFieldComponents {
    Eigen::VectorXd exact;      // dα component
    Eigen::VectorXd coexact;    // δβ component  
    Eigen::VectorXd harmonic;   // γ component
};

class HodgeDecomposition {
public:
    // Decompose 1-form ω into ω = dα + δβ + γ
    static VectorFieldComponents decompose(const Mesh& mesh, 
                                           const Eigen::VectorXd& omega);
    
    // Build exterior derivative d0: 0-forms → 1-forms
    static Eigen::SparseMatrix<double> buildD0(const Mesh& mesh);
    
    // Build exterior derivative d1: 1-forms → 2-forms
    static Eigen::SparseMatrix<double> buildD1(const Mesh& mesh);
    
    // Build Hodge star operators
    static Eigen::SparseMatrix<double> buildHodgeStar0(const Mesh& mesh);
    static Eigen::SparseMatrix<double> buildHodgeStar1(const Mesh& mesh);
    static Eigen::SparseMatrix<double> buildHodgeStar2(const Mesh& mesh);
    
    // Compute harmonic bases (dimension = 2*genus)
    static Eigen::MatrixXd harmonicBases(const Mesh& mesh);
    
    // Tree-cotree algorithm for homology generators
    static std::vector<std::vector<int>> treeCoTree(const Mesh& mesh);
};

} // namespace ddg
