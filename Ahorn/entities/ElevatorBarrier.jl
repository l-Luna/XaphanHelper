module XaphanHelperElevatorBarrier

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/ElevatorBarrier" ElevatorBarrier(x::Integer, y::Integer, width::Integer=16, height::Integer=16)

const placements = Ahorn.PlacementDict(
    "Elevator Barrier (Xaphan Helper)" => Ahorn.EntityPlacement(
        ElevatorBarrier,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::ElevatorBarrier) = 8, 8
Ahorn.resizable(entity::ElevatorBarrier) =  true, true

Ahorn.selection(entity::ElevatorBarrier) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ElevatorBarrier, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.4, 0.4, 0.4, 0.8), (0.0, 0.0, 0.0, 0.0))
end

end