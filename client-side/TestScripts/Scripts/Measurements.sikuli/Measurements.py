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
import Functions.Measurement as Measurement

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
reload(Measurement);

mxdName = "Measurements.mxd";

def playMeasurements(CountryCode = Country.currentCode):
    SubTitle.setStartTime("01");

    SubTitle.printCurrentTime("01");
    Layer.markLayer("01_Measurements.png", 3);

    SubTitle.printCurrentTime("02");
    Cyclorama.selectCycloramaTool();
    Cyclorama.openCyclorama("02_" + CountryCode + "_OpenMeasurementCyclorama.png", 6, 2);

    SubTitle.printCurrentTime("03");
    Measurement.enableSmartClick();

    SubTitle.printCurrentTime("04");
    Layer.toggleShowInCyclorama("04_Measurements.png");

    SubTitle.printCurrentTime("05");
    Layer.setLayerInEdit("05_Measurements.png");

    SubTitle.printCurrentTime("06");
    Measurement.selectFromFeatures("06_Measurements.png", 2);

    SubTitle.printCurrentTime("07");
    Measurement.clickPoint("07_" + CountryCode + "_Point.png", "07_" + CountryCode + "_Results.png");

    SubTitle.printCurrentTime("08");
    Measurement.markResults("08_" + CountryCode + "_Results.png", 3)

    SubTitle.printCurrentTime("09");
    Measurement.DetailsSelectOtherImage("09_" + CountryCode + "_Image1.png", "09_" + CountryCode + "_Check_Image1.png", 5);
    Measurement.DetailsSelectOtherImage("09_" + CountryCode + "_Image2.png", "09_" + CountryCode + "_Check_Image2.png", 5);

    SubTitle.printCurrentTime("10");
    Measurement.clickPoint("10_" + CountryCode + "_Point.png", "10_" + CountryCode + "_Results.png");

    SubTitle.printCurrentTime("11");
    Measurement.deletePoint("11_" + CountryCode + "_Delete.png", 3);

    SubTitle.printCurrentTime("12");
    rotationAngle = {"NL":400, "DE":100, "US_Feet":250, "US_Meters":250}
    Measurement.addPoint("12_" + CountryCode + "_Image.png", "12_" + CountryCode + "_Point.png", rotationAngle[CountryCode], "12_" + CountryCode + "_Results.png", 5);

    SubTitle.printCurrentTime("13");
    Measurement.finished("13_" + CountryCode + "_Position.png");

    SubTitle.printCurrentTime("14");
    Measurement.markResults("14_" + CountryCode + "_ResultsCyclorama.png", 3);
    Measurement.markResults("14_" + CountryCode + "_ResultsMap.png", 3);

    SubTitle.printCurrentTime("15");
    Measurement.saveEdits();
    Measurement.stopEditing();

def doPlayMeasurements(CountryCode = Country.currentCode, ArcMapVersion = ArcMap.currentVersion, record = False):
    # delete any existing files
    fullmxdName = Country.fullMxdName(CountryCode, mxdName);
    fullgdbName = Utils.getgdbLocation(CountryCode + ".gdb");
    Utils.deleteFileFromDefault(fullmxdName);
    Utils.deleteDirectory(fullgdbName);

    windowsPath = getBundlePath().replace("/", "\\");
    thisDirectory = windowsPath + "..\\Scripts\\Measurements.sikuli\\";
    Utils.copyFileToDefault("\"" + thisDirectory + fullmxdName + "\"");
    Utils.copyDirectory("\"" + thisDirectory + CountryCode + ".gdb\"", fullgdbName);

    Settings.delete();
    ArcMap.open(ArcMapVersion);
    Toolbar.show();
    Settings.login();
    ArcMap.loadMxdFromDefault(fullmxdName, CountryCode + "_LoadMxdMeasurement.png");

    if record:
        Camtasia.startVideo(getCenter());

    playMeasurements(CountryCode)

    if record:
        Camtasia.stopVideo("Measurements");

    ArcMap.close();

    # delete any remaining files
    Utils.deleteDirectory(fullgdbName);
    Utils.deleteFileFromDefault(fullmxdName);
