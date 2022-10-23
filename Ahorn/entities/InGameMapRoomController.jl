module XaphanHelperInGameMapRoomController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapRoomController" InGameMapRoomController(x::Integer, y::Integer, showUnexplored::Bool=false, secret::Bool=false, entrance0Position::String="None", entrance0Cords::String="0-0", entrance1Position::String="None", entrance1Cords::String="0-0", entrance2Position::String="None", entrance2Cords::String="0-0", entrance3Position::String="None", entrance3Cords::String="0-0", entrance4Position::String="None", entrance4Cords::String="0-0", entrance5Position::String="None", entrance5Cords::String="0-0", entrance6Position::String="None", entrance6Cords::String="0-0", entrance7Position::String="None", entrance7Cords::String="0-0", entrance8Position::String="None", entrance8Cords::String="0-0", entrance9Position::String="None", entrance9Cords::String="0-0", mapShardIndex::Number=0, subAreaIndex::Number=0)

const placements = Ahorn.PlacementDict(
    "In-Game Map Room Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapRoomController
    )
)

function Ahorn.selection(entity::InGameMapRoomController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

positions = String["None", "Left", "Right", "Top", "Bottom"]

Ahorn.editingOptions(entity::InGameMapRoomController) = Dict{String, Any}(
    "entrance0Position" => positions,
    "entrance1Position" => positions,
    "entrance2Position" => positions,
    "entrance3Position" => positions,
    "entrance4Position" => positions,
    "entrance5Position" => positions,
    "entrance6Position" => positions,
    "entrance7Position" => positions,
    "entrance8Position" => positions,
    "entrance9Position" => positions
)

Ahorn.editingOrder(entity::InGameMapRoomController) = String["x", "y", "entrance0Cords", "entrance0Position", "entrance1Cords", "entrance1Position",
"entrance2Cords", "entrance2Position", "entrance3Cords", "entrance3Position", "entrance4Cords", "entrance4Position",
"entrance5Cords", "entrance5Position", "entrance6Cords", "entrance6Position", "entrance7Cords", "entrance7Position",
"entrance8Cords", "entrance8Position", "entrance9Cords", "entrance9Position", "mapShardIndex", "showUnexplored", "secret", "subAreaIndex"]

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapRoomController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/roomController", 0, 0)
end

end