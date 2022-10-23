local InGameMapTilesController = {}
local tilesList = {"None", "Middle", "TopEdge", "BottomEdge", "LeftsideEdge", "RightsideEdge", "SingleHorizontalLeftEdge", "SingleHorizontalRightEdge", "SingleHorizontalMiddle", "SingleVerticalTopEdge", "SingleVerticalBottomEdge", "SingleVerticalMiddle", "AllWalls", "UpperLeftCorner", "UpperRightCorner", "LowerLeftCorner", "LowerRightCorner", "UpperLeftSlopeCorner", "UpperRightSlopeCorner", "LowerLeftSlopeCorner", "LowerRightSlopeCorner", "UpperLeftSlightSlopeCornerStart", "UpperLeftSlightSlopeCornerEnd", "UpperRightSlightSlopeCornerStart", "UpperRightSlightSlopeCornerEnd", "LowerLeftSlightSlopeCornerStart", "LowerLeftSlightSlopeCornerEnd", "LowerRightSlightSlopeCornerStart", "LowerRightSlightSlopeCornerEnd", "UpperLeftHalfSlopeCorner", "UpperRightHalfSlopeCorner", "LowerLeftHalfSlopeCorner", "LowerRightHalfSlopeCorner", "ElevatorShaft", "ElevatorUpAllWalls", "ElevatorDownAllWalls", "ElevatorUpSingleHorizontalLeftEdge", "ElevatorUpSingleHorizontalRightEdge", "ElevatorDownSingleHorizontalLeftEdge", "ElevatorDownSingleHorizontalRightEdge", "UpArrow", "DownArrow", "LeftArrow", "RightArrow"}

InGameMapTilesController.name = "XaphanHelper/InGameMapTilesController"
InGameMapTilesController.depth = -100000
InGameMapTilesController.fieldOrder = {
    "x", "y", "tile0Cords", "tile0", "tile1Cords", "tile1", "tile2Cords", "tile2", "tile3Cords", "tile3",
    "tile4Cords", "tile4", "tile5Cords", "tile5", "tile6Cords", "tile6", "tile7Cords", "tile7",
    "tile8Cords", "tile8", "tile9Cords", "tile9"
}
InGameMapTilesController.fieldInformation = {
    tile0 = {
        options = tilesList,
        editable = false
    },
    tile1 = {
        options = tilesList,
        editable = false
    },
    tile2 = {
        options = tilesList,
        editable = false
    },
    tile3 = {
        options = tilesList,
        editable = false
    },
    tile4 = {
        options = tilesList,
        editable = false
    },
    tile5 = {
        options = tilesList,
        editable = false
    },
    tile6 = {
        options = tilesList,
        editable = false
    },
    tile7 = {
        options = tilesList,
        editable = false
    },
    tile8 = {
        options = tilesList,
        editable = false
    },
    tile9 = {
        options = tilesList,
        editable = false
    }
}
InGameMapTilesController.placements = {
    name = "InGameMapTilesController",
    data = {
        tile0Cords = "0-0",
        tile0 = "None",
        tile1Cords = "0-0",
        tile1 = "None",
        tile2Cords = "0-0",
        tile2 = "None",
        tile3Cords = "0-0",
        tile3 = "None",
        tile4Cords = "0-0",
        tile4 = "None",
        tile5Cords = "0-0",
        tile5 = "None",
        tile6Cords = "0-0",
        tile6 = "None",
        tile7Cords = "0-0",
        tile7 = "None",
        tile8Cords = "0-0",
        tile8 = "None",
        tile9Cords = "0-0",
        tile9 = "None"
    }
}

InGameMapTilesController.texture = "util/XaphanHelper/Loenn/tilesController"

return InGameMapTilesController