module XaphanHelperInGameMapRoomAdjustController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapRoomAdjustController" InGameMapRoomAdjustController(x::Integer, y::Integer, positionX::Int=0, positionY::Int=0, sizeX::Int=0, sizeY::Int=0, hiddenTiles::String="", removeEntrance0::Bool=false, removeEntrance1::Bool=false, removeEntrance2::Bool=false, removeEntrance3::Bool=false, removeEntrance4::Bool=false, removeEntrance5::Bool=false, removeEntrance6::Bool=false, removeEntrance7::Bool=false, removeEntrance8::Bool=false, removeEntrance9::Bool=false, ignoreIcons::Bool=false)

const placements = Ahorn.PlacementDict(
    "In-Game Map Room Adjust Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapRoomAdjustController
    )
)

function Ahorn.selection(entity::InGameMapRoomAdjustController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end


function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapRoomAdjustController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/roomAdjustController", 0, 0)
end

end