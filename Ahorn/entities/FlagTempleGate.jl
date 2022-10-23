module XaphanHelperFlagTempleGate

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/FlagTempleGate" FlagTempleGate(x::Integer, y::Integer,  width::Integer=8, height::Integer=48, flag::String="", spriteName::String="default", openOnHeartCollection::Bool=false, startOpen::Bool=false, horizontal::Bool=false, attachRight::Bool=false)

const placements = Ahorn.PlacementDict(
    "Flag Temple Gate (Xaphan Helper)" => Ahorn.EntityPlacement(
        FlagTempleGate,
        "point",
        Dict{String, Any}(
            "spriteName" => "default"
        )
    ),
)

textures = String["default", "mirror", "theo"]

Ahorn.editingOptions(entity::FlagTempleGate) = Dict{String, Any}(
    "spriteName" => textures
)

function Ahorn.selection(entity::FlagTempleGate)
    x, y = Ahorn.position(entity)
    height = Int(get(entity.data, "height", 8))
    horizontal = get(entity.data, "horizontal", false)
    if horizontal
        return Ahorn.Rectangle(x, y - 3, height, 15)
    else
        return Ahorn.Rectangle(x - 3, y, 15, height)
    end
    
end

sprites = Dict{String, String}(
    "default" => "objects/door/TempleDoor00",
    "mirror" => "objects/door/TempleDoorB00",
    "theo" => "objects/door/TempleDoorC00"
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FlagTempleGate, room::Maple.Room)
    sprite = get(entity.data, "spriteName", "default")
    horizontal = get(entity.data, "horizontal", false)
    attachRight = get(entity.data, "attachRight", false)

    if haskey(sprites, sprite)
        if horizontal
            if attachRight
                Ahorn.drawSprite(ctx, sprites[sprite], 55, 21, rot=pi / 2)
            else
                Ahorn.drawSprite(ctx, sprites[sprite], 7, 36, rot=-pi / 2)
            end
        else
            Ahorn.drawSprite(ctx, sprites[sprite], 4, 24)
        end
    end
end

end