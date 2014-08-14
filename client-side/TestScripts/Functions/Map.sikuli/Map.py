from sikuli import *

import Utils

reload(Utils);

mapRegion = Region(204,100,1674,418);

def getMapRegionCenter():
    return mapRegion.getCenter();

def zoomIn():
    zoomIn(getMapRegionCenter());
    wait(3);

def zoomOut():
    zoomOut(getMapRegionCenter());
    wait(3);

def zoomIn(pattern = getCenter(), lines = 1, speed = Utils.wheelSpeed):
    Utils.slowWheel(pattern, WHEEL_DOWN, lines, speed);

def zoomOut(pattern = getCenter(), lines = 1, speed = Utils.wheelSpeed):
    Utils.slowWheel(pattern, WHEEL_UP, lines, speed);

def zoomToRecordingLayer(patternCheck, speedStart, pattern, lines, speed):
    wait(speedStart);
    zoomIn(pattern, lines, speed);
    exists(patternCheck, 10);
    setThrowException(True);
    find(patternCheck);

def dragBy(dx, dy, location = None):
    Utils.slowClick("Pan.png");

    if not location:
        location = mapRegion.getCenter()
    
    Utils.slowDragDrop(location, dx, dy);

def checkWaitInMap(pattern, time = Utils.maxWaitExistsTime):
    mapRegion.exists(pattern, time);
    checkInMap(pattern);

def checkInMap(pattern):
    mapRegion.setThrowException(True);
    mapRegion.find(pattern);
