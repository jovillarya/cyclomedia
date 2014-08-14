from sikuli import *

import Functions.Utils as Utils
import Functions.ArcMap as ArcMap
import Functions.Country as Country
import Scripts.AddTheRecordingLayer as AddTheRecordingLayer

reload(Utils);
reload(ArcMap);
reload(Country);
reload(AddTheRecordingLayer);

Record = False;
ArcMapVersion = ArcMap.ArcMap10_0;
CountryCode = Country.NL;
AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);

CountryCode = Country.DE;
AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);

CountryCode = Country.US_Feet;
AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);

CountryCode = Country.US_Meters;
AddTheRecordingLayer.doPlayAddTheRecordingLayer(CountryCode, ArcMapVersion, Record);
