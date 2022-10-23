module XaphanHelperBreakBlock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/BreakBlock" BreakBlock(x::Integer, y::Integer, width::Integer=8, height::Integer=8, mode::String="Block", tiletype::String="3", type::String="Bomb", color::String="FFFFFF", startRevealed::Bool=false, directory::String="objects/XaphanHelper/BreakBlock")

const placements = Ahorn.PlacementDict(
    "Break Block (Xaphan Helper)" => Ahorn.EntityPlacement(
        BreakBlock,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::BreakBlock) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions(),
    "mode" => String["Block", "Wall"],
    "type" => String["Bomb", "Drone", "LightningDash", "MegaBomb", "RedBooster", "ScrewAttack"]
)

Ahorn.minimumSize(entity::BreakBlock) = 8, 8
Ahorn.resizable(entity::BreakBlock) = true, true

Ahorn.selection(entity::BreakBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::BreakBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end