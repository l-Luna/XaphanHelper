local fakeTilesHelper = require("helpers.fake_tiles")

local CustomExitBlock = {}

CustomExitBlock.name = "XaphanHelper/CustomExitBlock"
CustomExitBlock.depth = -13000
local fieldInformation = {
    group = {
        fieldType = "integer"
    }
}
CustomExitBlock.fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tiletype")
CustomExitBlock.placements = {
    name = "CustomExitBlock",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        closeSound = false,
        group = 0
    }
}

CustomExitBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return CustomExitBlock