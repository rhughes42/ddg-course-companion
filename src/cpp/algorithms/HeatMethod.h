// HeatMethod.h
// Discrete Differential Geometry - Heat Method for Geodesic Distance
// Added by Graph Technologies, 2025
// Description: Fast computation of geodesic distances using short-time heat diffusion

#pragma once
#include <Eigen/Dense>
#include <Eigen/Sparse>
#include "../core/Mesh.h"

namespace ddg {

class HeatMethod {
public:
    // Compute geodesic distance from source vertices
    static Eigen::VectorXd compute(const Mesh& mesh,
                                    const std::vector<int>& sourceVertices);
    
    // Compute geodesic distance from single source
    static Eigen::VectorXd compute(const Mesh& mesh, int sourceVertex);
    
    // Set timestep parameter (default: mean edge length squared)
    static double computeTimestep(const Mesh& mesh);
    
private:
    // Step 1: Solve heat equation
    static Eigen::VectorXd solveHeatFlow(const Mesh& mesh,
                                         const std::vector<int>& sources,
                                         double timestep);
    
    // Step 2: Compute normalized gradient
    static Eigen::VectorXd computeIntegratedDivergence(const Mesh& mesh,
                                                        const Eigen::VectorXd& u);
    
    // Step 3: Solve Poisson equation
    static Eigen::VectorXd solveDistance(const Mesh& mesh,
                                         const Eigen::VectorXd& divergence);
};

} // namespace ddg
