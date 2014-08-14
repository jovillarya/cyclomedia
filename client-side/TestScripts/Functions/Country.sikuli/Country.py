from sikuli import *

NL = "NL";
DE = "DE";
US_Meters = "US_Meters";
US_Feet = "US_Feet";

currentCode = NL;

def fullMxdName(countryCode, mxdName):
    return countryCode + "_" + mxdName;
