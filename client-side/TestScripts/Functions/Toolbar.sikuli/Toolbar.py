from sikuli import *

import Utils

reload(Utils);

def show():
    if exists("Toolbar.png") == None:
        Utils.slowRightClick("ArcMapMenuPart.png");
        Utils.slowClick("ToolbarGlobeSpotter.png");
        exists("Toolbar.png", Utils.maxWaitExistsTime)
