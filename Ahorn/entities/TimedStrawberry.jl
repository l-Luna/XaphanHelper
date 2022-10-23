module XaphanHelperTimedStrawberry

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TimedStrawberry" TimedStrawberry(x::Integer, y::Integer, keepEvenIfTimerRunOut::Bool=false, order::Integer=-1, checkpointID::Integer=-1, moon::Bool=false)

const placements = Ahorn.PlacementDict(
    "Timed Strawberry (Xaphan Helper)" => Ahorn.EntityPlacement(
        TimedStrawberry
    )
)

function Ahorn.selection(entity::TimedStrawberry)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle("collectables/strawberry/normal00", x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TimedStrawberry, room::Maple.Room) = Ahorn.drawSprite(ctx, "collectables/strawberry/normal00", 0, 0)

end