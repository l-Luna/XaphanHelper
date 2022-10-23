local drawableSprite = require("structs.drawable_sprite")

local TeleportToOtherSidePortal = {}

TeleportToOtherSidePortal.name = "XaphanHelper/TeleportToOtherSidePortal"
TeleportToOtherSidePortal.depth = 2000
TeleportToOtherSidePortal.fieldOrder = {
    "x", "y", "side", "warpSfx", "requireCassetteCollected", "requireCSideUnlocked", "flags", "teleportToStartingSpawnOfChapter", "wipeType", "wipeDuration", "registerCurrentSideAsCompelete"
}
TeleportToOtherSidePortal.canResize = {false, false}
TeleportToOtherSidePortal.fieldInformation = {
    side = {
        options = {"A-Side", "B-Side", "C-Side"},
        editable = false
    },
    wipeType = {
        options = {"Spotlight", "Curtain", "Mountain", "Dream", "Starfield", "Wind", "Drop", "Fall", "KeyDoor", "Angled", "Heart", "Fade"},
        editable = false
    }
}
TeleportToOtherSidePortal.placements = {
    name = "TeleportToOtherSidePortal",
    data = {
        side = "A-Side",
        requireCassetteCollected = false,
        requireCSideUnlocked = false,
        flags = "",
        wipeType = "Fade",
        wipeDuration = 1.35,
        warpSfx = "event:/game/xaphan/warp",
        teleportToStartingSpawnOfChapter = false,
        registerCurrentSideAsCompelete = false
    }
}

function TeleportToOtherSidePortal.sprite(room, entity)
    local texture = entity.side or "A-Side"
    local sprite = drawableSprite.fromTexture( "objects/XaphanHelper/TeleportToOtherSidePortal/" .. texture .."00", entity)

    return sprite
end

return TeleportToOtherSidePortal