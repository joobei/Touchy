cmake_minimum_required(VERSION 3.11)

project(Touchy)

include_directories("$ENV{OH_SDK_BASE}/include" "$ENV{OH_SDK_BASE}/utilities/include" )
link_directories("$ENV{OH_SDK_BASE}/lib/x64" "$ENV{OH_SDK_BASE}/utilities/lib/x64")

add_library(${PROJECT_NAME} SHARED ${PROJECT_NAME}.cpp)
target_link_libraries(${PROJECT_NAME} hd)

#set environment variable ${Unity_Plugins_Folder}
#and uncomment the following line
#if you want the build system to copy the library file
#to your unity Assets/Plugins folder automatically
#with every build

#add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> $ENV{Unity_Plugins_Folder}${PROJECT_NAME}.dll)
