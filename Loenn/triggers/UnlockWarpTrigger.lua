local UnlockWarpTrigger = {}

UnlockWarpTrigger.name = "XaphanHelper/UnlockWarpTrigger"
UnlockWarpTrigger.fieldOrder = {
    "x", "y", "width", "height", "chapter", "room", "index"
}
UnlockWarpTrigger.fieldInformation = {
    chapter = {
        fieldType = "integer",
        minimumValue = 0
    },
    index = {
        fieldType = "integer",
        minimumValue = 0
    }
}
UnlockWarpTrigger.placements = {
    name = "UnlockWarpTrigger",
    data = {
        chapter = 1,
        room = "",
        index = 0
    }
}

return UnlockWarpTrigger