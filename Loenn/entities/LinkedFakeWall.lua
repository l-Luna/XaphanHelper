local fakeTilesHelper = require("helpers.fake_tiles")

local LinkedFakeWall = {}

LinkedFakeWall.name = "XaphanHelper/LinkedFakeWall"
LinkedFakeWall.depth = -13000
local fieldInformation = {
    mode = {
        options = {"Block", "Wall"},
        editable = false
    }
}
LinkedFakeWall.fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tiletype")
LinkedFakeWall.placements = {
    name = "LinkedFakeWall",
    data = {
        width = 8,
        height = 8,
        mode = "Block",
        tiletype = "3",
        playTransitionReveal = false
    }
}

LinkedFakeWall.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return LinkedFakeWall