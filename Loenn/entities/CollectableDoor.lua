local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")
local drawing = require("utils.drawing")
local enums = require("consts.celeste_enums")

local CollectableDoor = {}

CollectableDoor.name = "XaphanHelper/CollectableDoor"
CollectableDoor.depth = 0
CollectableDoor.nodeLimits = {0, 1}
CollectableDoor.fieldOrder = {
    "x", "y", "width", "height", "orientation", "mode", "edges", "edgesAnimationMode", "directory", "mapIcon", "interiorColor", "mistColor", "iconsColor", "edgesColor", "interiorParticlesColor", "openingParticlesColor1", "openingParticlesColor2", "sliceColor", "sliceParticlesColor1", "sliceParticlesColor2", "requires", "flags", "openSpeedMultiplier", "beforeSliceDelay", "afterSliceDelay", "checkDistance", "checkDisplaySpeed", "unlockSound", "fillSound", "soundIndex", "registerInSaveData"
}
CollectableDoor.minimumSize = {24, 24}
CollectableDoor.fieldInformation = {
    orientation = {
        options = { "Vertical", "Horizontal" },
        editable = false
    },
    mode = {
        options = { "TotalHearts", "CurrentChapterHeart", "CurrentSessionHeart", "TotalCassettes", "CurrentChapterCassette", "CurrentSessionCassette", "TotalStrawberries", "CurrentChapterStrawberries", "CurrentSessionStrawberries", "GoldenStrawberry", "Flags" },
        editable = false
    },
    requires = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 9999
    },
    interiorColor = {
        fieldType = "color"
    },
    mistColor = {
        fieldType = "color"
    },
    edgesColor = {
        fieldType = "color"
    },
    iconsColor = {
        fieldType ="color"
    },
    interiorParticlesColor = {
        fieldType = "color"
    },
    openingParticlesColor1 = {
        fieldType = "color"
    },
    openingParticlesColor2 = {
        fieldType = "color"
    },
    sliceColor = {
        fieldType = "color"
    },
    sliceParticlesColor1 = {
        fieldType = "color"
    },
    sliceParticlesColor2 = {
        fieldType = "color"
    },
    checkDistance = {
        fieldType = "integer",
        minimumValue = 1
    },
    checkDisplaySpeed = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 20
    },
    soundIndex = {
        options = enums.tileset_sound_ids,
        editable = false
    },
    edges = {
        options = { "All", "LeftRight", "TopBottom", "None" },
        editable = false
    },
    edgesAnimationMode = {
        options = { "Clockwise", "CounterClockwise", "Static" },
        editable = false
    }
}
CollectableDoor.placements = {
    name = "CollectableDoor",
    data = {
        width = 24,
        height = 24,
        requires = 1,
        orientation = "Vertical",
        mode = "TotalHearts",
        interiorColor = "18668f",
        mistColor = "ffffff",
        edgesColor = "ffffff",
        iconsColor = "ffffff",
        interiorParticlesColor = "ffffff",
        openingParticlesColor1 = "baffff",
        openingParticlesColor2 = "5abce2",
        sliceColor = "ffffff";
        sliceParticlesColor1 = "ffffff",
        sliceParticlesColor2 = "ffffff",
        directory = "",
        flags = "",
        checkDistance = 10,
        checkDisplaySpeed = 5,
        unlockSound = "event:/game/09_core/frontdoor_unlock",
        fillSound = "event:/game/09_core/frontdoor_heartfill",
        soundIndex = 32,
        beforeSliceDelay = 0.5,
        afterSliceDelay = 0.6,
        openSpeedMultiplier = 1.0,
        edges = "All",
        edgesAnimationMode = "Clockwise",
        mapIcon = "",
        registerInSaveData = false
    }
}

local collectablePadding = 2

local function collectablesWidth(collectableSpriteWidth, collectables)
    return collectables * (collectableSpriteWidth + collectablePadding) - collectablePadding
end

local function collectablesPossible(edgeSpriteWidth, collectableSpriteWidth, width, required)
    local rowWidth = width - 2 * edgeSpriteWidth

    for i = 0, required do
        if collectablesWidth(collectableSpriteWidth, i) > rowWidth then
            return i - 1
        end
    end

    return required
end

