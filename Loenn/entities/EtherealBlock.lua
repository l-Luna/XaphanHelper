local fakeTilesHelper = require("helpers.fake_tiles")

local EtherealBlock = {}

EtherealBlock.name = "XaphanHelper/EtherealBlock"
EtherealBlock.depth = -13000
local fieldInformation = {
    
}
EtherealBlock.fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tiletype")
EtherealBlock.placements = {
    name = "EtherealBlock",
    data = {
        width = 8,
        height = 8,
        tiletype = "3"
    }
}

EtherealBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return EtherealBlock