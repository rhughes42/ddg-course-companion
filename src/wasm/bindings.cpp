// bindings.cpp
// Emscripten bindings for DDG algorithms
// Added by Graph Technologies, 2025
// Description: Expose C++ DDG algorithms to JavaScript via WebAssembly

#include <emscripten/bind.h>
#include <emscripten/val.h>
#include <vector>
#include <string>
#include <Eigen/Dense>

#include "../cpp/core/Mesh.h"
#include "../cpp/algorithms/CotanLaplacian.h"
#include "../cpp/algorithms/MeanCurvatureFlow.h"
#include "../cpp/algorithms/DiscreteGaussianCurvature.h"
#include "../cpp/algorithms/ConformalParameterization.h"
#include "../cpp/algorithms/HeatMethod.h"
#include "../cpp/algorithms/HodgeDecomposition.h"

using namespace emscripten;
using namespace ddg;

// ============================================================================
// WASM Mesh Wrapper
// ============================================================================

class WASMMesh {
private:
    Mesh mesh;
    
public:
    WASMMesh() = default;
    
    // Build mesh from JavaScript arrays
    void buildFromArrays(const val& positions, const val& faces) {
        // Convert JS arrays to Eigen matrices
        int numVertices = positions["length"].as<int>() / 3;
        int numFaces = faces["length"].as<int>() / 3;
        
        Eigen::MatrixXd V(numVertices, 3);
        Eigen::MatrixXi F(numFaces, 3);
        
        // Fill vertex positions
        for (int i = 0; i < numVertices; i++) {
            V(i, 0) = positions[i * 3 + 0].as<double>();
            V(i, 1) = positions[i * 3 + 1].as<double>();
            V(i, 2) = positions[i * 3 + 2].as<double>();
        }
        
        // Fill face indices
        for (int i = 0; i < numFaces; i++) {
            F(i, 0) = faces[i * 3 + 0].as<int>();
            F(i, 1) = faces[i * 3 + 1].as<int>();
            F(i, 2) = faces[i * 3 + 2].as<int>();
        }
        
        mesh.build(V, F);
    }
    
    // Get mesh statistics
    int numVertices() const { return mesh.numVertices(); }
    int numEdges() const { return mesh.numEdges(); }
    int numFaces() const { return mesh.numFaces(); }
    int eulerCharacteristic() const { return mesh.eulerCharacteristic(); }
    
    // Get positions as flat array
    val getPositions() const {
        Eigen::MatrixXd V = mesh.vertexPositions();
        val result = val::array();
        
        for (int i = 0; i < V.rows(); i++) {
            result.call<void>("push", V(i, 0));
            result.call<void>("push", V(i, 1));
            result.call<void>("push", V(i, 2));
        }
        
        return result;
    }
    
    // Normalize mesh
    void normalize() {
        mesh.normalize();
    }
    
    void center() {
        mesh.center();
    }
    
    // ========================================================================
    // Algorithm wrappers
    // ========================================================================
    
    // Mean curvature flow
    void meanCurvatureFlowStep(double timestep) {
        MeanCurvatureFlow::step(mesh, timestep);
    }
    
    void meanCurvatureFlow(double timestep, int numSteps) {
        MeanCurvatureFlow::flow(mesh, timestep, numSteps);
    }
    
    // Gaussian curvature
    val computeGaussianCurvature() const {
        Eigen::VectorXd K = DiscreteGaussianCurvature::compute(mesh);
        val result = val::array();
        
        for (int i = 0; i < K.size(); i++) {
            result.call<void>("push", K(i));
        }
        
        return result;
    }
    
    double totalGaussianCurvature() const {
        return DiscreteGaussianCurvature::totalCurvature(mesh);
    }
    
    double gaussBonnetError() const {
        return DiscreteGaussianCurvature::gaussBonnetError(mesh);
    }
    
    // Conformal parameterization
    val spectralConformalParameterization() const {
        Eigen::MatrixXd uv = ConformalParameterization::spectral(mesh);
        val result = val::array();
        
        for (int i = 0; i < uv.rows(); i++) {
            result.call<void>("push", uv(i, 0));
            result.call<void>("push", uv(i, 1));
        }
        
        return result;
    }
    
    val boundaryCircleParameterization() const {
        Eigen::MatrixXd uv = ConformalParameterization::boundaryCircle(mesh);
        val result = val::array();
        
        for (int i = 0; i < uv.rows(); i++) {
            result.call<void>("push", uv(i, 0));
            result.call<void>("push", uv(i, 1));
        }
        
        return result;
    }
    
