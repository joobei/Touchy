cmake_minimum_required(VERSION 2.8.10)

project(Touchy)

include_directories("$ENV{OH_SDK_BASE}/include" "$ENV{OH_SDK_BASE}/utilities/include" )
link_directories(
"$ENV{OH_SDK_BASE}/lib/x64/Release"
"$ENV{OH_SDK_BASE}/utilities/lib/x64/Release"
)

add_library(${PROJECT_NAME} SHARED ${PROJECT_NAME}.cpp)
target_link_libraries(${PROJECT_NAME} hd)


#if you want the build system to copy the library file
#to your unity Assets/Plugins folder automatically
#after every build set environment variable ${Unity_Plugins_Folder}
#and uncomment the add_custom_command line
#be aware that in order for unity to see the new plugin you must restart 
#the unity editor. Simply stopping and playing in the editor will not work. 
#you need to exit the editor in order for the new plugin to be used

#add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> $ENV{Unity_Plugins_Folder}\\${PROJECT_NAME}.dll)
