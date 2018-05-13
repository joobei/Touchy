cmake_minimum_required(VERSION 3.11)

project(HapticBridge)

include_directories("$ENV{OH_SDK_BASE}/include" "$ENV{OH_SDK_BASE}/utilities/include" )
link_directories("$ENV{OH_SDK_BASE}/lib/x64" "$ENV{OH_SDK_BASE}/utilities/lib/x64/")

add_executable(${PROJECT_NAME} hapticbridge.cpp)
target_link_libraries(${PROJECT_NAME} hl hd)