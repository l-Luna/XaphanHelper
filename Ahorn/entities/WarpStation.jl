module XaphanHelperWarpStation

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/WarpStation" WarpStation(x::Integer, y::Integer, beamColor::String="FFFFFF", flag::String="", sprite::String="objects/XaphanHelper/WarpStation", confirmSfx::String="", index::Integer=0, wipeType::String="Fade", wipeDuration::Number=0.75, noBeam::Bool=false)

const placements = Ahorn.PlacementDict(
    "Warp Station (Xaphan Helper)" => Ahorn.EntityPlacement(
        WarpStation,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::WarpStation) = 32, 16
Ahorn.resizable(entity::WarpStation) = false, false

Ahorn.editingOptions(Trigger::WarpStation) = Dict{String, Any}(
    "wipeType" => String["Spotlight", "Curtain", "Mountain", "Dream", "Starfield", "Wind", "Drop", "Fall", "KeyDoor", "Angled", "Heart", "Fade"]
)

function Ahorn.selection(entity::WarpStation)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x, y, 32, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::WarpStation, room::Maple.Room)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/WarpStation")
    if sprite == ""
        sprite = "objects/XaphanHelper/WarpStation"
    end
    Ahorn.drawSprite(ctx, "$(sprite)/idle00.png", 16, 8)
end

end