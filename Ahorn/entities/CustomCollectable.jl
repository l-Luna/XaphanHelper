module XaphanHelperCustomCollectable

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomCollectable" CustomCollectable(x::Integer, y::Integer, width::Integer=16, height::Integer=16, sprite::String="collectables/XaphanHelper/CustomCollectable/collectable", collectSound::String="event:/game/07_summit/gem_get", changeMusic::Bool=false, newMusic::String="", flag::String="", mapIcon::String="", mustDash::Bool=false, collectGoldenStrawberry::Bool=false, endChapter::Bool=false, completeSpeedrun::Bool=false, registerInSaveData::Bool=false, ignoreGolden::Bool=false)

const placements = Ahorn.PlacementDict(
    "Custom Collectable (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomCollectable,
        "point",
        ),
    )

Ahorn.minimumSize(entity::CustomCollectable) = 16, 16
Ahorn.resizable(entity::CustomCollectable) = false, false

Ahorn.selection(entity::CustomCollectable) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CustomCollectable, room::Maple.Room)
    sprite = get(entity.data, "sprite", "collectables/XaphanHelper/CustomCollectable/collectable")
    Ahorn.drawSprite(ctx, "$(sprite)00.png", 8, 8)
end

end