module XaphanHelperInGameMapSubAreaController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapSubAreaController" InGameMapSubAreaController(x::Integer, y::Integer, exploredRoomColor::String="D83890", unexploredRoomColor::String="000080", secretRoomColor::String="057A0C", heatedRoomColor::String="FF650D", roomBorderColor::String="FFFFFF", elevatorColor::String="F80000", subAreaName::String="", subAreaIndex::Number=0)

const placements = Ahorn.PlacementDict(
    "In-Game Map Sub Area Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapSubAreaController
    )
)

function Ahorn.selection(entity::InGameMapSubAreaController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapSubAreaController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/subAreaController", 0, 0)
end

end