local utils = require("utils")

local CustomTorch = {}

CustomTorch.name = "XaphanHelper/CustomTorch"
CustomTorch.depth = 2000
CustomTorch.fieldOrder = {
    "x", "y", "sprite", "sound", "startLit", "playLitSound", "color", "alpha", "startFade", "endFade", "flag"
}
CustomTorch.fieldInformation = {
    color = {
        fieldType = "color"
    },
    startFade = {
        fieldType = "integer",
    },
    endFade = {
        fieldType = "integer",
    }
}
CustomTorch.placements = {
    name = "CustomTorch",
    data = {
        playLitSound = false,
        startLit = false,
        flag = "",
        color = "FFA500",
        sprite = "objects/XaphanHelper/CustomTorch/torch",
        alpha = 1.00,
        startFade = 48,
        endFade = 64,
        sound = "event:/game/05_mirror_temple/torch_activate"
    }
}

function CustomTorch.texture(room, entity)
    local texture = entity.sprite or "objects/XaphanHelper/CustomTorch/torch"
  
    return texture .. "00"
end

function CustomTorch.selection(room, entity)
    return utils.rectangle(entity.x, entity.y , 16, 16)
end

CustomTorch.offset = {0, 0}

return CustomTorch