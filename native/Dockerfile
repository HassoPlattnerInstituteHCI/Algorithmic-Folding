FROM ubuntu:21.10

RUN apt-get update && apt-get install libcgal-dev g++ git -y
RUN apt-get install cmake -y

# get OpenMesh
RUN mkdir /code
WORKDIR /code
RUN git clone --branch OpenMesh-8.1 https://gitlab.vci.rwth-aachen.de:9000/OpenMesh/OpenMesh.git &&\
	cd OpenMesh && mkdir build && cd build && cmake .. && make -j8 OpenMeshCore

COPY main.cpp /code/
COPY CMakeLists.txt /code/

RUN mkdir build && cd build && cmake .. && make

WORKDIR ./build
