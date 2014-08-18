from sikuli import *

import Utils

reload(Utils);

username = "globespotter@cyclomedia.com";
password = "tyjavy";

def delete():
   settingsPath = "%USERPROFILE%\\AppData\\Roaming\\IntegrationArcMap";
   Utils.deleteDirectory(settingsPath);

def login(usernameUse = username, passwordUse = password):
    open();

    if exists("LoginSuccessFully.png") == None:
        Utils.slowClick(Pattern("EnterUsername.png").targetOffset(5,0));
        Utils.slowType(usernameUse);
        Utils.slowClick(Pattern("EnterPassword.png").targetOffset(13,1));
        Utils.slowType(passwordUse);
        apply();
        exists("LoginSuccessFully.png", Utils.maxWaitExistsTime);
        OK();
    else:
        OK();

def open():
    Utils.slowClick("OpenSettings.png");
    exists("GlobeSpotterForArcGISDesktop.png", Utils.maxWaitExistsTime);

def OK(speed = Utils.clickSpeed):
    Utils.slowClick("OK.png", speed);

def apply(speed = Utils.clickSpeed):
    Utils.slowClick("Apply.png", speed);

def tabSettings():
    open();

    if exists("TabSettings.png") != None:
        Utils.slowClick("TabSettings.png");

def markMaximumCycloramaViewers(speed = Utils.hoverSpeed):
    markSettings("MaximumCycloramaViewers.png", speed);

def markOverlayDrawDistance(speed = Utils.hoverSpeed):
    markSettings("OverlayDrawDistance.png", speed);

def markShowDetailImages(speed = Utils.hoverSpeed):
    markSettings("ShowDetailImages.png", speed);

def markEnableSmartClickMeasurement(speed = Utils.hoverSpeed):
    markSettings("EnableSmartClickMeasurement.png", speed);

def markSettings(pattern, speed = Utils.hoverSpeed):
    wait(speed);
    Utils.slowHover(pattern, True, speed);

def selectCoordinateSystem(pattern, speed = Utils.clickSpeed):
    Utils.slowClick("SelectCoordinateSystem.png", speed);
    Utils.slowClick(pattern);

def toggleSmartClickMeasurement(speed = Utils.clickSpeed):
    toggleCheckBox("EnableSmartClickMeasurement.png", speed);

def toggleCheckBox(checkbox, speed = Utils.clickSpeed):
    Utils.slowClick(checkbox, speed);

def enableSmartClickMeasurement():
    makeStatusCheckBox("EnableSmartClickMeasurement.png", "CheckedCheckbox.png");

def disableSmartClickMeasurement():
    makeStatusCheckBox("EnableSmartClickMeasurement.png", "UnCheckedCheckbox.png");

def makeStatusCheckBox(checkbox, status):
    r = find(checkbox);

    if r.exists(status) == None:
        toggleCheckBox(checkbox);
    else:
        markSettings(checkbox);

    setThrowException(True);
    r.find(status);
