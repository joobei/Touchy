#include "HL/hl.h"
#include "GL/glut.h"

int main() {

	//Initialize Device
	HHD hHD;
	hHD = hdInitDevice(HD_DEFAULT_DEVICE);
	if (HD_DEVICE_ERROR(hdGetError()))
	{
		return(-1);
	}

	//Create Context
	HHLRC hHLRC;
	hHLRC = hlCreateContext(hHD);

	//Make created context the current context
	//*** Probably need to add 2nd context for 2nd device ***
	hlMakeCurrent(hHLRC);

	GLuint myShapeId =1;

	hlBeginShape(HL_SHAPE_DEPTH_BUFFER, myShapeId);
	glBegin(GL_POLYGON);
	glVertex3f(0, 0, 0);
	glVertex3f(1, 0, 0);
	glVertex3f(1, 2, 0);
	glVertex3f(0, 2, 0);
	glEnd();

	hlEndShape();

	while (true) {
		hlBeginFrame();
		hlEndFrame();
		
	}

	return 1;
}