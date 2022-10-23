local fakeTilesHelper = require("helpers.fake_tiles")

local BreakBlock = {}

BreakBlock.name = "XaphanHelper/BreakBlock"
BreakBlock.depth = -13000
local fieldInformation = {
    mode = {
        options = {"Block", "Wall"},
        editable = false
    },
    type = {
        options = {"Bomb", "Drone", "LightningDash", "MegaBomb", "RedBooster", "ScrewAttack"},
        editable = false
    },
    color = {
        fieldType = "color"
    }
}
BreakBlock.fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tiletype")
BreakBlock.placements = {
    name = "BreakBlock",
    data = {
        width = 8,
        height = 8,
        mode = "Block",
        tiletype = "3",
        type = "Bomb",
        color = "FFFFFF",
        startRevealed = false,
        directory = "objects/XaphanHelper/BreakBlock"
    }
}

BreakBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return BreakBlock