    double dirichletEnergy(const val& uvArray) const {
        // Convert JS array to Eigen matrix
        int numVertices = uvArray["length"].as<int>() / 2;
        Eigen::MatrixXd uv(numVertices, 2);
        
        for (int i = 0; i < numVertices; i++) {
            uv(i, 0) = uvArray[i * 2 + 0].as<double>();
            uv(i, 1) = uvArray[i * 2 + 1].as<double>();
        }
        
        return ConformalParameterization::dirichletEnergy(mesh, uv);
    }
    
    // Heat method (geodesic distance)
    val computeGeodesicDistance(int sourceVertex) const {
        Eigen::VectorXd dist = HeatMethod::compute(mesh, sourceVertex);
        val result = val::array();
        
        for (int i = 0; i < dist.size(); i++) {
            result.call<void>("push", dist(i));
        }
        
        return result;
    }
    
    val computeGeodesicDistanceMultiple(const val& sources) const {
        // Convert JS array to vector
        std::vector<int> sourceVerts;
        int numSources = sources["length"].as<int>();
        
        for (int i = 0; i < numSources; i++) {
            sourceVerts.push_back(sources[i].as<int>());
        }
        
        Eigen::VectorXd dist = HeatMethod::compute(mesh, sourceVerts);
        val result = val::array();
        
        for (int i = 0; i < dist.size(); i++) {
            result.call<void>("push", dist(i));
        }
        
        return result;
    }
    
    // Hodge decomposition
    val hodgeDecomposition(const val& omegaArray) const {
        // Convert JS array to Eigen vector
        int numEdges = omegaArray["length"].as<int>();
        Eigen::VectorXd omega(numEdges);
        
        for (int i = 0; i < numEdges; i++) {
            omega(i) = omegaArray[i].as<double>();
        }
        
        VectorFieldComponents comp = HodgeDecomposition::decompose(mesh, omega);
        
        // Return as JS object
        val result = val::object();
        
        val exact = val::array();
        for (int i = 0; i < comp.exact.size(); i++) {
            exact.call<void>("push", comp.exact(i));
        }
        result.set("exact", exact);
        
        val coexact = val::array();
        for (int i = 0; i < comp.coexact.size(); i++) {
            coexact.call<void>("push", comp.coexact(i));
        }
        result.set("coexact", coexact);
        
        val harmonic = val::array();
        for (int i = 0; i < comp.harmonic.size(); i++) {
            harmonic.call<void>("push", comp.harmonic(i));
        }
        result.set("harmonic", harmonic);
        
        return result;
    }
};

// ============================================================================
// Emscripten Bindings
// ============================================================================

EMSCRIPTEN_BINDINGS(ddg_module) {
    // Register WASMMesh class
    class_<WASMMesh>("Mesh")
        .constructor<>()
        .function("buildFromArrays", &WASMMesh::buildFromArrays)
        .function("numVertices", &WASMMesh::numVertices)
        .function("numEdges", &WASMMesh::numEdges)
        .function("numFaces", &WASMMesh::numFaces)
        .function("eulerCharacteristic", &WASMMesh::eulerCharacteristic)
        .function("getPositions", &WASMMesh::getPositions)
        .function("normalize", &WASMMesh::normalize)
        .function("center", &WASMMesh::center)
        .function("meanCurvatureFlowStep", &WASMMesh::meanCurvatureFlowStep)
        .function("meanCurvatureFlow", &WASMMesh::meanCurvatureFlow)
        .function("computeGaussianCurvature", &WASMMesh::computeGaussianCurvature)
        .function("totalGaussianCurvature", &WASMMesh::totalGaussianCurvature)
        .function("gaussBonnetError", &WASMMesh::gaussBonnetError)
        .function("spectralConformalParameterization", &WASMMesh::spectralConformalParameterization)
        .function("boundaryCircleParameterization", &WASMMesh::boundaryCircleParameterization)
        .function("dirichletEnergy", &WASMMesh::dirichletEnergy)
        .function("computeGeodesicDistance", &WASMMesh::computeGeodesicDistance)
        .function("computeGeodesicDistanceMultiple", &WASMMesh::computeGeodesicDistanceMultiple)
        .function("hodgeDecomposition", &WASMMesh::hodgeDecomposition);
}
