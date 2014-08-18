from sikuli import *

import Utils

reload(Utils);

# Application constants
ArcMap10_0 = "C:\Program Files (x86)\\ArcGIS\\Desktop10.0\\Bin\\ArcMap.exe";
ArcMap10_1 = "C:\Program Files (x86)\\ArcGIS\\Desktop10.1\\Bin\\ArcMap.exe";
ArcMap10_2 = "C:\Program Files (x86)\\ArcGIS\\Desktop10.2\\Bin\\ArcMap.exe";

currentVersion = ArcMap10_0;
closeSpeed = 5;

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
    Utils.slowClick("CloseArcMap.png");

    if exists("CloseSaveChanges.png") != None:
        Utils.slowClick("SaveChangesNo.png");

    wait(speed);
