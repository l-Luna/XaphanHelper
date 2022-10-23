module XaphanHelperCustomExitBlock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomExitBlock" CustomExitBlock(x::Integer, y::Integer, width::Integer=8, height::Integer=8, tiletype::String="3", closeSound::Bool=false, group::Number=0)

const placements = Ahorn.PlacementDict(
    "Custom Exit Block (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomExitBlock,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::CustomExitBlock) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::CustomExitBlock) = 8, 8
Ahorn.resizable(entity::CustomExitBlock) = true, true

Ahorn.selection(entity::CustomExitBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::CustomExitBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end