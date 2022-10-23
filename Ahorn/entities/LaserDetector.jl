module XaphanHelperLaserDetector

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/LaserDetector" LaserDetector(x::Integer, y::Integer, flag::String="", sides::String="Right", directory::String="objects/XaphanHelper/LaserDetector")

const placements = Ahorn.PlacementDict(
    "Laser Detector (Xaphan Helper)" => Ahorn.EntityPlacement(
        LaserDetector,
        "point",
        ),
    )


Ahorn.resizable(entity::LaserDetector) = false, false

function Ahorn.selection(entity::LaserDetector)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x, y, 8, 8)
end

Ahorn.editingOptions(entity::LaserDetector) = Dict{String, Any}(
    "sides" => String["Left", "Right", "Top", "Bottom", "Top + Left", "Top + Right", "Bottom + Left", "Bottom + Right", "Top + Bottom", "Left + Right", "Top + Left + Right", "Top + Bottom + Right", "Bottom + Left + Right", "Top + Bottom + Left", "Top + Bottom + Left + Right"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::LaserDetector, room::Maple.Room)
    sides = get(entity.data, "sides", "Right")
    directory = get(entity.data, "directory", "objects/XaphanHelper/LaserDetector")

    Ahorn.drawSprite(ctx, "$(directory)/baseActive00.png", 4, 4)
    if occursin("Left", sides)
        Ahorn.drawSprite(ctx, "$(directory)/sensor00.png", -4, 12, rot=-pi / 2)
    end
    if occursin("Right", sides)
        Ahorn.drawSprite(ctx, "$(directory)/sensor00.png", 20, 4, rot=pi / 2)
    end
    if occursin("Top", sides)
        Ahorn.drawSprite(ctx, "$(directory)/sensor00.png", 4, -4)
    end
    if occursin("Bottom", sides)
        Ahorn.drawSprite(ctx, "$(directory)/sensor00.png", 12, 20, rot=pi)
    end
end

end