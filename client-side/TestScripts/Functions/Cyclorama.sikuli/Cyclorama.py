from sikuli import *

import Utils

reload(Utils);

viewerRegion = Region(203,535,1695,488);

def rotate( dx, dy, location = None ):
    if location :
        rotationLocation = location;
    else:
        rotationLocation = viewerRegion.getCenter();
    
    Utils.slowDragDrop(rotationLocation, dx, dy);

def rotateViewer(dx, dy, viewerNr, viewerCount):
    rotationLocation = getViewerCenter(viewerNr, viewerCount);
    rotate(dx, dy, rotationLocation);

def zoomIn(steps, location = None):
    if location :
        zoomLocation = location;
    else:
        zoomLocation = viewerRegion.getCenter();

    Utils.slowWheel(zoomLocation, WHEEL_UP, steps, 0);

def zoomOut(steps, location = None):
    if location :
        zoomLocation = location;
    else:
        zoomLocation = viewerRegion.getCenter();

    Utils.slowWheel(zoomLocation, WHEEL_DOWN, steps, 0);

def zoomInViewer(steps, viewerNr, viewerCount):
    zoomLocation = getViewerCenter(viewerNr, viewerCount);
    zoomIn(steps, zoomLocation);

def zoomOutViewer(steps, viewerNr, viewerCount):
    zoomLocation = getViewerCenter(viewerNr, viewerCount);
    zoomOut(steps, zoomLocation);

def getViewerRegionCenter():
    return viewerRegion.getCenter();

def getViewerCenter(viewerNr, viewerCount):
    centerLocation = viewerRegion.getTopLeft();
    viewerHeight = viewerRegion.h;
    viewerWidth = int(viewerRegion.w / viewerCount);

    centerLocation.x += viewerWidth * (viewerNr - 1) + int(viewerWidth * 0.5);
    centerLocation.y += int(viewerHeight * 0.5);

    return centerLocation;

def selectCycloramaTool():
    Utils.slowClick("OpenLocationTool.png");

def openCyclorama(pattern, loadSpeed, clickSpeed = Utils.clickSpeed):
    Utils.slowClick(pattern, clickSpeed);
    wait(loadSpeed);

def checkWaitInCyclorama(pattern, time = Utils.maxWaitExistsTime):
    viewerRegion.exists(pattern, time);
    checkInCyclorama(pattern);

def checkInCyclorama(pattern):
    viewerRegion.setThrowException(True);
    viewerRegion.find(pattern);

def checkNotInCyclorama(pattern):
    if viewerRegion.exists(pattern):
        exit(-1);
