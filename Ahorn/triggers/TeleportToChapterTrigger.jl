module XaphanHelperTeleportToChapterTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/TeleportToChapterTrigger" TeleportToChapterTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, spawnRoomX::Integer=0, spawnRoomY::Integer=0, toChapter::Integer=1, destinationRoom::String="", registerCurrentChapterAsCompelete::Bool=false, wipeType::String="Fade", wipeDuration::Number=1.35, canInteract::Bool=false)

const placements = Ahorn.PlacementDict(
    "Teleport to Chapter Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        TeleportToChapterTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(Trigger::TeleportToChapterTrigger) = Dict{String, Any}(
    "wipeType" => String["Spotlight", "Curtain", "Mountain", "Dream", "Starfield", "Wind", "Drop", "Fall", "KeyDoor", "Angled", "Heart", "Fade"]
)

end