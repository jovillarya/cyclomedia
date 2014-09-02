from sikuli import *

import Functions.Utils as Utils
import Functions.Map as Map
import Functions.Camtasia as Camtasia
import Functions.SubTitle as SubTitle
import Functions.ArcMap as ArcMap
import Functions.Toolbar as Toolbar
import Functions.Settings as Settings
import Functions.Country as Country
import Functions.Cyclorama as Cyclorama
import Functions.Layer as Layer

reload(Utils);
reload(Map);
reload(Camtasia);
reload(SubTitle);
reload(ArcMap);
reload(Toolbar);
reload(Settings);
reload(Country);
reload(Cyclorama);
reload(Layer);

mxdName = "ShowOverlayData.mxd";

def playAddTheRecordingLayer(CountryCode = Country.currentCode):
    SubTitle.setStartTime("01");

    SubTitle.printCurrentTime("01");
    Layer.markLayer("VectorData_01_" + CountryCode + ".png", 3);
    Map.zoomIn();
    Map.zoomOut();

    SubTitle.printCurrentTime("02");
    Cyclorama.selectCycloramaTool();
    Cyclorama.openCyclorama("OpenCyclorama_02_" + CountryCode + ".png", 6, 2);

    SubTitle.printCurrentTime("03");
    Layer.toggleShowInCyclorama("VectorData_03_" + CountryCode + ".png", 3);

    SubTitle.printCurrentTime("04");
    Cyclorama.checkInCyclorama("CheckInCyclorama_04_" + CountryCode + ".png");
    Cyclorama.rotateViewer(200, 0, 1, 1);
    Cyclorama.rotateViewer(-200, 0, 1, 1);

    SubTitle.printCurrentTime("05");
    Settings.tabSettings();
    Settings.markOverlayDrawDistance(3);
    Settings.OK();

def doPlayShowOverlayData(CountryCode = Country.currentCode, ArcMapVersion = ArcMap.currentVersion, record = False):
    # delete any existing files
    fullmxdName = Country.fullMxdName(CountryCode, mxdName);
    fullgdbName = Utils.getgdbLocation(CountryCode + ".gdb");
    Utils.deleteFileFromDefault(fullmxdName);
    Utils.deleteDirectory(fullgdbName);

    windowsPath = getBundlePath().replace("/", "\\");
    thisDirectory = windowsPath + "..\\Scripts\\ShowOverlayData.sikuli\\";
    Utils.copyFileToDefault("\"" + thisDirectory + fullmxdName + "\"");
    Utils.copyDirectory("\"" + thisDirectory + CountryCode + ".gdb\"", fullgdbName);

    Settings.delete();
    ArcMap.open(ArcMapVersion);
    Toolbar.show();
    Settings.login();
    ArcMap.loadMxdFromDefault(fullmxdName, "LoadMxd_" + CountryCode + ".png");

    if record:
        Camtasia.startVideo(Location(getCenter().getX(), 10));

    playAddTheRecordingLayer(CountryCode)

    if record:
        Camtasia.stopVideo("Show overlay data");

    ArcMap.close();

    # delete any remaining files
    Utils.deleteDirectory(fullgdbName);
    Utils.deleteFileFromDefault(fullmxdName);
