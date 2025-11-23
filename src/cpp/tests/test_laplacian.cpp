// test_laplacian.cpp
// Unit tests for Laplacian operator
// Added by Graph Technologies, 2025

#include <catch2/catch_test_macros.hpp>
#include <catch2/matchers/catch_matchers_floating_point.hpp>
#include "../core/Mesh.h"
#include "../algorithms/CotanLaplacian.h"
#include <Eigen/Dense>

using namespace ddg;

TEST_CASE("Cotan Laplacian properties", "[laplacian]") {
    // Build simple mesh
    Eigen::MatrixXd V(4, 3);
    V << 0, 0, 0,
         1, 0, 0,
         0.5, 0.866, 0,
         0.5, 0.289, 0.816;
    
    Eigen::MatrixXi F(4, 3);
    F << 0, 1, 2,
         0, 1, 3,
         0, 2, 3,
         1, 2, 3;
    
    Mesh mesh;
    mesh.build(V, F);
    
    auto L = CotanLaplacian::build(mesh);
    
    SECTION("Matrix dimensions") {
        REQUIRE(L.rows() == mesh.numVertices());
        REQUIRE(L.cols() == mesh.numVertices());
    }
    
    SECTION("Zero row sum") {
        // Each row should sum to approximately zero
        for (int i = 0; i < L.rows(); i++) {
            double rowSum = 0.0;
            for (Eigen::SparseMatrix<double>::InnerIterator it(L, i); it; ++it) {
                rowSum += it.value();
            }
            REQUIRE_THAT(rowSum, Catch::Matchers::WithinAbs(0.0, 1e-10));
        }
    }
    
    SECTION("Symmetry") {
        // Laplacian should be symmetric
        for (int k = 0; k < L.outerSize(); ++k) {
            for (Eigen::SparseMatrix<double>::InnerIterator it(L, k); it; ++it) {
                double val = it.value();
                double transpose_val = L.coeff(it.col(), it.row());
                REQUIRE_THAT(val, Catch::Matchers::WithinAbs(transpose_val, 1e-10));
            }
        }
    }
}

TEST_CASE("Mass matrix", "[laplacian][mass]") {
    Eigen::MatrixXd V(3, 3);
    V << 0, 0, 0,
         1, 0, 0,
         0, 1, 0;
    
    Eigen::MatrixXi F(1, 3);
    F << 0, 1, 2;
    
    Mesh mesh;
    mesh.build(V, F);
    
    auto M = CotanLaplacian::buildMassMatrix(mesh);
    
    SECTION("Diagonal matrix") {
        for (int k = 0; k < M.outerSize(); ++k) {
            for (Eigen::SparseMatrix<double>::InnerIterator it(M, k); it; ++it) {
                if (it.row() != it.col()) {
                    REQUIRE_THAT(it.value(), Catch::Matchers::WithinAbs(0.0, 1e-10));
                }
            }
        }
    }
    
    SECTION("Positive entries") {
        for (int i = 0; i < M.rows(); i++) {
            REQUIRE(M.coeff(i, i) > 0.0);
        }
    }
}

TEST_CASE("Poisson equation", "[laplacian][poisson]") {
    // Test on simple mesh with known solution
    Eigen::MatrixXd V(4, 3);
    V << 0, 0, 0,
         1, 0, 0,
         0.5, 0.866, 0,
         0.5, 0.289, 0.816;
    
    Eigen::MatrixXi F(4, 3);
    F << 0, 1, 2,
         0, 1, 3,
         0, 2, 3,
         1, 2, 3;
    
    Mesh mesh;
    mesh.build(V, F);
    
    // Set up test function
    Eigen::VectorXd f = Eigen::VectorXd::Random(mesh.numVertices());
    
    // Solve Poisson equation
    Eigen::MatrixXd u = CotanLaplacian::solvePoisson(mesh, f);
    
    SECTION("Solution dimension") {
        REQUIRE(u.rows() == mesh.numVertices());
    }
}
