local drawableSprite = require("structs.drawable_sprite")

local TimedDashSwitch = {}

TimedDashSwitch.name = "XaphanHelper/TimedDashSwitch"
TimedDashSwitch.depth = 0
TimedDashSwitch.fieldOrder = {
    "x", "y", "side", "spriteName", "timer", "mode", "tickingType", "flag", "inWall", "particleColor1", "particleColor2"
}
TimedDashSwitch.fieldInformation = {
    side = {
        options = {"Left", "Right", "Up", "Down"},
        editable = false
    },
    timer = {
        fieldType = "integer",
    },
    mode = {
        options = {"add", "set"},
        editable = false
    },
    tickingType = {
        options = {"On Top", "Tick only", "None"},
        editable = false
    },
    particleColor1 = {
        fieldType = "color"
    },
    particleColor2 = {
        fieldType = "color"
    }
}
TimedDashSwitch.placements = {
    name = "TimedDashSwitch",
    data = {
        side = "Up",
        spriteName = "default",
        timer = 10,
        mode = "add",
        tickingType = "Tick only",
        particleColor1 = "99E550",
        particleColor2 = "D9FFB5",
        flag = "",
        inWall = false
    }
}

function TimedDashSwitch.sprite(room, entity)
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

return TimedDashSwitch