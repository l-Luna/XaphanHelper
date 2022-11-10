local fakeTilesHelper = require("helpers.fake_tiles")

local CustomCoverupWall = {}

CustomCoverupWall.name = "XaphanHelper/CustomCoverupWall"
CustomCoverupWall.depth = -13000
CustomCoverupWall.fieldOrder = {
    "x", "y", "width", "height", "flag", "tiletype", "flagTiletype"
}
function CustomCoverupWall.fieldInformation(entity)
    return {
        tiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        },
        flagTiletype = {
            options = fakeTilesHelper.getTilesOptions(),
            editable = false
        }    
    }
end
CustomCoverupWall.placements = {
    name = "CustomCoverupWall",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        flagTiletype = "3",
        flag = ""
    }
}

CustomCoverupWall.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})

return CustomCoverupWall