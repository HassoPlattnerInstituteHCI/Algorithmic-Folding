cmake_minimum_required(VERSION 3.10)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_BUILD_TYPE Debug)

find_package(CGAL)
find_package(OpenMesh PATHS OpenMesh/build)

add_executable(main main.cpp)
target_link_libraries(main CGAL::CGAL)
target_link_libraries (main
  OpenMeshCore
)
