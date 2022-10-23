module XaphanHelperTimedTempleGate

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TimedTempleGate" TimedTempleGate(x::Integer, y::Integer,  width::Integer=8, height::Integer=48, startOpen::Bool=false, spriteName::String="default")

const placements = Ahorn.PlacementDict(
    "Timed Temple Gate (Xaphan Helper)" => Ahorn.EntityPlacement(
        TimedTempleGate,
        "point",
        Dict{String, Any}(
            "spriteName" => "default"
        )
    ),
)

textures = String["default", "mirror", "theo"]

Ahorn.editingOptions(entity::TimedTempleGate) = Dict{String, Any}(
    "spriteName" => textures
)

function Ahorn.selection(entity::TimedTempleGate)
    x, y = Ahorn.position(entity)
    height = Int(get(entity.data, "height", 8))

    return Ahorn.Rectangle(x - 4, y, 15, height)
end

sprites = Dict{String, String}(
    "default" => "objects/door/TempleDoor00",
    "mirror" => "objects/door/TempleDoorB00",
    "theo" => "objects/door/TempleDoorC00"
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TimedTempleGate, room::Maple.Room)
    sprite = get(entity.data, "spriteName", "default")

    if haskey(sprites, sprite)
        Ahorn.drawImage(ctx, sprites[sprite], -4, 0)
    end
end

end