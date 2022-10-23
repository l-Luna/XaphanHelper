local drawableSprite = require("structs.drawable_sprite")

local DroneSwitch = {}

DroneSwitch.name = "XaphanHelper/DroneSwitch"
DroneSwitch.depth = 0
DroneSwitch.canResize = {false, false}
DroneSwitch.fieldInformation = {
    side = {
        options = {"Left", "Right", "Bottom"},
        editable = false
    }
}
DroneSwitch.placements = {
    name = "DroneSwitch",
    data = {
        flag = "",
        side = "Left"
    }
}

function DroneSwitch.sprite(room, entity)
    local side = entity.side or "Left"
    local sprite = drawableSprite.fromTexture("objects/XaphanHelper/DroneSwitch/idle00", entity)
    sprite:addPosition(4, 4)

    if side == "Down" then
        sprite.rotation = -math.pi
    elseif side == "Left" then
        sprite.rotation = math.pi / 2
    elseif side == "Right" then
        sprite.rotation = -math.pi / 2
    end

    return sprite
end

return DroneSwitch