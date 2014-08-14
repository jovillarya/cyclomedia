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

def markMaximumCycloramaViewers(speed):
    wait(speed);
    Utils.slowHover("MaximumCycloramaViewers.png", True, speed);

def markShowDetailImages(speed):
    wait(speed);
    Utils.slowHover("ShowDetailImages.png", True, speed);

def selectCoordinateSystem(pattern, speed = Utils.clickSpeed):
    Utils.slowClick("SelectCoordinateSystem.png");
    Utils.slowClick(pattern);
