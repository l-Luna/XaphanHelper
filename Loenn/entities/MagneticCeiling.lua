local drawableSprite = require("structs.drawable_sprite")

local MagneticCeiling = {}

MagneticCeiling.name = "XaphanHelper/MagneticCeiling"
MagneticCeiling.depth = -9999
MagneticCeiling.fieldOrder = {"x", "y", "width", "directory", "animationSpeed", "canJump", "noStaminaDrain"}
MagneticCeiling.canResize = {true, false}
MagneticCeiling.placements = {
    name = "MagneticCeiling",
    data = {
        width = 8,
        directory = "objects/XaphanHelper/MagneticCeiling",
        animationSpeed = 0.20,
        canJump = false,
        noStaminaDrain = false
    }
}

function MagneticCeiling.sprite(room, entity)
    local sprites = {}

    local directory = entity.directory or "objects/XaphanHelper/MagneticCeiling"
    local width = entity.width / 8 or 1

    local sprite = nil
    for i = 1,width do
        if (i % 2 == 0) then
            sprite = drawableSprite.fromTexture(directory .. "/idle_b00", entity)
        else
            sprite = drawableSprite.fromTexture(directory .. "/idle_a00", entity)
        end
        sprite:addPosition((i - 1) * 8 + 4, 4)
        if sprite then
            table.insert(sprites, sprite)
        end
    end

    return sprites
end

return MagneticCeiling