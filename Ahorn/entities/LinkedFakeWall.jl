module XaphanHelperLinkedFakeWall

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/LinkedFakeWall" LinkedFakeWall(x::Integer, y::Integer, width::Integer=8, height::Integer=8, mode::String="Block", tiletype::String="3", playTransitionReveal::Bool=false)

const placements = Ahorn.PlacementDict(
    "Linked Fake Wall (Xaphan Helper)" => Ahorn.EntityPlacement(
        LinkedFakeWall,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::LinkedFakeWall) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions(),
    "mode" => String["Block", "Wall"]
)

Ahorn.minimumSize(entity::LinkedFakeWall) = 8, 8
Ahorn.resizable(entity::LinkedFakeWall) = true, true

Ahorn.selection(entity::LinkedFakeWall) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::LinkedFakeWall, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end