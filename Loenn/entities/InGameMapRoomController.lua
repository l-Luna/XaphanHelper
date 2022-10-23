local InGameMapRoomController = {}
local entrancesPositions = {"None", "Left", "Right", "Top", "Bottom"}

InGameMapRoomController.name = "XaphanHelper/InGameMapRoomController"
InGameMapRoomController.depth = -100000
InGameMapRoomController.fieldOrder = {
    "x", "y", "entrance0Cords", "entrance0Position", "entrance1Cords", "entrance1Position",
    "entrance2Cords", "entrance2Position", "entrance3Cords", "entrance3Position", "entrance4Cords", "entrance4Position",
    "entrance5Cords", "entrance5Position", "entrance6Cords", "entrance6Position", "entrance7Cords", "entrance7Position",
    "entrance8Cords", "entrance8Position", "entrance9Cords", "entrance9Position", "showUnexplored", "mapShardIndex", "secret", "subAreaIndex"
}
InGameMapRoomController.fieldInformation = {
    entrance0Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance1Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance2Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance3Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance4Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance5Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance6Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance7Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance8Position = {
        options = entrancesPositions,
        editable = false
    },
    entrance9Position = {
        options = entrancesPositions,
        editable = false
    },
    mapShardIndex = {
        fieldType = "integer"
    }
    ,
    subAreaIndex = {
        fieldType = "integer"
    }
}
InGameMapRoomController.placements = {
    name = "InGameMapRoomController",
    data = {
        showUnexplored = false,
        secret = false,
        mapShardIndex = 0,
        entrance0Position = "None",
        entrance0Cords = "0-0",
        entrance1Position = "None",
        entrance1Cords = "0-0",
        entrance2Position = "None",
        entrance2Cords = "0-0",
        entrance3Position = "None",
        entrance3Cords = "0-0",
        entrance4Position = "None",
        entrance4Cords = "0-0",
        entrance5Position = "None",
        entrance5Cords = "0-0",
        entrance6Position = "None",
        entrance6Cords = "0-0",
        entrance7Position = "None",
        entrance7Cords = "0-0",
        entrance8Position = "None",
        entrance8Cords = "0-0",
        entrance9Position = "None",
        entrance9Cords = "0-0",
        subAreaIndex = 0
    }
}

InGameMapRoomController.texture = "util/XaphanHelper/Loenn/roomController"

return InGameMapRoomController