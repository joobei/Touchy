cmake_minimum_required(VERSION 3.11)

project(Tachi)

include_directories("$ENV{OH_SDK_BASE}/include" "$ENV{OH_SDK_BASE}/utilities/include" )
link_directories("$ENV{OH_SDK_BASE}/lib/x64" "$ENV{OH_SDK_BASE}/utilities/lib/x64")

add_library(${PROJECT_NAME} SHARED tachi.cpp)
target_link_libraries(${PROJECT_NAME} hd)

set(Unity_Plugin_folder C:/Users/jubei/Desktop/HapticExperiment/Assets/Plugins/)

add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ${Unity_Plugin_folder}Tachi.dll)
