from sikuli import *

import os

# Interaction constants
Settings.MoveMouseDelay = 1.0;

typeSpeed = 0.1;
clickSpeed = 1;
hoverSpeed = 2;
wheelSpeed = 0.5;
existsTime = 5;
maxWaitExistsTime = 90;

defaultDirectory = "%USERPROFILE%\\Desktop\\";
defaultGdbLocation = "C:\\";

def Is64Windows():
    return 'PROGRAMFILES(X86)' in os.environ

def GetProgramFiles32():
    if Is64Windows():
        return os.environ['PROGRAMFILES(X86)']
    else:
        return os.environ['PROGRAMFILES']

def GetProgramFiles64():
    if Is64Windows():
        return os.environ['PROGRAMW6432']
    else:
        return None

def fileNameWithDefault(fileName):
    return defaultDirectory + fileName;

def getgdbLocation(gdbName):
    return "\"" + defaultGdbLocation + gdbName + "\"";

def deleteFileFromDefault(fileName):
    filePath = fileNameWithDefault(fileName);
    deleteFile(filePath);

def deleteFile(fileName):
    App.open("cmd /k del " + fileName);
    wait(1);
    slowType("exit\n");

def deleteDirectory(dirName):
    App.open("cmd /k rd " + dirName + "/S /Q");
    wait(1);
    slowType("exit\n");

def copyFileToDefault(fileName):
    copyFile(fileName, defaultDirectory);

def copyFile(fileName, destination):
    App.open("cmd /k copy " + fileName + " " + destination);
    wait(1);
    slowType("exit\n");

def copyAllFiles(source, destination):
    allFiles = source + "\\*.*";
    copyFile(allFiles, destination);

def makeDirectory(dirName):
    App.open("cmd /k md " + dirName);
    wait(1);
    slowType("exit\n");

def copyDirectory(source, destination):
    makeDirectory(destination);
    copyAllFiles(source, destination);

# custom interaction methods
def slowType(text, speed = typeSpeed):
    for c in text:
        type(c);
        wait(speed);

def slowClick(pattern, speed = clickSpeed, silent = False):
    if checkPattern(pattern, silent):
        if(isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        click(location);
        return location;

def slowDoubleClick(pattern, speed = clickSpeed, silent = False):
    if checkPattern(pattern, silent):
        if( isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        doubleClick(location);
        return location;

def slowRightClick(pattern, speed = clickSpeed, silent = False):
    if checkPattern(pattern, silent):
        if( isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        rightClick(location);
        return location;

def slowMouseDown(pattern, speed = clickSpeed, silent = False):
    if checkPattern(pattern, silent):
        if( isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        mouseMove(location);
        mouseDown(Button.LEFT);
        return location;

def slowMouseUp(pattern, speed = clickSpeed, silent = False):
    if checkPattern(pattern, silent):
        if(isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        mouseMove(location);
        mouseUp(Button.LEFT);
        return location;

def slowHover(pattern, show = False, speed = hoverSpeed, silent = False):
    if checkPattern(pattern, silent):
        m = None;
        if(isinstance(pattern, Location)):
            location = pattern;
        elif(isinstance(pattern, Region)):
            location = pattern.getCenter();
            m = pattern;
        else:
            m = find(pattern);
            location = m.getTarget();
        
        if m and show:
            m.highlight();
        
        wait(speed);
        hover(location);
        
        if m and show:
            m.highlight();
        
        return location;

def slowWheel(pattern, direction, lines = 1, speed = wheelSpeed, patternVanish = None, silent = False):
    if checkPattern(pattern, silent):
        if(isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();        
        for num in range(0,lines):
            wait( speed);

            if (patternVanish != None):
                waitVanish(patternVanish);

            wheel(location, direction, 1);
        return location;

def slowDragDrop(pattern, dx, dy, downTime = 0.0, speed = clickSpeed,  silent = False):
    if checkPattern(pattern, silent):
        if(isinstance(pattern, Location)):
            location = pattern;
        else:
            location = find(pattern).getTarget();
        wait(speed);
        drag(location);
        dropLocation = Location(location.x + dx, location.y + dy);
        wait(downTime);
        dropAt(dropLocation);
        return dropLocation;

def checkPattern(pattern, silent = False, debug = False):
    if(isinstance(pattern, Location) or isinstance(pattern, Region)):
        return True;
    elif((silent and exists(pattern, 0.0)) or exists(pattern, existsTime)):

        if debug:
            ms = list(findAll(pattern));
            print(str(len(ms)) + " matches found for " + pattern.getFilename());
            if len(ms) > 1:
                for m in ms: 
                    m.highlight();
                for m in ms: 
                    m.highlight();
            elif len(ms) == 1:
                ms[0].highlight(3);
        return True;
    elif not silent:
        if(isinstance(pattern, Pattern)):
            print(pattern.getFilename() + " not found!");
        elif(isinstance(pattern, str)):
            print(pattern + " not found!");
        else:
            print("Utils.check::Unknown pattern");
        exit();
    return False;

def createRegionFromLocation(pattern, w, h, offsetX = 0, offsetY = 0):
    if(isinstance(pattern, Location)):
        location = pattern;
    elif(isinstance(pattern, Region)):
        location = pattern.getCenter();
        m = pattern;
    else:
        m = find(pattern);
        location = m.getTarget();
        
    region = Region(location.x + offsetX, location.y + offsetY, w, h); 
    return region;
