local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")
local matrixLib = require("utils.matrix")
local drawableSprite = require("structs.drawable_sprite")
local connectedEntities = require("helpers.connected_entities")
local enums = require("consts.celeste_enums")

local JumpBlock = {}

JumpBlock.name = "XaphanHelper/JumpBlock"
JumpBlock.minimumSize = {16, 16}
JumpBlock.depth = 0
JumpBlock.fieldOrder = {"x", "y", "width", "height", "directory", "color", "soundIndex", "index"}
JumpBlock.fieldInformation = {
    index = {
        fieldType = "integer",
    },
    color = {
        fieldType = "color"
    },
    soundIndex = {
        options = enums.tileset_sound_ids,
        editable = false
    },
    switchType = {
        options = {"Wiggle", "Fade"},
        editable = false
    }
}
JumpBlock.placements = {
    name = "JumpBlock",
    data = {
        width = 16,
        height = 16,
        index = 0,
        color = "FFFFFF",
        directory = "objects/XaphanHelper/JumpBlock",
        soundIndex = 35,
        switchType = "Wiggle",
        improvedTileset = false,
        onlyOneTileset = false
    }
}

local function getSearchPredicate(entity)
    return function(target)
        return entity._name == target._name and entity.index == target.index
    end
end

local function getTileSprite(entity, x, y, frame, color, depth, rectangles)
    local improvedTileset = entity.improvedTileset or false
    local hasAdjacent = connectedEntities.hasAdjacent

    local drawX, drawY = (x - 1) * 8, (y - 1) * 8

    local closedLeft = hasAdjacent(entity, drawX - 8, drawY, rectangles)
    local closedRight = hasAdjacent(entity, drawX + 8, drawY, rectangles)
    local closedUp = hasAdjacent(entity, drawX, drawY - 8, rectangles)
    local closedDown = hasAdjacent(entity, drawX, drawY + 8, rectangles)
    local completelyClosed = closedLeft and closedRight and closedUp and closedDown

    local quadX, quadY = false, false

    if improvedTileset then
        if completelyClosed then
            if not hasAdjacent(entity, drawX + 8, drawY - 8, rectangles) then
                quadX, quadY = 0, 64
    
            elseif not hasAdjacent(entity, drawX - 8, drawY - 8, rectangles) then
                quadX, quadY = 8, 64
    
            elseif not hasAdjacent(entity, drawX + 8, drawY + 8, rectangles) then
                quadX, quadY = 16, 64
    
            elseif not hasAdjacent(entity, drawX - 8, drawY + 8, rectangles) then
                quadX, quadY = 24, 64
    
            else
                quadX, quadY = 0, 72
            end
        else
            if closedLeft and closedRight and not closedUp and closedDown then
                quadX, quadY = 0, 0
    
            elseif closedLeft and closedRight and closedUp and not closedDown then
                quadX, quadY = 0, 8
    
            elseif closedLeft and not closedRight and closedUp and closedDown then
                quadX, quadY = 0, 24
    
            elseif not closedLeft and closedRight and closedUp and closedDown then
                quadX, quadY = 0, 16
    
            elseif closedLeft and not closedRight and not closedUp and closedDown then
                quadX, quadY = 0, 40
    
            elseif not closedLeft and closedRight and not closedUp and closedDown then
                quadX, quadY = 0, 32
    
            elseif not closedLeft and closedRight and closedUp and not closedDown then
                quadX, quadY = 0, 48
    
            elseif closedLeft and not closedRight and closedUp and not closedDown then
                quadX, quadY = 0, 56
            end
        end
    else
        if completelyClosed then
            if not hasAdjacent(entity, drawX + 8, drawY - 8, rectangles) then
                quadX, quadY = 24, 0
    
            elseif not hasAdjacent(entity, drawX - 8, drawY - 8, rectangles) then
                quadX, quadY = 24, 8
    
            elseif not hasAdjacent(entity, drawX + 8, drawY + 8, rectangles) then
                quadX, quadY = 24, 16
    
            elseif not hasAdjacent(entity, drawX - 8, drawY + 8, rectangles) then
                quadX, quadY = 24, 24
    
            else
                quadX, quadY = 8, 8
            end
        else
            if closedLeft and closedRight and not closedUp and closedDown then
                quadX, quadY = 8, 0
    
            elseif closedLeft and closedRight and closedUp and not closedDown then
                quadX, quadY = 8, 16
    
            elseif closedLeft and not closedRight and closedUp and closedDown then
                quadX, quadY = 16, 8
    
            elseif not closedLeft and closedRight and closedUp and closedDown then
                quadX, quadY = 0, 8
    
            elseif closedLeft and not closedRight and not closedUp and closedDown then
                quadX, quadY = 16, 0
    
            elseif not closedLeft and closedRight and not closedUp and closedDown then
                quadX, quadY = 0, 0
    
            elseif not closedLeft and closedRight and closedUp and not closedDown then
                quadX, quadY = 0, 16
    
            elseif closedLeft and not closedRight and closedUp and not closedDown then
                quadX, quadY = 16, 16
            end
        end
    end  

    if quadX and quadY then
        local sprite = drawableSprite.fromTexture(frame, entity)

        sprite:addPosition(drawX, drawY)
        sprite:useRelativeQuad(quadX, quadY, 8, 8)
        sprite:setColor(color)

        sprite.depth = depth

        return sprite
    end
end

function JumpBlock.sprite(room, entity)
    local improvedTileset = entity.improvedTileset or false
    local relevantBlocks = utils.filter(getSearchPredicate(entity), room.entities)

    connectedEntities.appendIfMissing(relevantBlocks, entity)

    local rectangles = connectedEntities.getEntityRectangles(relevantBlocks)

    local sprites = {}

    local width, height = entity.width or 32, entity.height or 32
    local tileWidth, tileHeight = math.ceil(width / 8), math.ceil(height / 8)

    local color = entity.color

    local frame
    if improvedTileset then
        if entity.startPressed then
            frame = entity.directory .. "/pressed-impr00"
        else
            frame = entity.directory .. "/solid-impr"
        end
    else
        if entity.startPressed then
            frame = entity.directory .. "/pressed00"
        else
            frame = entity.directory .. "/solid"
        end
    end
    local depth = 0

    for x = 1, tileWidth do
        for y = 1, tileHeight do
            local sprite = getTileSprite(entity, x, y, frame, color, depth, rectangles)

            if sprite then
                table.insert(sprites, sprite)
            end
        end
    end

    return sprites
end

return JumpBlock