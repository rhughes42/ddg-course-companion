#pragma once
#include "../core/Mesh.h"

class MeanCurvatureFlow {
public:
    // Perform one step of implicit mean curvature flow
    // timestep: integration timestep
    static void step(Mesh& mesh, double timestep);
    
    // Run multiple steps
    static void flow(Mesh& mesh, double timestep, int numSteps);
};
