local TimerRefill = {}

TimerRefill.name = "XaphanHelper/TimerRefill"
TimerRefill.depth = 8999
TimerRefill.fieldInformation = {
    timer = {
        fieldType = "integer",
    },
    mode = {
        options = {"add", "set"},
        editable = false
    }
}
TimerRefill.placements = {
    name = "TimerRefill",
    data = {
        oneUse = false,
        timer = 10,
        mode = "add",
        respawnTime = 2.5
    }
}

TimerRefill.texture = "objects/XaphanHelper/TimerRefill/idle00"

return TimerRefill