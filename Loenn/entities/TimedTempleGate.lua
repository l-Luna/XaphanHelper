local TimedTempleGate = {}

TimedTempleGate.name = "XaphanHelper/TimedTempleGate"
TimedTempleGate.depth = -9000
TimedTempleGate.canResize = {false, false}
TimedTempleGate.ignoredFields = {
    "_name", "_id", "width", "height"
}
TimedTempleGate.placements = {
    name = "TimedTempleGate",
    data = {
        width = 8,
        height = 48,
        startOpen = false,
        spriteName = "default"
    }
}

function TimedTempleGate.texture(room, entity)
    local sprite = entity.spriteName
    local sprites = {}
    sprites["default"] = "objects/door/TempleDoor00"
    sprites["mirror"] = "objects/door/TempleDoorB00"
    sprites["theo"] = "objects/door/TempleDoorC00"

    if sprites[sprite] ~= nil then
        TimedTempleGate.texture = sprites[sprite]
    else
        TimedTempleGate.texture = sprites["mirror"]
    end

    return TimedTempleGate.texture
end

TimedTempleGate.offset = {4, 0}

return TimedTempleGate