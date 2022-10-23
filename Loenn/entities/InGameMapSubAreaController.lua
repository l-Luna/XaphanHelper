local InGameMapSubAreaController = {}

InGameMapSubAreaController.name = "XaphanHelper/InGameMapSubAreaController"
InGameMapSubAreaController.depth = -100000
InGameMapSubAreaController.fieldOrder = {
    "x", "y", "subAreaIndex", "subAreaName", "exploredRoomColor", "unexploredRoomColor", "secretRoomColor", "heatedRoomColor", "roomBorderColor", "elevatorColor"
}
InGameMapSubAreaController.fieldInformation = {
    roomIndicatorColor = {
        subAreaIndex = "integer"
    },
    exploredRoomColor = {
        fieldType = "color"
    },
    unexploredRoomColor = {
        fieldType = "color"
    },
    secretRoomColor = {
        fieldType = "color"
    },
    heatedRoomColor = {
        fieldType = "color"
    },
    roomBorderColor = {
        fieldType = "color"
    },
    elevatorColor = {
        fieldType = "color"
    }
}
InGameMapSubAreaController.placements = {
    name = "InGameMapSubAreaController",
    data = {
        subAreaIndex = 0,
        subAreaName = "",
        exploredRoomColor = "D83890",
        unexploredRoomColor = "000080",
        secretRoomColor = "057A0C",
        heatedRoomColor = "FF650D",
        roomBorderColor = "FFFFFF",
        elevatorColor = "F80000",
    }
}

InGameMapSubAreaController.texture = "util/XaphanHelper/Loenn/subAreaController"

return InGameMapSubAreaController