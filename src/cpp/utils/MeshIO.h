// MeshIO.h
// Mesh input/output utilities
// Added by Graph Technologies, 2025
// Description: Load/save meshes in various formats (OBJ, OFF, PLY)

#pragma once
#include <string>
#include <Eigen/Dense>
#include "../core/Mesh.h"

namespace ddg {
namespace io {

class MeshIO {
public:
    // Load mesh from OBJ file
    static bool loadOBJ(const std::string& filename,
                        Eigen::MatrixXd& V,
                        Eigen::MatrixXi& F);
    
    // Save mesh to OBJ file
    static bool saveOBJ(const std::string& filename,
                        const Eigen::MatrixXd& V,
                        const Eigen::MatrixXi& F);
    
    // Load from OFF format
    static bool loadOFF(const std::string& filename,
                        Eigen::MatrixXd& V,
                        Eigen::MatrixXi& F);
    
    // Save to OFF format
    static bool saveOFF(const std::string& filename,
                        const Eigen::MatrixXd& V,
                        const Eigen::MatrixXi& F);
    
    // Load mesh directly into Mesh object
    static bool loadMesh(const std::string& filename, Mesh& mesh);
    
    // Save mesh from Mesh object
    static bool saveMesh(const std::string& filename, const Mesh& mesh);
};

} // namespace io
} // namespace ddg
