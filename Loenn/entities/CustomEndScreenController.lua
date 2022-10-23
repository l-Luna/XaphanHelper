local CustomEndScreenController = {}

CustomEndScreenController.name = "XaphanHelper/CustomEndScreenController"
CustomEndScreenController.depth = -100000
CustomEndScreenController.fieldOrder = {
    "x", "y", "atlas", "images", "title", "showTitle", "subText1", "subText1Color", "subText2", "subText2Color", "music", "hideVanillaTimer", "requiredTime", "showTime", "requiredStrawberries",
    "showStrawberries", "strawberriesColor", "strawberriesMaxColor", "requiredItemPercent", "showItemPercent", "itemPercentColor", "itemPercentMaxColor", "requiredMapPercent", "showMapPercent",
    "mapPercentColor", "mapPercentMaxColor", "requiredFlags", "requirementsCheck", "priority"
}
CustomEndScreenController.fieldInformation = {
    subText1Color = {
        fieldType = "color"
    },
    subText2Color = {
        fieldType = "color"
    },
    requiredTime = {
        fieldType = "integer",
        minimumValue = 0
    },
    requiredStrawberries = {
        fieldType = "integer",
        minimumValue = 0
    },
    strawberriesColor = {
        fieldType = "color"
    },
    strawberriesMaxColor = {
        fieldType = "color"
    },
    requiredItemPercent = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 100
    },
    itemPercentColor = {
        fieldType = "color"
    },
    itemPercentMaxColor = {
        fieldType = "color"
    },
    requiredMapPercent = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 100
    },
    mapPercentColor = {
        fieldType = "color"
    },
    mapPercentMaxColor = {
        fieldType = "color"
    },
    requirementsCheck = {
        options = { "Chapter", "Campaign" },
        editable = false
    },
    priority = {
        fieldType = "integer",
        minimumValue = 0
    },
}
CustomEndScreenController.placements = {
    name = "CustomEndScreenController",
    data = {
        atlas = "",
        images = "",
        title = "",
        showTitle = true,
        subText1 = "",
        subText1Color = "FFFFFF",
        subText2 = "",
        subText2Color = "FFFFFF",
        music = "",
        hideVanillaTimer = false,
        requiredTime = 0,
        showTime = false,
        requiredStrawberries = 0,
        showStrawberries = false,
        strawberriesColor = "FFFFFF",
        strawberriesMaxColor = "FFD700",
        requiredItemPercent = 0,
        showItemPercent = false,
        itemPercentColor = "FFFFFF",
        itemPercentMaxColor = "FFD700",
        requiredMapPercent = 0,
        showMapPercent = false,
        mapPercentColor = "FFFFFF",
        mapPercentMaxColor = "FFD700",
        requiredFlags = "",
        requirementsCheck = "Chapter",
        priority = 0
    }
}

CustomEndScreenController.texture = "util/XaphanHelper/Loenn/customEndScreenController"

return CustomEndScreenController