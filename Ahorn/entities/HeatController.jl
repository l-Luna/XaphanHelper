module XaphanHelperHeatController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/HeatController" HeatController(x::Integer, y::Integer, maxDuration::Number = 3.00, heatEffect::Bool = false, inactiveFlag::String="")

const placements = Ahorn.PlacementDict(
    "Heat Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        HeatController
    )
)

function Ahorn.selection(entity::HeatController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HeatController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/heatController", 0, 0)
end

end