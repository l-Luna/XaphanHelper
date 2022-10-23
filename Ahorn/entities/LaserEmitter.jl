module XaphanHelperLaserEmitter

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/LaserEmitter" LaserEmitter(x::Integer, y::Integer, flag::String="", side::String="Right", type::String="Kill", directory::String="objects/XaphanHelper/LaserEmitter", inverted::Bool=false, base::Bool=true, noBeam::Bool=false)

const placements = Ahorn.PlacementDict(
    "Laser Emitter (Xaphan Helper)" => Ahorn.EntityPlacement(
        LaserEmitter,
        "point",
        ),
    )


Ahorn.resizable(entity::LaserEmitter) = false, false

function Ahorn.selection(entity::LaserEmitter)
    x, y = Ahorn.position(entity)
    side = get(entity.data, "side", "Left")
    base = get(entity.data, "base", true)
    if side == "Left"
        return Ahorn.Rectangle(x, y, 16, 8)
    end
    if side == "Right"
        return Ahorn.Rectangle(x - 8, y, 16, 8)
    end
    if side == "Top"
        return Ahorn.Rectangle(x, y, 8, 16)
    end
    if side == "Bottom"
        return Ahorn.Rectangle(x, y - 8, 8, 16)
    end
end

Ahorn.editingOptions(entity::LaserEmitter) = Dict{String, Any}(
    "side" => String["Left", "Right", "Top", "Bottom"],
    "type" => String["Kill", "Must Dash", "No Dash"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::LaserEmitter, room::Maple.Room)
    side = get(entity.data, "side", "Right")
    directory = get(entity.data, "directory", "objects/XaphanHelper/LaserEmitter")
    base = get(entity.data, "base", true)

    if side == "Left"
        Ahorn.drawSprite(ctx, "$(directory)/idle00.png", 4, 12, rot=-pi / 2)
        if base
            Ahorn.drawSprite(ctx, "$(directory)/baseActive$(side)00.png", 12, 4)
        end
    end
    if side == "Right"
        Ahorn.drawSprite(ctx, "$(directory)/idle00.png", 12, 4, rot=pi / 2)
        if base
            Ahorn.drawSprite(ctx, "$(directory)/baseActive$(side)00.png", -4, 4)
        end
    end
    if side == "Bottom"
        Ahorn.drawSprite(ctx, "$(directory)/idle00.png", 12, 12, rot=pi)
        if base
            Ahorn.drawSprite(ctx, "$(directory)/baseActive$(side)00.png", 4, -4)
        end
    end
    if side == "Top"
        Ahorn.drawSprite(ctx, "$(directory)/idle00.png", 4, 4)
        if base
            Ahorn.drawSprite(ctx, "$(directory)/baseActive$(side)00.png", 4, 12)
        end
    end
end

end