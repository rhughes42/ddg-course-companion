# CMakeLists.txt
# DDG Course Companion - C++ Build Configuration
# Added by Graph Technologies, 2025

cmake_minimum_required(VERSION 3.15)
project(DDGCourseCompanion VERSION 1.0.0 LANGUAGES CXX)

# C++ Standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Build options
option(BUILD_TESTS "Build unit tests" ON)
option(BUILD_EXAMPLES "Build example programs" ON)

# Find Eigen3
find_package(Eigen3 3.3 REQUIRED NO_MODULE)

# Core library
file(GLOB_RECURSE CORE_SOURCES "core/*.cpp")
file(GLOB_RECURSE CORE_HEADERS "core/*.h")

add_library(ddg-core STATIC ${CORE_SOURCES})
target_include_directories(ddg-core PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(ddg-core PUBLIC Eigen3::Eigen)

# Algorithms library
file(GLOB_RECURSE ALGO_SOURCES "algorithms/*.cpp")
file(GLOB_RECURSE ALGO_HEADERS "algorithms/*.h")

add_library(ddg-algorithms STATIC ${ALGO_SOURCES})
target_include_directories(ddg-algorithms PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(ddg-algorithms PUBLIC ddg-core Eigen3::Eigen)

# Utilities library
file(GLOB_RECURSE UTIL_SOURCES "utils/*.cpp")
file(GLOB_RECURSE UTIL_HEADERS "utils/*.h")

add_library(ddg-utils STATIC ${UTIL_SOURCES})
target_include_directories(ddg-utils PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
target_link_libraries(ddg-utils PUBLIC ddg-core Eigen3::Eigen)

# Main executable
add_executable(ddg-demo main.cpp)
target_link_libraries(ddg-demo PRIVATE ddg-core ddg-algorithms ddg-utils)

# Examples
if(BUILD_EXAMPLES)
    add_subdirectory(examples)
endif()

# Tests
if(BUILD_TESTS)
    enable_testing()
    add_subdirectory(tests)
endif()

# Installation
install(TARGETS ddg-core ddg-algorithms ddg-utils
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib)

install(DIRECTORY core/ algorithms/ utils/
        DESTINATION include/ddg
        FILES_MATCHING PATTERN "*.h")

# Print configuration summary
message(STATUS "")
message(STATUS "DDG Course Companion Configuration:")
message(STATUS "  Build type: ${CMAKE_BUILD_TYPE}")
message(STATUS "  C++ standard: ${CMAKE_CXX_STANDARD}")
message(STATUS "  Build tests: ${BUILD_TESTS}")
message(STATUS "  Build examples: ${BUILD_EXAMPLES}")
message(STATUS "  Eigen3 version: ${Eigen3_VERSION}")
message(STATUS "")
