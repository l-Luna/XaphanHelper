module XaphanHelperFlagBlock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/FlagBlock" FlagBlock(x::Integer, y::Integer, width::Integer=8, height::Integer=8, mode::String="Block", tiletype::String="3", flags::String="")

const placements = Ahorn.PlacementDict(
    "Flag Block (Xaphan Helper)" => Ahorn.EntityPlacement(
        FlagBlock,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::FlagBlock) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions(),
    "mode" => String["Block", "Wall"]
)

Ahorn.minimumSize(entity::FlagBlock) = 8, 8
Ahorn.resizable(entity::FlagBlock) = true, true

Ahorn.selection(entity::FlagBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::FlagBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end