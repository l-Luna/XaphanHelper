local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local LaserDetector = {}

LaserDetector.name = "XaphanHelper/LaserDetector"
LaserDetector.depth = -2000
LaserDetector.canResize = {false, false}
LaserDetector.fieldInformation = {
    sides = {
        options = {"Left", "Right", "Top", "Bottom", "Top + Left", "Top + Right", "Bottom + Left", "Bottom + Right", "Top + Bottom", "Left + Right", "Top + Left + Right", "Top + Bottom + Right", "Bottom + Left + Right", "Top + Bottom + Left", "Top + Bottom + Left + Right"},
        editable = false
    }
}
LaserDetector.placements = {
    name = "LaserDetector",
    data = {
        flag = "",
        sides = "Right",
        directory = "objects/XaphanHelper/LaserDetector"
    }
}

function LaserDetector.sprite(room, entity)
    local sides = entity.sides or "Right"
    local directory = entity.directory or "objects/XaphanHelper/LaserDetector"

    local sprites = {}

    local base = drawableSprite.fromTexture(directory .. "/baseActive00", entity)
    local bottomSensor = nil
    local topSensor = nil
    local leftSensor = nil
    local rightSensor = nil

    base:addPosition(4, 4)
  
    if sides:find("Bottom", 1, true) then
        bottomSensor = drawableSprite.fromTexture(directory .. "/sensor00", entity)
        bottomSensor.rotation = math.pi
        bottomSensor:addPosition(4, 12)
    end
    if sides:find("Top", 1, true) then
        topSensor = drawableSprite.fromTexture(directory .. "/sensor00", entity)
        topSensor.rotation = 0
        topSensor:addPosition(4, -4)
    end
    if sides:find("Left", 1, true) then
        leftSensor = drawableSprite.fromTexture(directory .. "/sensor00", entity)
        leftSensor.rotation = -math.pi / 2
        leftSensor:addPosition(-4, 4)
    end
    if sides:find("Right", 1, true) then
        rightSensor = drawableSprite.fromTexture(directory .. "/sensor00", entity)
        rightSensor.rotation = math.pi / 2
        rightSensor:addPosition(12, 4)
    end

    table.insert(sprites, base)
    if bottomSensor then
        table.insert(sprites, bottomSensor)
    end
    if topSensor then
        table.insert(sprites, topSensor)
    end
    if leftSensor then
        table.insert(sprites, leftSensor)
    end
    if rightSensor then
        table.insert(sprites, rightSensor)
    end

    return sprites
end

function LaserDetector.selection(room, entity)
    local sides = entity.sides or "Right"
    local xAdjust = 0
    local width = 8
    local yAdjust = 0
    local height = 8
    if sides:find("Bottom", 1, true) then
        height += 2
    end
    if sides:find("Top", 1, true) then
        yAdjust = -2
        height += 2
    end
    if sides:find("Left", 1, true) then
        xAdjust = -2
        width += 2
    end
    if sides:find("Right", 1, true) then
        width += 2
    end

    return utils.rectangle(entity.x + xAdjust, entity.y + yAdjust, width, height)
end

return LaserDetector