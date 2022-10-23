module XaphanHelperCustomTorch

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomTorch" CustomTorch(x::Integer, y::Integer, width::Integer=16, height::Integer=16, playLitSound::Bool=false, startLit::Bool=false, flag::String="", color::String="ffa500", sprite::String="objects/XaphanHelper/CustomTorch/torch", alpha::Number=1.00, startFade::Integer=48, endFade::Integer=64, sound::String="event:/game/05_mirror_temple/torch_activate")

const placements = Ahorn.PlacementDict(
    "Custom Torch (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomTorch,
        "point",
        ),
    )

Ahorn.selection(entity::CustomTorch) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CustomTorch, room::Maple.Room)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/CustomTorch/torch")
    Ahorn.drawSprite(ctx, "$(sprite)00.png", 8, 8)
end

end