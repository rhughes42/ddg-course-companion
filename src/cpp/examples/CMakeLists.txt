# Examples CMakeLists.txt
# Added by Graph Technologies, 2025

# Laplacian smoothing example
add_executable(laplacian-smoothing laplacian_smoothing.cpp)
target_link_libraries(laplacian-smoothing PRIVATE ddg-core ddg-algorithms)

# Curvature analysis example
add_executable(curvature-analysis curvature_analysis.cpp)
target_link_libraries(curvature-analysis PRIVATE ddg-core ddg-algorithms)

# Geodesic distance example
add_executable(geodesic-distance geodesic_distance.cpp)
target_link_libraries(geodesic-distance PRIVATE ddg-core ddg-algorithms)

# Install examples
install(TARGETS laplacian-smoothing curvature-analysis geodesic-distance
        RUNTIME DESTINATION bin/examples)
