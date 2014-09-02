from sikuli import *

import Functions.Utils as Utils
import Functions.ArcMap as ArcMap
import Functions.Country as Country
import Scripts.AddTheRecordingLayer as AddTheRecordingLayer
import Scripts.ShowOverlayData as ShowOverlayData
import Scripts.Measurements as Measurements

reload(Utils);
reload(ArcMap);
reload(Country);
reload(AddTheRecordingLayer);
reload(ShowOverlayData);
reload(Measurements);

Record = False;
ArcMapVersion = ArcMap.ArcMap10_0;
CountryCodes = [Country.NL, Country.DE, Country.US_Feet, Country.US_Meters];

# Do all test cases
###################

for CountryCode in CountryCodes:
    AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);
    ShowOverlayData.doPlayShowOverlayData(CountryCode, ArcMapVersion, Record);
    Measurements.doPlayMeasurements(CountryCode, ArcMapVersion, Record);

# Do Single test case
#####################
#CountryCode = Country.NL;
#AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);
#ShowOverlayData.doPlayShowOverlayData(CountryCode, ArcMapVersion, Record);
#Measurements.doPlayMeasurements(CountryCode, ArcMapVersion, Record);
