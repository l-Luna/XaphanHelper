module XaphanHelperMergeChaptersController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/MergeChaptersController" MergeChaptersController(x::Integer, y::Integer, mode::String="Classic", keepPrologueSeparated::Bool=false)

const placements = Ahorn.PlacementDict(
    "Merge Chapters Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        MergeChaptersController
    )
)

function Ahorn.selection(entity::MergeChaptersController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

Ahorn.editingOptions(entity::MergeChaptersController) = Dict{String, Any}(
    "mode" => String["Classic", "Rooms", "Warps"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::MergeChaptersController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/mergeChaptersController", 0, 0)
end

end