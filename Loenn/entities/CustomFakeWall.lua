local fakeTilesHelper = require("helpers.fake_tiles")

local CustomFakeWall = {}

CustomFakeWall.name = "XaphanHelper/CustomFakeWall"
CustomFakeWall.depth = -13000
CustomFakeWall.fieldOrder = {
    "x", "y", "width", "height", "flag", "mode", "tiletype", "flagTiletype", "group", "playTransitionReveal"
}
function CustomFakeWall.fieldInformation(entity)
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
        },
        group = {
            fieldType = "integer"
        }     
    }
end
CustomFakeWall.placements = {
    name = "CustomFakeWall",
    data = {
        width = 8,
        height = 8,
        mode = "Block",
        tiletype = "3",
        flagTiletype = "3",
        flag = "",
        group = 0,
        playTransitionReveal = false
    }
}

CustomFakeWall.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return CustomFakeWall