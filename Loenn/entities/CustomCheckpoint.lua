local CustomCheckpoint = {}

CustomCheckpoint.name = "XaphanHelper/CustomCheckpoint"
CustomCheckpoint.depth = 8999
CustomCheckpoint.fieldOrder = {
    "x", "y", "sprite", "siactivatedSpriteXde", "activatedSpriteY", "removeBackgroundWhenActive", "emitLight", "lightColor", "sound"
}
CustomCheckpoint.fieldInformation = {
    lightColor = {
        fieldType = "color"
    }
}
CustomCheckpoint.placements = {
    name = "CustomCheckpoint",
    data = {
        sprite = "objects/XaphanHelper/CustomCheckpoint",
        activatedSpriteX = 0.00,
        activatedSpriteY = 0.00,
        removeBackgroundWhenActive = false,
        sound = "",
        emitLight = false,
        lightColor = "FFFFFF"
    }
}

function CustomCheckpoint.texture(room, entity)
    local texture = entity.sprite or "objects/XaphanHelper/CustomCheckpoint"
  
    return texture .. "/bg00"
end

return CustomCheckpoint