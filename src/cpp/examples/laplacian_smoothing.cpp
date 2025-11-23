// laplacian_smoothing.cpp
// Example: Mesh smoothing using mean curvature flow
// Added by Graph Technologies, 2025

#include <iostream>
#include <Eigen/Dense>
#include "../core/Mesh.h"
#include "../algorithms/MeanCurvatureFlow.h"

using namespace ddg;

int main(int argc, char** argv) {
    std::cout << "DDG Example: Laplacian Smoothing\n";
    std::cout << "================================\n\n";
    
    // Create noisy sphere approximation (icosahedron + noise)
    Eigen::MatrixXd V(12, 3);
    double phi = (1.0 + std::sqrt(5.0)) / 2.0;
    
    V << -1,  phi,  0,
          1,  phi,  0,
         -1, -phi,  0,
          1, -phi,  0,
          0, -1,  phi,
          0,  1,  phi,
          0, -1, -phi,
          0,  1, -phi,
          phi,  0, -1,
          phi,  0,  1,
         -phi,  0, -1,
         -phi,  0,  1;
    
    // Add random noise
    V += 0.1 * Eigen::MatrixXd::Random(12, 3);
    
    Eigen::MatrixXi F(20, 3);
    F << 0, 11, 5,   0, 5, 1,    0, 1, 7,    0, 7, 10,   0, 10, 11,
         1, 5, 9,    5, 11, 4,   11, 10, 2,  10, 7, 6,   7, 1, 8,
         3, 9, 4,    3, 4, 2,    3, 2, 6,    3, 6, 8,    3, 8, 9,
         4, 9, 5,    2, 4, 11,   6, 2, 10,   8, 6, 7,    9, 8, 1;
    
    // Build mesh
    Mesh mesh;
    mesh.build(V, F);
    mesh.normalize();
    
    std::cout << "Initial mesh:" << std::endl;
    std::cout << "  Vertices: " << mesh.numVertices() << std::endl;
    std::cout << "  Euler characteristic: " << mesh.eulerCharacteristic() << std::endl;
    
    // Compute initial total area
    double initialArea = 0.0;
    for (const auto& f : mesh.faces) {
        initialArea += f->area();
    }
    std::cout << "  Total area: " << initialArea << std::endl << std::endl;
    
    // Run mean curvature flow
    std::cout << "Running mean curvature flow..." << std::endl;
    double timestep = 0.001;
    int numSteps = 100;
    
    for (int i = 0; i < numSteps; i++) {
        MeanCurvatureFlow::step(mesh, timestep);
        
        if (i % 20 == 0) {
            double currentArea = 0.0;
            for (const auto& f : mesh.faces) {
                currentArea += f->area();
            }
            std::cout << "  Step " << i << ": Area = " << currentArea << std::endl;
        }
    }
    
    std::cout << "\nSmoothing complete!" << std::endl;
    std::cout << "Final mesh should be smoother (lower frequency noise removed)" << std::endl;
    
    return 0;
}
