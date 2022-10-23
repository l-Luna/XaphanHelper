local InGameMapController = {}

InGameMapController.name = "XaphanHelper/InGameMapController"
InGameMapController.depth = -100000
InGameMapController.fieldOrder = {
    "x", "y", "exploredRoomColor", "unexploredRoomColor", "secretRoomColor", "heatedRoomColor", "roomBorderColor", "elevatorColor", "gridColor", "mapName", "revealUnexploredRooms", "hideIconsInUnexploredRooms", "requireMapUpgradeToOpen", "showProgress", "progressColor", "progressCompleteColor", "customCollectablesProgress", "secretsCustomCollectablesProgress", "hideMapProgress", "hideStrawberryProgress", "hideMoonberryProgress", "hideUpgradeProgress", "hideHeartProgress", "hideCassetteProgress", "worldmapOffsetX", "worldmapOffsetY"
}
InGameMapController.fieldInformation = {
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
    },
    gridColor = {
        fieldType = "color"
    },
    showProgress = {
        options = { "Always", "AfterChapterComplete", "AfterCampaignComplete", "Never" },
        editable = false
    },
    progressColor = {
        fieldType = "color"
    },
    progressCompleteColor = {
        fieldType = "color"
    },
    worldmapOffsetX = {
        fieldType = "integer"
    },
    worldmapOffsetY = {
        fieldType = "integer"
    }
}
InGameMapController.placements = {
    name = "InGameMapController",
    data = {
        exploredRoomColor = "D83890",
        unexploredRoomColor = "000080",
        secretRoomColor = "057A0C",
        heatedRoomColor = "FF650D",
        roomBorderColor = "FFFFFF",
        elevatorColor = "F80000",
        gridColor = "262626",
        mapName = "",
        revealUnexploredRooms = false,
        requireMapUpgradeToOpen = false,
        showProgress = "Always",
        hideMapProgress = false,
        hideStrawberryProgress = false,
        hideMoonberryProgress = false,
        hideUpgradeProgress = false,
        hideHeartProgress = false,
        hideCassetteProgress = false,
        hideIconsInUnexploredRooms = false,
        customCollectablesProgress = "",
        secretsCustomCollectablesProgress = "",
        progressColor="FFFFFF",
        progressCompleteColor="FFD700",
        worldmapOffsetX= 0,
        worldmapOffsetY= 0
    }
}

InGameMapController.texture = "util/XaphanHelper/Loenn/mapController"

return InGameMapController