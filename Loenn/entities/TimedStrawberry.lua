local TimedStrawberry = {}

TimedStrawberry.name = "XaphanHelper/TimedStrawberry"
TimedStrawberry.depth = 0
TimedStrawberry.fieldInformation = {
    order = {
        fieldType = "integer",
    },
    checkpointID = {
        fieldType = "integer",
    }
}
TimedStrawberry.placements = {
    name = "TimedStrawberry",
    data = {
        keepEvenIfTimerRunOut = false,
        order = -1,
        checkpointID = -1,
        moon = false
    }
}

TimedStrawberry.texture = "collectables/strawberry/normal00"

return TimedStrawberry