module XaphanHelperInGameMapTilesController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapTilesController" InGameMapTilesController(x::Integer, y::Integer, tile0Cords::String="0-0", tile0::String="None",tile1Cords::String="0-0", tile1::String="None",tile2Cords::String="0-0", tile2::String="None",tile3Cords::String="0-0", tile3::String="None",tile4Cords::String="0-0", tile4::String="None",tile5Cords::String="0-0", tile5::String="None",tile6Cords::String="0-0", tile6::String="None",tile7Cords::String="0-0", tile7::String="None",tile8Cords::String="0-0", tile8::String="None",tile9Cords::String="0-0", tile9::String="None")

const placements = Ahorn.PlacementDict(
    "In-Game Map Tiles Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapTilesController
    )
)

function Ahorn.selection(entity::InGameMapTilesController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

tilesList = String["None", "Middle", "TopEdge", "BottomEdge", "LeftsideEdge", "RightsideEdge", "SingleHorizontalLeftEdge", "SingleHorizontalRightEdge", "SingleHorizontalMiddle", "SingleVerticalTopEdge", "SingleVerticalBottomEdge", "SingleVerticalMiddle", "AllWalls", "UpperLeftCorner", "UpperRightCorner", "LowerLeftCorner", "LowerRightCorner", "UpperLeftSlopeCorner", "UpperRightSlopeCorner", "LowerLeftSlopeCorner", "LowerRightSlopeCorner", "UpperLeftSlightSlopeCornerStart", "UpperLeftSlightSlopeCornerEnd", "UpperRightSlightSlopeCornerStart", "UpperRightSlightSlopeCornerEnd", "LowerLeftSlightSlopeCornerStart", "LowerLeftSlightSlopeCornerEnd", "LowerRightSlightSlopeCornerStart", "LowerRightSlightSlopeCornerEnd", "UpperLeftHalfSlopeCorner", "UpperRightHalfSlopeCorner", "LowerLeftHalfSlopeCorner", "LowerRightHalfSlopeCorner", "ElevatorShaft", "ElevatorUpAllWalls", "ElevatorDownAllWalls", "ElevatorUpSingleHorizontalLeftEdge", "ElevatorUpSingleHorizontalRightEdge", "ElevatorDownSingleHorizontalLeftEdge", "ElevatorDownSingleHorizontalRightEdge", "UpArrow", "DownArrow", "LeftArrow", "RightArrow"]

Ahorn.editingOptions(entity::InGameMapTilesController) = Dict{String, Any}(
    "tile0" => tilesList,
    "tile1" => tilesList,
    "tile2" => tilesList,
    "tile3" => tilesList,
    "tile4" => tilesList,
    "tile5" => tilesList,
    "tile6" => tilesList,
    "tile7" => tilesList,
    "tile8" => tilesList,
    "tile9" => tilesList
)

Ahorn.editingOrder(entity::InGameMapTilesController) = String["x", "y", "tile0Cords", "tile0", "tile1Cords", "tile1", "tile2Cords", "tile2", "tile3Cords", "tile3",
"tile4Cords", "tile4", "tile5Cords", "tile5", "tile6Cords", "tile6", "tile7Cords", "tile7",
"tile8Cords", "tile8", "tile9Cords", "tile9"]

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapTilesController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/tilesController", 0, 0)
end

end