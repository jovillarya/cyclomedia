from sikuli import *

import Utils
import Settings
import Cyclorama

reload(Utils);
reload(Settings);
reload(Cyclorama);

def enableSmartClick():
    Settings.tabSettings();
    Settings.enableSmartClickMeasurement();
    Settings.OK();

def disableSmartClick():
    Settings.tabSettings();
    Settings.disableSmartClickMeasurement();
    Settings.OK();

def selectFromFeatures(pattern, speed):
    Utils.slowClick(pattern);
    wait(speed);

def clickPoint(point, results):
    Utils.slowClick(point);
    wait(results, Utils.maxWaitExistsTime);
    setThrowException(True);
    find(results);

def markResults(pattern, speed):
    wait(speed);
    Utils.slowHover(pattern, True, speed);

def DetailsSelectOtherImage(pattern, checkImage, speed):
    Utils.slowClick(pattern);
    Utils.slowClick("Show.png");
    wait(speed);
    setThrowException(True);
    find(checkImage);

def deletePoint(pattern, speed):
    Utils.slowClick(pattern);
    wait(speed);

def addPoint(image, point, rotation, results, speed):
    Utils.slowClick(image);
    wait(speed);
    Cyclorama.rotateViewer(rotation, 0, 1, 1);
    clickPoint(point, results);

def finished(checkPattern):
    setThrowException(True);
    find(checkPattern);
    Utils.slowClick("CloseMeasurement.png");

def saveEdits():
    Utils.slowClick("Editor.png");
    Utils.slowClick("saveEdits.png");

def stopEditing():
    Utils.slowClick("Editor.png");
    Utils.slowClick("StopEditing.png");
