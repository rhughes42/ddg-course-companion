// HodgeDecomposition.cpp
// Discrete Differential Geometry - Hodge Decomposition Implementation
// Added by Graph Technologies, 2025

#include "HodgeDecomposition.h"
#include <Eigen/SparseLU>
#include <Eigen/SparseQR>

namespace ddg {

VectorFieldComponents HodgeDecomposition::decompose(const Mesh& mesh,
                                                     const Eigen::VectorXd& omega) {
    VectorFieldComponents result;
    
    // Build operators
    Eigen::SparseMatrix<double> d0 = buildD0(mesh);
    Eigen::SparseMatrix<double> d1 = buildD1(mesh);
    Eigen::SparseMatrix<double> star1 = buildHodgeStar1(mesh);
    
    // Codifferential: δ = ⋆d⋆
    Eigen::SparseMatrix<double> delta1 = star1 * d1 * star1;
    
    // Solve for exact component: d0*α = ω (least squares)
    Eigen::SparseQR<Eigen::SparseMatrix<double>, Eigen::COLAMDOrdering<int>> qr;
    qr.compute(d0);
    Eigen::VectorXd alpha = qr.solve(omega);
    result.exact = d0 * alpha;
    
    // Solve for coexact component: δ1*β = ω - exact
    Eigen::VectorXd residual = omega - result.exact;
    qr.compute(delta1);
    Eigen::VectorXd beta = qr.solve(residual);
    result.coexact = delta1 * beta;
    
    // Harmonic component: what remains
    result.harmonic = omega - result.exact - result.coexact;
    
    return result;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildD0(const Mesh& mesh) {
    int nVerts = mesh.numVertices();
    int nEdges = mesh.numEdges();
    
    Eigen::SparseMatrix<double> d0(nEdges, nVerts);
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& e : mesh.edges) {
        int i = e->index;
        int v0 = e->halfedge->twin->vertex->index;
        int v1 = e->halfedge->vertex->index;
        
        triplets.push_back(Eigen::Triplet<double>(i, v0, -1.0));
        triplets.push_back(Eigen::Triplet<double>(i, v1, 1.0));
    }
    
    d0.setFromTriplets(triplets.begin(), triplets.end());
    return d0;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildD1(const Mesh& mesh) {
    int nEdges = mesh.numEdges();
    int nFaces = mesh.numFaces();
    
    Eigen::SparseMatrix<double> d1(nFaces, nEdges);
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& f : mesh.faces) {
        if (!f->isTriangle()) continue;
        
        int fIdx = f->index;
        auto halfedges = f->halfedges();
        
        for (auto he : halfedges) {
            int eIdx = he->edge->index;
            // Sign depends on orientation relative to face
            double sign = (he->edge->halfedge == he) ? 1.0 : -1.0;
            triplets.push_back(Eigen::Triplet<double>(fIdx, eIdx, sign));
        }
    }
    
    d1.setFromTriplets(triplets.begin(), triplets.end());
    return d1;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar0(const Mesh& mesh) {
    // Hodge star for 0-forms (vertex-based)
    Eigen::VectorXd areas = CotanLaplacian::computeVertexAreas(mesh);
    
    Eigen::SparseMatrix<double> star0(mesh.numVertices(), mesh.numVertices());
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (int i = 0; i < areas.size(); i++) {
        triplets.push_back(Eigen::Triplet<double>(i, i, areas(i)));
    }
    
    star0.setFromTriplets(triplets.begin(), triplets.end());
    return star0;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar1(const Mesh& mesh) {
    // Hodge star for 1-forms (edge-based)
    Eigen::SparseMatrix<double> star1(mesh.numEdges(), mesh.numEdges());
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& e : mesh.edges) {
        // Dual edge length / primal edge length
        double weight = 1.0; // Simplified - full version uses edge ratios
        triplets.push_back(Eigen::Triplet<double>(e->index, e->index, weight));
    }
    
    star1.setFromTriplets(triplets.begin(), triplets.end());
    return star1;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar2(const Mesh& mesh) {
    // Hodge star for 2-forms (face-based)
    Eigen::SparseMatrix<double> star2(mesh.numFaces(), mesh.numFaces());
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& f : mesh.faces) {
        double area = f->area();
        triplets.push_back(Eigen::Triplet<double>(f->index, f->index, 1.0 / area));
    }
    
    star2.setFromTriplets(triplets.begin(), triplets.end());
    return star2;
}

Eigen::MatrixXd HodgeDecomposition::harmonicBases(const Mesh& mesh) {
    // Compute harmonic 1-form bases
    // Dimension = 2*genus (first Betti number)
    
    int genus = (2 - mesh.eulerCharacteristic()) / 2;
    int dim = 2 * genus;
    
    if (dim == 0) {
        return Eigen::MatrixXd::Zero(mesh.numEdges(), 0);
    }
    
    // Use tree-cotree to find generators
    auto generators = treeCoTree(mesh);
    
    // Build harmonic bases from generators
    Eigen::MatrixXd bases(mesh.numEdges(), dim);
    // ... implementation details ...
    
    return bases;
}

std::vector<std::vector<int>> HodgeDecomposition::treeCoTree(const Mesh& mesh) {
    // Tree-cotree algorithm for finding homology generators
    // Returns list of edge loops representing generators
    
    std::vector<std::vector<int>> generators;
    
    // Build spanning tree
    // Build dual spanning tree  
    // Remaining edges form generators
    
    // Placeholder - full implementation requires graph algorithms
    
    return generators;
}

} // namespace ddg
