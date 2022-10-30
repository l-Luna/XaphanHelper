local fakeTilesHelper = require("helpers.fake_tiles")

local FlagBlock = {}

FlagBlock.name = "XaphanHelper/FlagBlock"
FlagBlock.depth = -13000
local fieldInformation = {
    mode = {
        options = {"Block", "Wall"},
        editable = false
    }
}
FlagBlock.fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tiletype")
FlagBlock.placements = {
    name = "FlagBlock",
    data = {
        width = 8,
        height = 8,
        mode = "Block",
        tiletype = "3",
        flags = "",
        permanent = true
    }
}

FlagBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return FlagBlock