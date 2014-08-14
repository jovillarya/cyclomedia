from sikuli import *

class ScenarioTimer:
    startTime = 0;
    chapter = "";

timer = ScenarioTimer();

def setStartTime(chapter):
    if(chapter):
        timer.chapter = chapter;
    timer.startTime = time.clock(); 
    print( "<<< Starting chapter" + chapter + " >>>" );

def printCurrentTime(subTitle):
    duration = time.clock() - timer.startTime;
    print( '''<classes:SubTitle time="''' + "%0.1f" % duration + '''" name="SubTitle''' + subTitle + '''" chapterName="Chapter''' + timer.chapter + '''"/>''' );
    wait(0.5);
