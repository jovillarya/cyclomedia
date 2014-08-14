from sikuli import *

import Utils
import Cyclorama
import Map

reload(Utils);
reload(Cyclorama);
reload(Map);

def markRecent(speed = Utils.hoverSpeed):
    wait(speed);
    Utils.slowHover("RecentRecordings.png", True, speed);

def markHistorical(speed = Utils.hoverSpeed):
    wait(speed);
    Utils.slowHover("HistoricalRecordings.png", True, speed);

def addRecent(pattern = None):
    addLayer("RecentRecordings.png", "AddRecentRecordings.png", pattern);

def addHistorical(pattern = None):
    addLayer("HistoricalRecordings.png", "AddHistoricalRecordings.png", pattern);

def addLayer(typeLayer, typeIcon, pattern):
    setThrowException(True);

    if exists(typeLayer) == None:
        Utils.slowClick(typeIcon);
        exists(typeLayer, Utils.maxWaitExistsTime);
        find(typeLayer);

    if (pattern != None):
        exists(pattern, Utils.maxWaitExistsTime);
        find(pattern);

def openHistory():
    Utils.slowClick("OpenHistory.png");
    setThrowException(True);
    exists("HistoryForm.png", Utils.maxWaitExistsTime);
    find("HistoryForm.png");

def closeHistory():
    if exists("OpenHistory.png") != None:
        Utils.slowClick("OpenHistory.png");

def moveHistorySlider(patternCheck, steps, speed, slider, stepSize):
    setThrowException(True);
    pixels = steps * stepSize;
    Utils.slowDragDrop(slider, pixels, 0);
    exists(patternCheck, Utils.maxWaitExistsTime);
    find(patternCheck);
    wait(speed);

def switchRecentRecordings(patternMap, patternCyclorama, speed):
    Utils.slowClick(Pattern("RecentRecordingsCheckBox.png").targetOffset(-54,1));
    Map.checkWaitInMap(patternMap, Utils.maxWaitExistsTime);
    Cyclorama.checkWaitInCyclorama(patternCyclorama);
    wait(speed);

def toggleShowInCycloramaRecentRecordings(speed = Utils.clickSpeed):
    toggleShowInCyclorama("RecentRecordings.png", speed);

def toggleShowInCycloramaHistoricalRecordings():
    toggleShowInCyclorama("HistoricalRecordings.png", speed);

def toggleShowInCyclorama(pattern, speed = Utils.clickSpeed):
    Utils.slowRightClick(pattern);
    Utils.slowClick("ShowInCyclorama.png");
    wait(speed);
