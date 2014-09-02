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

mxdName = "AddTheRecordingLayer.mxd";

def playAddTheRecordingLayer(CountryCode = Country.currentCode):
    SubTitle.setStartTime("01");

    SubTitle.printCurrentTime("01");
    Layer.addRecent();

    SubTitle.printCurrentTime("02");
    Layer.markRecent(3);

    SubTitle.printCurrentTime("03");
    Map.zoomToRecordingLayer("ZoomToRecordingLayer_03_" + CountryCode + ".png", 5, getCenter(), 10, 3.0);

    SubTitle.printCurrentTime("04");
    Layer.addHistorical("AddHistorical_04_" + CountryCode + ".png");

    SubTitle.printCurrentTime("05");
    Layer.markHistorical(3);

    SubTitle.printCurrentTime("06");
    Layer.openHistory();

    SubTitle.printCurrentTime("07");
    stepSize = {"NL":35, "DE":140, "US_Feet":105, "US_Meters":105}
    stepsFrom1 = {"NL":-2, "DE":-1, "US_Feet":-1, "US_Meters":-1}
    stepsFrom2 = {"NL":2, "DE":1, "US_Feet":1, "US_Meters":1}
    stepsTo1 = {"NL":1, "DE":1, "US_Feet":2, "US_Meters":2}
    stepsTo2 = {"NL":-1, "DE":0, "US_Feet":0, "US_Meters":0}
    Slider1 = {"NL": Pattern("HistorySliderFrom_1_07_NL.png").targetOffset(2,-8), "DE": Pattern("HistorySliderFrom_1_07_DE.png").targetOffset(0,-9), "US_Feet": Pattern("HistorySliderFrom_1_07_US_Feet.png").targetOffset(2,-8), "US_Meters": Pattern("HistorySliderFrom_1_07_US_Meters.png").targetOffset(2,-8)}
    Slider2 = {"NL": Pattern("HistorySliderFrom_2_07_NL.png").targetOffset(2,-10), "DE": Pattern("HistorySliderFrom_2_07_DE.png").targetOffset(1,-5), "US_Feet": Pattern("HistorySliderFrom_2_07_US_Feet.png").targetOffset(1,-6), "US_Meters": Pattern("HistorySliderFrom_2_07_US_Meters.png").targetOffset(1,-6)}
    Slider3 = {"NL": Pattern("HistorySliderTo_1_07_NL.png").targetOffset(0,-7), "DE": Pattern("HistorySliderTo_1_07_DE.png").targetOffset(0,-9), "US_Feet": Pattern("HistorySliderTo_1_07_US_Feet.png").targetOffset(2,-6), "US_Meters": Pattern("HistorySliderTo_1_07_US_Meters.png").targetOffset(2,-6)}
    Slider4 = {"NL": Pattern("HistorySliderTo_2_07_NL.png").targetOffset(0,-7), "DE": Pattern("HistorySliderTo_2_07_DE.png").targetOffset(4,-7), "US_Feet": Pattern("HistorySliderTo_2_07_US_Feet.png").targetOffset(4,-8), "US_Meters": Pattern("HistorySliderTo_2_07_US_Meters.png").targetOffset(4,-8)}
    Layer.moveHistorySlider("MoveHistorySliderFrom_1_07_" + CountryCode + ".png", stepsFrom1[CountryCode], 2, Slider1[CountryCode], stepSize[CountryCode]);
    Layer.moveHistorySlider("MoveHistorySliderFrom_2_07_" + CountryCode + ".png", stepsFrom2[CountryCode], 2, Slider2[CountryCode], stepSize[CountryCode]);
    Layer.moveHistorySlider("MoveHistorySliderTo_1_07_" + CountryCode + ".png", stepsTo1[CountryCode], 2, Slider3[CountryCode], stepSize[CountryCode]);
    Layer.moveHistorySlider("MoveHistorySliderTo_2_07_" + CountryCode + ".png", stepsTo2[CountryCode], 2, Slider4[CountryCode], stepSize[CountryCode]);
    Layer.closeHistory();
    Map.zoomIn();
    Map.zoomOut();

    SubTitle.printCurrentTime("08");
    Settings.tabSettings();

    SubTitle.printCurrentTime("09");
    Settings.selectCoordinateSystem("SelectCoordSystem_09_" + CountryCode + ".png", 3);

    SubTitle.printCurrentTime("10");
    Settings.markMaximumCycloramaViewers(3);

    SubTitle.printCurrentTime("11");
    Settings.markShowDetailImages(3);

    SubTitle.printCurrentTime("12");
    Settings.apply(3);
    Settings.OK();

    SubTitle.printCurrentTime("13");
    Cyclorama.selectCycloramaTool();

    SubTitle.printCurrentTime("14");
    Cyclorama.openCyclorama("OpenCyclorama_14_" + CountryCode + ".png", 8, 2);
    Cyclorama.rotateViewer(200, 0, 1, 1);
    Cyclorama.rotateViewer(-200, 0, 1, 1);

    SubTitle.printCurrentTime("15");
    Cyclorama.openCyclorama("OpenCyclorama_15_" + CountryCode + ".png", 8, 2);
    Cyclorama.rotateViewer(200, 0, 2, 2);
    Cyclorama.rotateViewer(-200, 0, 2, 2);

    SubTitle.printCurrentTime("16");
    Layer.switchRecentRecordings("SwitchRecentRecordingsMap_16_" + CountryCode + ".png", "SwitchRecentRecordingsCyclorama_16_" + CountryCode + ".png", 2);

    SubTitle.printCurrentTime("17");
    Layer.toggleShowInCycloramaRecentRecordings(3);

    SubTitle.printCurrentTime("18");
    Cyclorama.checkNotInCyclorama("CheckNotInCyclorama_18_" + CountryCode + ".png");

def doPlayAddTheRecordingLayer(CountryCode = Country.currentCode, ArcMapVersion = ArcMap.currentVersion, record = False):
    # delete any existing files
    fullmxdName = Country.fullMxdName(CountryCode, mxdName);
    Utils.deleteFileFromDefault(fullmxdName);

    windowsPath = getBundlePath().replace("/", "\\");
    Utils.copyFileToDefault("\"" + windowsPath + "..\\Scripts\\AddTheRecordingLayer.sikuli\\" + fullmxdName + "\"");

    Settings.delete();
    ArcMap.open(ArcMapVersion);
    Toolbar.show();
    Settings.login();
    ArcMap.loadMxdFromDefault(fullmxdName, "LoadMxd.png");

    if record:
        Camtasia.startVideo(Location(getCenter().getX(), 10));

    playAddTheRecordingLayer(CountryCode)

    if record:
        Camtasia.stopVideo("Add the recording layer");

    ArcMap.close();

    # delete any remaining files
    Utils.deleteFileFromDefault(fullmxdName);
