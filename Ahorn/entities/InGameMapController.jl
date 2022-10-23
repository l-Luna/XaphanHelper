module XaphanHelperInGameMapController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapController" InGameMapController(x::Integer, y::Integer, exploredRoomColor::String="D83890", unexploredRoomColor::String="000080", secretRoomColor::String="057A0C", heatedRoomColor::String="FF650D", roomBorderColor::String="FFFFFF", elevatorColor::String="F80000", gridColor::String="262626", mapName::String="", revealUnexploredRooms::Bool=false, requireMapUpgradeToOpen::Bool=false, showProgress::String="Always", hideMapProgress::Bool=false, hideStrawberryProgress::Bool=false, hideMoonberryProgress::Bool=false, hideUpgradeProgress::Bool=false, hideHeartProgress::Bool=false, hideCassetteProgress::Bool=false, hideIconsInUnexploredRooms::Bool=false, customCollectablesProgress::String="", secretsCustomCollectablesProgress::String="", progressColor::String="FFFFFF", progressCompleteColor::String="FFD700")

const placements = Ahorn.PlacementDict(
    "In-Game Map Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapController
    )
)

function Ahorn.selection(entity::InGameMapController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

Ahorn.editingOrder(entity::InGameMapController) = String["x", "y", "exploredRoomColor", "unexploredRoomColor", "secretRoomColor", "heatedRoomColor", "roomBorderColor", "elevatorColor",
"gridColor", "mapName", "revealUnexploredRooms", "hideIconsInUnexploredRooms", "requireMapUpgradeToOpen", "showProgress", "progressColor", "progressCompleteColor", "customCollectablesProgress",
"secretsCustomCollectablesProgress", "hideMapProgress", "hideStrawberryProgress", "hideMoonberryProgress", "hideUpgradeProgress", "hideHeartProgress", "hideCassetteProgress"]

Ahorn.editingOptions(entity::InGameMapController) = Dict{String, Any}(
    "showProgress" => String["Always", "AfterChapterComplete", "AfterCampaignComplete", "Never"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/mapController", 0, 0)
end

end