function CollectableDoor.sprite(room, entity)
    local orientation = entity.orientation or "Vertical"
    local x, y = entity.x or 0, entity.y or 0
    local width = entity.width or 24
    local height = entity.height or 24
    local collectables = entity.requires or 0
    local mode = entity.mode or "TotalHearts"
    local edges = entity.edges or "All"
    local edgesColor = entity.edgesColor or "FFFFFF"
    local iconsColor = entity.iconsColor or "FFFFFF"
    local directory = entity.directory or ""
    local iconTexture = {}
    iconTexture["heart"] = "objects/XaphanHelper/CollectableDoor/Heart/icon00"
    iconTexture["strawberries"] = "objects/XaphanHelper/CollectableDoor/Strawberry/icon00"
    iconTexture["golden"] = "objects/XaphanHelper/CollectableDoor/GoldenStrawberry/icon00"
    iconTexture["cassette"] = "objects/XaphanHelper/CollectableDoor/Cassette/icon00"
    iconTexture["flag"] = "objects/XaphanHelper/CollectableDoor/Flag/icon00"
    local icon = nil
    local edgeTexture = "objects/heartdoor/edge"
    local topTexture = "objects/heartdoor/top"
    if directory ~= "" then
        iconTexture["custom"] = directory .. "/icon00"
        icon = "custom"
        edgeTexture = directory .."/edge"
        topTexture = directory .."/top"
    else
        if string.find(entity.mode, "Heart") then
            icon = "heart"
        elseif string.find(entity.mode, "Strawberries") then
            icon = "strawberries"
        elseif string.find(entity.mode, "Golden") then
            icon = "golden"
        elseif string.find(entity.mode, "Cassette") then
            icon = "cassette"
        elseif string.find(entity.mode, "Flag") then
            icon = "flag"
        end
    end

    local edgeSpriteSample = drawableSprite.fromTexture(edgeTexture, entity)
    local topSpriteSample = drawableSprite.fromTexture(topTexture, entity)

    local collectableSpriteSample = drawableSprite.fromTexture(iconTexture[icon], entity)

    local edgeWidth, edgeHeight = edgeSpriteSample.meta.width, edgeSpriteSample.meta.height
    local topWidth, topHeight = topSpriteSample.meta.width, topSpriteSample.meta.height
    local collectableWidth, collectableHeight = collectableSpriteSample.meta.width, collectableSpriteSample.meta.height

    local doorColor = entity.interiorColor:gsub("#","")

    local rectangleSprite = nil
    if orientation == "Vertical" then
        rectangleSprite = drawableRectangle.fromRectangle("fill", x, y - height, width, height * 2, {tonumber("0x"..doorColor:sub(1,2)) / 255, tonumber("0x"..doorColor:sub(3,4)) / 255, tonumber("0x"..doorColor:sub(5,6)) / 255, 255 / 255})
    else
        rectangleSprite = drawableRectangle.fromRectangle("fill", x - width, y, width * 2, height , {tonumber("0x"..doorColor:sub(1,2)) / 255, tonumber("0x"..doorColor:sub(3,4)) / 255, tonumber("0x"..doorColor:sub(5,6)) / 255, 255 / 255})
    end

    local position = nil
    if orientation == "Vertical" then
        position = {x = x, y = y - height}
    else
        position = {x = x - width, y = y}
    end
    local sprites = {rectangleSprite}

    if orientation == "Vertical" then
        for i = 0, height * 2 - 1, edgeHeight do
            local leftSprite = drawableSprite.fromTexture(edgeTexture, position)
            local rightSprite = drawableSprite.fromTexture(edgeTexture, position)
    
            leftSprite:setJustification(0.5, 0.0)
            leftSprite:setScale(-1, 1)
            leftSprite:addPosition(edgeWidth - 1, i)
            leftSprite:setColor(edgesColor)
    
            rightSprite:setJustification(0.5, 0.0)
            rightSprite:addPosition(width - edgeWidth + 1, i)
            rightSprite:setColor(edgesColor)
            
            if edges == "All" or edges == "LeftRight" then
                table.insert(sprites, leftSprite)
                table.insert(sprites, rightSprite)
            end
        end
        for i = 0, width - 1, topWidth do
            local topSprite = drawableSprite.fromTexture(topTexture, position)
            local bottomSprite = drawableSprite.fromTexture(topTexture, position)
    
            topSprite:setJustification(0.0, 0.5)
            topSprite:addPosition(i, topHeight - 1)
            topSprite:setColor(edgesColor)

            bottomSprite:setJustification(0.0, 0.5)
            bottomSprite:setScale(1, -1)
            bottomSprite:addPosition(i, height * 2 - topWidth + 4)
            bottomSprite:setColor(edgesColor)
    
            if edges == "All" or edges == "TopBottom" then
                table.insert(sprites, topSprite)
                table.insert(sprites, bottomSprite)
            end
        end
    else
        for i = 0, height - 1, edgeHeight do
            local leftSprite = drawableSprite.fromTexture(edgeTexture, position)
            local rightSprite = drawableSprite.fromTexture(edgeTexture, position)
    
            leftSprite:setJustification(0.5, 0.0)
            leftSprite:setScale(-1, 1)
            leftSprite:addPosition(edgeWidth - 1, i)
            leftSprite:setColor(edgesColor)
    
            rightSprite:setJustification(0.5, 0.0)
            rightSprite:addPosition(width * 2 - edgeWidth + 1, i)
            rightSprite:setColor(edgesColor)
    
            if edges == "All" or edges == "LeftRight" then
                table.insert(sprites, leftSprite)
                table.insert(sprites, rightSprite)
            end
        end
        for i = 0, width * 2 - 1, topWidth do
            local topSprite = drawableSprite.fromTexture(topTexture, position)
            local bottomSprite = drawableSprite.fromTexture(topTexture, position)
    
            topSprite:setJustification(0.0, 0.5)
            topSprite:addPosition(i, topHeight - 1)
            topSprite:setColor(edgesColor)

            bottomSprite:setJustification(0.0, 0.5)
            bottomSprite:setScale(1, -1)
            bottomSprite:addPosition(i, height - topWidth + 4)
            bottomSprite:setColor(edgesColor)
    
            if edges == "All" or edges == "TopBottom" then
                table.insert(sprites, topSprite)
                table.insert(sprites, bottomSprite)
            end
        end
    end
    

    if collectables > 0 then
        local fits = nil
        if mode == "GoldenStrawberry" then
            collectables = 1
        end
        if orientation == "Vertical" then
            fits = collectablesPossible(edgeWidth, collectableWidth, width, collectables)
        else
            fits = collectablesPossible(edgeWidth, collectableWidth, width * 2, collectables)
        end
        local rows = math.ceil(collectables / fits)

        for row = 1, rows do
            local displayedcollectables = nil
            if orientation == "Vertical" then
                displayedcollectables = collectablesPossible(edgeWidth, collectableWidth, width, collectables)
            else
                displayedcollectables = collectablesPossible(edgeWidth, collectableWidth, width * 2, collectables)
            end
            local drawWidth = collectablesWidth(collectableWidth, displayedcollectables)

            local startX = nil
            local startY = nil
            if orientation == "Vertical" then
                startX = x + utils.round((width - drawWidth) / 2) + edgeWidth - 3
                startY = y - utils.round(rows / 2 * (collectableHeight + collectablePadding)) - collectablePadding - 4
            else
                startX = x - width + utils.round((width * 2 - drawWidth) / 2) + edgeWidth - 3
                startY = y - utils.round(rows / 2 * (collectableHeight + collectablePadding)) - collectablePadding + height / 2 - 4
            end

            for col = 1, displayedcollectables do
                local drawX = startX + (col - 1) * (collectableWidth + collectablePadding) - collectablePadding
                local drawY = startY + row * (collectableHeight + collectablePadding) - collectablePadding

                local sprite = drawableSprite.fromTexture(iconTexture[icon], {
                    x = drawX,
                    y = drawY
                })

                sprite:setJustification(0.0, 0.0)
                sprite:setColor(iconsColor)

                table.insert(sprites, sprite)
            end

            collectables -= displayedcollectables
        end
    end

    return sprites
