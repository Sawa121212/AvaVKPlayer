cmake_minimum_required(VERSION 3.5)

project(webelement LANGUAGES CXX)
set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CXX_STANDARD 17)
set(CXX_STANDARD_REQUIRED ON)

# set(QM_PATH ${CMAKE_SOURCE_DIR}/src/qmultimedia)

find_package(Qt5 REQUIRED COMPONENTS Widgets)
find_package(Qt5 REQUIRED COMPONENTS Network)
find_package(Qt5 REQUIRED COMPONENTS WebEngineWidgets)

file(GLOB SOURCES ${CMAKE_SOURCE_DIR}/src/*.cpp) #${VA_PATH}/*

add_executable(webelement ${SOURCES})
target_link_libraries(webelement PRIVATE Qt5::Widgets Qt5::Core 
  Qt5::WebEngineWidgets Qt5::Network pthread)