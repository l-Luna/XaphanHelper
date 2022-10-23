local drawableSprite = require("structs.drawable_sprite")

local FlagDashSwitch = {}

FlagDashSwitch.name = "XaphanHelper/FlagDashSwitch"
FlagDashSwitch.depth = 0
FlagDashSwitch.fieldOrder = {
    "x", "y", "side", "spriteName", "particleColor1", "particleColor2", "flag", "mode", "registerInSaveData", "saveDataOnlyAfterCheckpoint", "canSwapFlag", "inWall", "persistent"
}
FlagDashSwitch.fieldInformation = {
    side = {
        options = {"Left", "Right", "Up", "Down"},
        editable = false
    },
    mode = {
        options = {"SetTrue", "SetFalse", "SetInverted"},
        editable = false;
    },
    particleColor1 = {
        fieldType = "color"
    },
    particleColor2 = {
        fieldType = "color"
    }
}
FlagDashSwitch.placements = {
    name = "FlagDashSwitch",
    data = {
        side = "Up",
        persistent = false,
        spriteName = "default",
        flag = "",
        registerInSaveData = false,
        saveDataOnlyAfterCheckpoint = false,
        canSwapFlag = false,
        particleColor1 = "99E550",
        particleColor2 = "D9FFB5",
        mode = "SetTrue",
        inWall = false
    }
}

function FlagDashSwitch.sprite(room, entity)
    local sprites = {}
    sprites["default"] = "objects/temple/dashButton00"
    sprites["mirror"] = "objects/temple/dashButtonMirror00"
    local texture = entity.spriteName == "default" and sprites["default"] or sprites["mirror"]
    local sprite = drawableSprite.fromTexture(texture, entity)
    local side = entity.side
    local inWall = entity.inWall or false

    if side == "Down" then
        sprite:addPosition(8, inWall and -4 or 0)
        sprite.rotation = -math.pi / 2
    elseif side == "Up" then
        sprite:addPosition(8, inWall and 12 or 8)
        sprite.rotation = math.pi / 2
    elseif side == "Left" then
        sprite:addPosition(inWall and -4 or 0, 8)
        sprite.rotation = math.pi
    elseif side == "Right" then
        sprite:addPosition(inWall and 12 or 8, 8)
        sprite.rotation = 0
    end

    return sprite
end

return FlagDashSwitch