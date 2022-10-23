local drawableSprite = require("structs.drawable_sprite")

local CellLock = {}

CellLock.name = "XaphanHelper/CellLock"
CellLock.depth = 8999
CellLock.fieldOrder = {
    "x", "y", "sprite", "color", "type", "instant", "cellInside", "keepCell", "flag", "registerInSaveData", "sound", "slotSound"
}
CellLock.fieldInformation = {
    type = {
        options = {"Normal", "Floating"},
        editable = false
    },
    color = {
        options = {"Blue", "Red", "Green", "Yellow", "Grey"},
        editable = false
    }
}
CellLock.placements = {
    name = "CellLock",
    data = {
        sprite = "objects/XaphanHelper/CellLock",
        color = "Blue",
        flag = "",
        registerInSaveData = false,
        sound = "",
        cellInside = false,
        keepCell = false,
        slotSound = "event:/game/05_mirror_temple/button_activate",
        instant = false,
        type = "Normal"
    }
}

function CellLock.sprite(room, entity)
    local color = entity.color or "Blue"
    local cellInside = entity.cellInside
    local sprite = entity.Sprite or "objects/XaphanHelper/CellLock"
    local type = entity.type or "Normal"

    local sprites = {}

    local typeSprite = drawableSprite.fromTexture(sprite .. "/" .. type .. "00", entity)
    local colorSprite = drawableSprite.fromTexture(sprite .. "/" .. color .. "00", entity)
    local cellSprite = nil
    local leverSprite = nil

    if cellInside then
        cellSprite = drawableSprite.fromTexture(sprite .. "/bgCell00", entity)
        leverSprite = drawableSprite.fromTexture(sprite .. "/lever00", entity)
    end

    if typeSprite then
        typeSprite:addPosition(4, 0)
        typeSprite:setColor("White")
        table.insert(sprites, typeSprite)
    end
    if colorSprite then
        colorSprite:addPosition(4, 0)
        colorSprite:setColor("White")
        table.insert(sprites, colorSprite)
    end
    if cellSprite then
        cellSprite:addPosition(4, 5)
        cellSprite:setColor("White")
        table.insert(sprites, cellSprite)
    end
    if leverSprite then
        leverSprite:addPosition(4, 5)
        leverSprite:setColor("White")
        table.insert(sprites, leverSprite)
    end
  
    return sprites
end

return CellLock