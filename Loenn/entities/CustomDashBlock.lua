local fakeTilesHelper = require("helpers.fake_tiles")

local CustomDashBlock = {}

CustomDashBlock.name = "XaphanHelper/CustomDashBlock"
CustomDashBlock.depth = 0
CustomDashBlock.fieldOrder = {
    "x", "y", "width", "height", "flag", "tiletype", "flagTiletype", "blendIn", "canDash", "permanent"
}
function CustomDashBlock.fieldInformation(entity)
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
CustomDashBlock.placements = {
    name = "CustomDashBlock",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        flagTiletype = "3",
        flag = "",
        blendIn = true,
        canDash = true,
        permanent = true
    }
}

CustomDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")

return CustomDashBlock