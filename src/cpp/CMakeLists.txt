cmake_minimum_required(VERSION 3.15)
project(DDGCompanion VERSION 1.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find Eigen3
find_package(Eigen3 3.3 REQUIRED NO_MODULE)

# Include directories
include_directories(
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${EIGEN3_INCLUDE_DIR}
)

# Core library
add_library(ddg-core
    core/Vertex.cpp
    core/Edge.cpp
    core/Face.cpp
    core/HalfEdge.cpp
    core/Mesh.cpp
    core/SimplicialComplex.cpp
)

target_link_libraries(ddg-core Eigen3::Eigen)

# Algorithms library
add_library(ddg-algorithms
    algorithms/CotanLaplacian.cpp
    algorithms/MeanCurvatureFlow.cpp
    algorithms/DiscreteGaussianCurvature.cpp
    algorithms/ConformalParameterization.cpp
    algorithms/HodgeDecomposition.cpp
    algorithms/HeatMethod.cpp
)

target_link_libraries(ddg-algorithms ddg-core Eigen3::Eigen)

# Demo executable
add_executable(ddg-demo
    main.cpp
)

target_link_libraries(ddg-demo ddg-core ddg-algorithms)

# Tests
enable_testing()
add_subdirectory(tests)
