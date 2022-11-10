local fakeTilesHelper = require("helpers.fake_tiles")

local FlagBlock = {}

FlagBlock.name = "XaphanHelper/FlagBlock"
FlagBlock.depth = -13000
function FlagBlock.fieldInformation(entity)
    return {
        tiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        },
        flagTiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        },
        mode = {
            options = {"Block", "Wall"},
            editable = false
        }
    }
end
FlagBlock.placements = {
    name = "FlagBlock",
    data = {
        width = 8,
        height = 8,
        mode = "Block",
        tiletype = "3",
        flagTiletype = "3",
        flag = "",
        removeFlags = "",
        permanent = true
    }
}

FlagBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return FlagBlock