end

function CollectableDoor.drawSelected(room, layer, entity, color)
    local nodes = entity.nodes

    if nodes and #nodes > 0 then
        local x, y = entity.x or 0, entity.y or 0
        local nx, ny = nodes[1].x, nodes[1].y
        local width = entity.width or 24
        local height = entity.height or 24
        local orientation = entity.orientation or "Vertical"

        if orientation == "Vertical" then
            drawing.callKeepOriginalColor(function()
                love.graphics.setColor(1.0, 0.0, 0.0, 1.0)
    
                love.graphics.rectangle("fill", x, ny, width, 1)
                love.graphics.rectangle("fill", x, 2 * y - ny, width, 1)
    
                love.graphics.rectangle("fill", nx - 8, ny, width + 16, 8)
            end)
        else
            drawing.callKeepOriginalColor(function()
                love.graphics.setColor(1.0, 0.0, 0.0, 1.0)
    
                love.graphics.rectangle("fill", nx, y, 1, height)
                love.graphics.rectangle("fill", 2 * x - nx, y, 1, height)
    
                love.graphics.rectangle("fill", nx, ny - height - 16, 8, height + 16)
            end)
        end
    end
end

function CollectableDoor.selection(room, entity)
    local nodes = entity.nodes
    local x, y = entity.x or 0, entity.y or 0
    local width = entity.width or 24
    local height = entity.height or 24
    local orientation = entity.orientation or "Vertical"

    if orientation == "Vertical" then
        local mainRectangle = utils.rectangle(x, y - height, width, height * 2)

        if nodes and #nodes > 0 then
            local nx, ny = nodes[1].x, nodes[1].y
            local nodeRectangle = utils.rectangle(nx - 8, ny, width + 16, 8)

            return mainRectangle, {nodeRectangle}
        end
        return mainRectangle
    else
        local mainRectangle = utils.rectangle(x - width, y, width * 2, height)

        if nodes and #nodes > 0 then
            local nx, ny = nodes[1].x, nodes[1].y
            local nodeRectangle = utils.rectangle(nx, ny - height - 16, 8, height + 16)

            return mainRectangle, {nodeRectangle}
        end
        return mainRectangle
    end
end

return CollectableDoor