// HeatMethod.cpp
// Discrete Differential Geometry - Heat Method Implementation
// Added by Graph Technologies, 2025

#include "HeatMethod.h"
#include "CotanLaplacian.h"
#include <Eigen/SparseLU>
#include <cmath>

namespace ddg {

Eigen::VectorXd HeatMethod::compute(const Mesh& mesh,
                                     const std::vector<int>& sourceVertices) {
    double timestep = computeTimestep(mesh);
    
    // Step 1: Diffuse heat from sources
    Eigen::VectorXd u = solveHeatFlow(mesh, sourceVertices, timestep);
    
    // Step 2: Compute integrated divergence
    Eigen::VectorXd div = computeIntegratedDivergence(mesh, u);
    
    // Step 3: Solve for distance
    Eigen::VectorXd phi = solveDistance(mesh, div);
    
    // Shift so distance is zero at sources
    double minDist = std::numeric_limits<double>::max();
    for (int src : sourceVertices) {
        minDist = std::min(minDist, phi(src));
    }
    phi.array() -= minDist;
    
    return phi;
}

Eigen::VectorXd HeatMethod::compute(const Mesh& mesh, int sourceVertex) {
    return compute(mesh, std::vector<int>{sourceVertex});
}

double HeatMethod::computeTimestep(const Mesh& mesh) {
    // Timestep = mean edge length squared
    double meanEdgeLength = 0.0;
    for (const auto& e : mesh.edges) {
        meanEdgeLength += e->length();
    }
    meanEdgeLength /= mesh.numEdges();
    
    return meanEdgeLength * meanEdgeLength;
}

Eigen::VectorXd HeatMethod::solveHeatFlow(const Mesh& mesh,
                                          const std::vector<int>& sources,
                                          double timestep) {
    // Solve (M - t*L)*u = δ_sources
    Eigen::SparseMatrix<double> L = CotanLaplacian::build(mesh);
    Eigen::SparseMatrix<double> M = CotanLaplacian::buildMassMatrix(mesh);
    
    Eigen::SparseMatrix<double> A = M - timestep * L;
    
    // Right-hand side: delta function at sources
    Eigen::VectorXd rhs = Eigen::VectorXd::Zero(mesh.numVertices());
    for (int src : sources) {
        rhs(src) = 1.0;
    }
    rhs = M * rhs;
    
    // Solve system
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(A);
    Eigen::VectorXd u = solver.solve(rhs);
    
    return u;
}

Eigen::VectorXd HeatMethod::computeIntegratedDivergence(const Mesh& mesh,
                                                        const Eigen::VectorXd& u) {
    Eigen::VectorXd div = Eigen::VectorXd::Zero(mesh.numVertices());
    
    // Compute gradient of u on each face, normalize, integrate divergence
    for (const auto& f : mesh.faces) {
        if (!f->isTriangle()) continue;
        
        auto verts = f->vertices();
        int i0 = verts[0]->index;
        int i1 = verts[1]->index;
        int i2 = verts[2]->index;
        
        // Compute gradient in face
        double u0 = u(i0), u1 = u(i1), u2 = u(i2);
        
        Eigen::Vector3d p0 = verts[0]->position;
        Eigen::Vector3d p1 = verts[1]->position;
        Eigen::Vector3d p2 = verts[2]->position;
        
        Eigen::Vector3d e1 = p1 - p0;
        Eigen::Vector3d e2 = p2 - p0;
        Eigen::Vector3d N = e1.cross(e2);
        double area = 0.5 * N.norm();
        N.normalize();
        
        // Gradient of u
        Eigen::Vector3d grad_u = ((u1 - u0) * e2.cross(N) + 
                                  (u2 - u0) * N.cross(e1)) / (2.0 * area);
        
        // Normalize
        double gradNorm = grad_u.norm();
        if (gradNorm > 1e-10) {
            grad_u /= gradNorm;
        }
        
        // Integrate divergence back to vertices
        // Using cotan formula
        auto halfedges = f->halfedges();
        for (size_t j = 0; j < 3; j++) {
            auto he = halfedges[j];
            Eigen::Vector3d edge = he->vector();
            double cotWeight = he->edge->cotan();
            
            div(he->twin->vertex->index) += 0.5 * cotWeight * edge.dot(grad_u);
            div(he->vertex->index) -= 0.5 * cotWeight * edge.dot(grad_u);
        }
    }
    
    return div;
}

Eigen::VectorXd HeatMethod::solveDistance(const Mesh& mesh,
                                          const Eigen::VectorXd& divergence) {
    // Solve Δφ = div
    Eigen::SparseMatrix<double> L = CotanLaplacian::build(mesh);
    
    // Neumann boundary conditions (implicit)
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(L);
    
    Eigen::VectorXd phi = solver.solve(divergence);
    
    return phi;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar0(const Mesh& mesh) {
    return CotanLaplacian::buildMassMatrix(mesh);
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar1(const Mesh& mesh) {
    Eigen::SparseMatrix<double> star1(mesh.numEdges(), mesh.numEdges());
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& e : mesh.edges) {
        // Ratio of dual edge length to primal edge length
        double weight = 1.0; // Simplified
        triplets.push_back(Eigen::Triplet<double>(e->index, e->index, weight));
    }
    
    star1.setFromTriplets(triplets.begin(), triplets.end());
    return star1;
}

Eigen::SparseMatrix<double> HodgeDecomposition::buildHodgeStar2(const Mesh& mesh) {
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
    int genus = (2 - mesh.eulerCharacteristic()) / 2;
    return Eigen::MatrixXd::Zero(mesh.numEdges(), 2 * genus);
}

std::vector<std::vector<int>> HodgeDecomposition::treeCoTree(const Mesh& mesh) {
    return std::vector<std::vector<int>>();
}

} // namespace ddg
