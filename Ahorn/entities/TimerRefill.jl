module XaphanHelperTimerRefill

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TimerRefill" TimerRefill(x::Integer, y::Integer, width::Integer=16, height::Integer=16, oneUse::Bool=false, timer::Integer=10, mode::String="add", respawnTime::Number=2.5)

const placements = Ahorn.PlacementDict(
    "Timer Refill (Xaphan Helper)" => Ahorn.EntityPlacement(
        TimerRefill,
        "point",
        ),
    )

Ahorn.minimumSize(entity::TimerRefill) = 16, 16
Ahorn.resizable(entity::TimerRefill) = false, false

Ahorn.selection(entity::TimerRefill) = Ahorn.getEntityRectangle(entity)

Ahorn.editingOptions(entity::TimerRefill) = Dict{String, Any}(
    "mode" => String["add", "set"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TimerRefill, room::Maple.Room)
    Ahorn.drawSprite(ctx, "objects/XaphanHelper/TimerRefill/idle00.png", 8, 8)
end

end