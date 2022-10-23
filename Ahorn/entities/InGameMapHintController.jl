module XaphanHelperInGameMapHintController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/InGameMapHintController" InGameMapHintController(x::Integer, y::Integer, displayFlags::String="", hideFlag::String="", tileCords::String="0-0", removeWhenReachedByPlayer::Bool=false, type::String="Target", direction::String="Up")

const placements = Ahorn.PlacementDict(
    "In-Game Map Hint Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        InGameMapHintController
    )
)

function Ahorn.selection(entity::InGameMapHintController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

Ahorn.editingOptions(entity::InGameMapHintController) = Dict{String, Any}(
    "type" => String["Target", "Arrow"],
    "direction" => String["Left", "Right", "Up", "Down"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InGameMapHintController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/hintController", 0, 0)
end

end