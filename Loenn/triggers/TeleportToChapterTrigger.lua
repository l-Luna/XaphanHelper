local TeleportToChapterTrigger = {}

TeleportToChapterTrigger.name = "XaphanHelper/TeleportToChapterTrigger"
TeleportToChapterTrigger.fieldOrder = {
    "x", "y", "width", "height", "canInteract", "toChapter", "destinationRoom", "spawnRoomX", "spawnRoomY", "registerCurrentChapterAsCompelete", "wipeType", "wipeDuration"
}
TeleportToChapterTrigger.fieldInformation = {
    toChapter = {
        fieldType = "integer",
    },
    wipeType = {
        options = {"Spotlight", "Curtain", "Mountain", "Dream", "Starfield", "Wind", "Drop", "Fall", "KeyDoor", "Angled", "Heart", "Fade"},
        editable = false
    }
}
TeleportToChapterTrigger.placements = {
    name = "TeleportToChapterTrigger",
    data = {
        spawnRoomX = 0,
        spawnRoomY = 0,
        toChapter = 1,
        destinationRoom = "",
        registerCurrentChapterAsCompelete= false,
        wipeType = "Fade",
        wipeDuration = 1.35,
        canInteract = false
    }
}

return TeleportToChapterTrigger