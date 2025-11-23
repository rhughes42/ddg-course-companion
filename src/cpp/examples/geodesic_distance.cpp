// geodesic_distance.cpp
// Example: Computing geodesic distances using heat method
// Added by Graph Technologies, 2025

#include <iostream>
#include <Eigen/Dense>
#include "../core/Mesh.h"
#include "../algorithms/HeatMethod.h"

using namespace ddg;

int main() {
    std::cout << "DDG Example: Geodesic Distance (Heat Method)\n";
    std::cout << "============================================\n\n";
    
    // Create simple surface
    Eigen::MatrixXd V(5, 3);
    V << 0, 0, 0,
         1, 0, 0,
         2, 0, 0,
         0.5, 1, 0,
         1.5, 1, 0;
    
    Eigen::MatrixXi F(4, 3);
    F << 0, 1, 3,
         1, 4, 3,
         1, 2, 4,
         2, 4, 3;
    
    Mesh mesh;
    mesh.build(V, F);
    
    std::cout << "Computing geodesic distance from vertex 0..." << std::endl;
    
    // Compute distances
    Eigen::VectorXd distances = HeatMethod::compute(mesh, 0);
    
    std::cout << "\nGeodesic distances:\n";
    for (int i = 0; i < distances.size(); i++) {
        std::cout << "  d(v0, v" << i << ") = " << distances(i) << std::endl;
    }
    
    // Verify distance properties
    std::cout << "\nProperties:\n";
    std::cout << "  d(v0, v0) = " << distances(0) << " (should be 0)" << std::endl;
    std::cout << "  All distances non-negative: " 
              << (distances.minCoeff() >= 0 ? "✓" : "✗") << std::endl;
    
    // Compare to Euclidean distance
    std::cout << "\nComparison to Euclidean distance:\n";
    for (int i = 1; i < mesh.numVertices(); i++) {
        double euclidean = (mesh.vertices[i]->position - mesh.vertices[0]->position).norm();
        std::cout << "  v" << i << ": geodesic = " << distances(i) 
                  << ", Euclidean = " << euclidean 
                  << " (ratio: " << distances(i) / euclidean << ")" << std::endl;
    }
    
    return 0;
}
