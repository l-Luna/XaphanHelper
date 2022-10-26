local utils = require("utils")

local Liquid = {}

Liquid.name = "XaphanHelper/Liquid"
Liquid.depth = function(room, entity)
    local depth = 0
    if entity.foreground then
        depth = -19999
    else
        depth = -9999
    end
    return depth
end
Liquid.fieldOrder = {
    "x", "y", "width", "height", "liquidType", "directory", "surfaceHeight", "lowPosition", "color", "transparency", "frameDelay", "riseDistance", "riseDelay", "riseSpeed", "riseFlag", "riseEndFlag", "removeFlags", "riseShake", "riseSound", "canSwim", "visualOnly", "foreground"
}
Liquid.fieldInformation = {
    lowPosition = {
        fieldType = "integer",
    },
    liquidType = {
        options = {"acid", "acid_b", "lava", "quicksand", "water"},
        editable = false
    },
    color = {
        fieldType = "color"
    },
    riseDistance = {
        fieldType = "integer",
    },
    riseSpeed = {
        fieldType = "integer",
    },
    surfaceHeight = {
        fieldType = "integer",
        minimumValue = 0
        }
}
Liquid.placements = {
    name = "Liquid",
    data = {
        width = 8,
        height = 8,
        lowPosition = 0,
        liquidType = "acid",
        frameDelay = 0.15,
        color = "88C098",
        transparency = 0.65,
        foreground = false,
        riseDelay = 0.00,
        riseDistance = 0,
        riseSpeed = 10,
        riseShake = false,
        riseFlag = "",
        riseEndFlag = "",
        riseSound = false,
        directory = "objects/XaphanHelper/liquid",
        surfaceHeight = 0,
        canSwim = false,
        visualOnly = false,
        removeFlags = ""
    }
}

local function getEntityColor(entity)
    local defaults = {
        acid = "88C098",
        acid_b = "88C098",
        lava = "F85818",
        quicksand = "C8B078",
        water = "669CEE"
    }
    
    local rawColor = nil
    if not entity.color or entity.color == "" then
        rawColor = defaults[entity.liquidType or "acid"] or "FFFFFF"
    else
        rawColor = entity.color or "FFFFFF"
    end

    local color = utils.getColor(rawColor)

    return color
end

Liquid.fillColor = function(room, entity)
    local color = getEntityColor(entity)

    return {color[1] * 0.3, color[2] * 0.3, color[3] * 0.3, 0.6}
end

Liquid.borderColor = function(room, entity)
    local color = getEntityColor(entity)

    return {color[1] * 0.8, color[2] * 0.8, color[3] * 0.8, 0.8}
end

return Liquid