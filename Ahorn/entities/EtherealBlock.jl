module XaphanHelperEtherealBlock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/EtherealBlock" EtherealBlock(x::Integer, y::Integer, width::Integer=8, height::Integer=8, tiletype::String="3")

const placements = Ahorn.PlacementDict(
    "Ethereal Block (Xaphan Helper)" => Ahorn.EntityPlacement(
        EtherealBlock,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.minimumSize(entity::EtherealBlock) = 8, 8
Ahorn.resizable(entity::EtherealBlock) = true, true

Ahorn.selection(entity::EtherealBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::EtherealBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end