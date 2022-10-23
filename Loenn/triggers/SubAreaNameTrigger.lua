local SubAreaNameTrigger = {}

SubAreaNameTrigger.name = "XaphanHelper/SubAreaNameTrigger"
SubAreaNameTrigger.fieldOrder = {
    "x", "y", "width", "height", "dialogID", "timer", "textPositionX", "textPositionY"
}
SubAreaNameTrigger.fieldInformation = {
    textPositionX = {
        options = {"Left", "Middle", "Right"},
        editable = false
    },
    textPositionY = {
        fieldType = "integer",
        minimumValue = 80,
        maximumValue = 1080
    }
}
SubAreaNameTrigger.placements = {
    name = "SubAreaNameTrigger",
    data = {
        dialogID = "",
        timer = 3.00,
        textPositionX = "Right",
        textPositionY = 1040
    }
}

return SubAreaNameTrigger