from sikuli import *

import Utils

reload(Utils);

Camtasia = Utils.GetProgramFiles32().replace("/", "\\") + "\\TechSmith\\Camtasia Studio 5\\CamtasiaStudio.exe";

VideoPath = "%USERPROFILE%\\Camtasia Studio New\\";
VideoExtension = ".camrec";

def startVideo(location):
    camtasiaApp = App.open(Camtasia);
    Utils.slowClick("MakeRecording.png");
    Utils.slowClick("SelectAreaToRecord.png");
    Utils.slowClick(location);
    wait(0.5);
    dragLocation = Utils.slowHover("RecorderSelectionArea.png");
    recordLocation = Utils.slowHover("Record.png");
    drag(dragLocation);
    dropLocation = getCenter();
    dropLocation.x -= abs(recordLocation.x - dragLocation.x);
    dropLocation.y -= abs(recordLocation.y - dragLocation.y);
    dropAt(dropLocation);

    Utils.slowClick("Record.png");
    click(getCenter());

def stopVideo(fileName):
    wait(3);
    keyDown(Key.F9);
    keyUp();

    Utils.slowClick("ResumeStopDelete.png");

    Utils.slowClick("EnterFileName.png");
    Utils.slowType(VideoPath + fileName + VideoExtension + "\n");
    Utils.slowClick("EnterFileNameYes.png", Utils.clickSpeed, True);
    exists("EnterFileNameOK.png", 60 );
    Utils.slowClick("EnterFileNameOK.png");
    Utils.slowClick("Next.png");
    Utils.slowClick("Next.png");
    wait(0.5);
    Utils.slowClick(Pattern("FitIn.png").targetOffset(146,0));
    wait(0.5);
    Utils.slowClick("Resolution1024x768.png");
    wait(0.5);
    Utils.slowClick("Next.png");
    Utils.slowClick("Next.png");
    Utils.slowType(fileName + "\n");
    Utils.slowClick("Yes.png", Utils.clickSpeed, True);
    wait(3);
    waitVanish("RenderingProject.png", FOREVER);
    wait(3);
    
    # try to close all Camtasia windows
    Utils.slowClick("Close.png");
    wait(1);
    Utils.slowClick("CamtasiaRecorder.png");
    Utils.slowClick("Close.png");
    wait(3);
