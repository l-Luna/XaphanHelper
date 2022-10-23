module XaphanHelperCustomEndScreenController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomEndScreenController" CustomEndScreenController(x::Integer, y::Integer, atlas::String="", images::String="", title::String="", showTitle::Bool=true, subText1::String="", subText1Color::String="FFFFFF", subText2::String="", subText2Color::String="FFFFFF", music::String="", hideVanillaTimer::Bool=false, requiredTime::Integer=0, showTime::Bool=false, requiredStrawberries::Integer=0, showStrawberries::Bool=false, strawberriesColor::String="FFFFFF", strawberriesMaxColor::String="FFD700", requiredItemPercent::Integer=0, showItemPercent::Bool=false, itemPercentColor::String="FFFFFF", itemPercentMaxColor::String="FFD700", requiredMapPercent::Integer=0, showMapPercent::Bool=false, mapPercentColor::String="FFFFFF", mapPercentMaxColor::String="FFD700", requiredFlags::String="", requirementsCheck::String="Chapter", priority::Integer=0)

const placements = Ahorn.PlacementDict(
    "Custom End Screen Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomEndScreenController
    )
)

function Ahorn.selection(entity::CustomEndScreenController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

Ahorn.editingOptions(entity::CustomEndScreenController) = Dict{String, Any}(
    "requirementsCheck" => String["Chapter", "Campaign"]
)

Ahorn.editingOrder(entity::CustomEndScreenController) = String["x", "y", "atlas", "images", "title", "showTitle", "subText1", "subText1Color", "subText2", "subText2Color", "music", "hideVanillaTimer", "requiredTime", "showTime", "requiredStrawberries",
"showStrawberries", "strawberriesColor", "strawberriesMaxColor", "requiredItemPercent", "showItemPercent", "itemPercentColor", "itemPercentMaxColor", "requiredMapPercent", "showMapPercent",
"mapPercentColor", "mapPercentMaxColor", "requiredFlags", "requirementsCheck", "priority"]

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CustomEndScreenController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/customEndScreenController", 0, 0)
end

end