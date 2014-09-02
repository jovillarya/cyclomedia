from sikuli import *

import Utils

reload(Utils);

def getArcMapDir(desktopDirName):
    return Utils.GetProgramFiles32().replace("/", "\\") + "\\ArcGIS\\" + desktopDirName + "\\bin\\ArcMap.exe";

# Application constants
ArcMap10_0 = getArcMapDir("Desktop10.0");
ArcMap10_1 = getArcMapDir("Desktop10.1");
ArcMap10_2 = getArcMapDir("Desktop10.2");

currentVersion = ArcMap10_0;
closeSpeed = 11;

def open(b):
    App.open(b);
    exists("AgreementCheck.png", Utils.maxWaitExistsTime);

    if exists("AgreementCheck.png") != None:
        Utils.slowClick(Pattern("AgreementCheck.png").targetOffset(-106,-2));
        Utils.slowClick("AgreementOK.png");

    exists("mxdSelectCancel.png", Utils.maxWaitExistsTime);

    if exists("mxdSelectCancel.png") != None:
        Utils.slowClick("mxdSelectCancel.png");

def loadMxdFromDefault(mxd, waitPictureUntilFinished):
    filePath = Utils.fileNameWithDefault(mxd);
    loadMxd(filePath, waitPictureUntilFinished);

def loadMxd(mxd, waitPictureUntilFinished):
    setThrowException(True);
    Utils.slowClick("FileMenu.png");
    Utils.slowClick("FileMenuOpen.png");
    wait(1);
    Utils.slowType(mxd);
    Utils.slowClick("Openmxd.png");
    wait(1);

    if waitPictureUntilFinished != None:
        exists(waitPictureUntilFinished, Utils.maxWaitExistsTime);
        find(waitPictureUntilFinished);

def close(speed = closeSpeed):
    wait(speed);
    Utils.slowClick("ArcMapMenuPart.png");
    type(Key.F4, KEY_ALT);

    if exists("CloseSaveChanges.png") != None:
        if exists("SaveChangesNo.png") != None:
            Utils.slowClick("SaveChangesNo.png");
        if exists("SaveChangesNee.png") != None:
            Utils.slowClick("SaveChangesNee.png");

    wait(speed);
