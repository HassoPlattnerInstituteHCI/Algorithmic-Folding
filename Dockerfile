FROM ubuntu:21.10

COPY . /code
RUN cd /code

RUN apt-get update && apt-get install libcgal-dev g++ git -y
RUN apt-get install cmake -y

# get OpenMesh
RUN git clone --branch OpenMesh-8.1 https://gitlab.vci.rwth-aachen.de:9000/OpenMesh/OpenMesh.git
RUN cd OpenMesh && mkdir build && cd build && cmake .. && make -j8 OpenMeshCoreStatic && cd ..

RUN g++ main.cpp  -I${HOME}/.local/include OpenMesh/build/Build/lib/libOpenMeshCore.a    
