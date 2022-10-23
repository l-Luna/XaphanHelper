module XaphanHelperCustomCheckpoint

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomCheckpoint" CustomCheckpoint(x::Integer, y::Integer, width::Integer=24, height::Integer=32, sprite::String="objects/XaphanHelper/CustomCheckpoint", activatedSpriteX::Number=0.00, activatedSpriteY::Number=0.00, removeBackgroundWhenActive::Bool=false, sound::String="", emitLight::Bool=false, lightColor::String="")

function swapFinalizer(entity)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))
end

const placements = Ahorn.PlacementDict(
    "Custom Checkpoint (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomCheckpoint,
        "point",
        ),
    )

Ahorn.resizable(entity::CustomCheckpoint) = false, false

function Ahorn.selection(entity::CustomCheckpoint)
    x, y = Ahorn.position(entity)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/CustomCheckpoint/ruins")
    if sprite == ""
        sprite = "objects/XaphanHelper/CustomCheckpoint/ruins"
    end
    return Ahorn.getSpriteRectangle("$(sprite)/bg00.png", x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CustomCheckpoint, room::Maple.Room)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/CustomCheckpoint/ruins")
    if sprite == ""
        sprite = "objects/XaphanHelper/CustomCheckpoint/ruins"
    end
    Ahorn.drawSprite(ctx, "$(sprite)/bg00.png", 0, 0)
end

end