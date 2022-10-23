module XaphanHelperDroneGate

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/DroneGate" DroneGate(x::Integer, y::Integer, side::String="Left", directory::String="objects/XaphanHelper/DroneGate", flag::String="")

const placements = Ahorn.PlacementDict(
    "Drone Gate (Xaphan Helper)" => Ahorn.EntityPlacement(
        DroneGate,
        "point",
        ),
    )

Ahorn.resizable(entity::DroneGate) = false, false

function Ahorn.selection(entity::DroneGate)
    x, y = Ahorn.position(entity)
    side = get(entity.data, "side", "Left")
    width = 8
    height = 8
    if side == "Left"
        y -= 8
        width = 8
        height = 24
    end
    if side == "Right"
        y -= 8
        width = 8
        height = 24
    end
    if side == "Top"
        x -= 8
        width = 24
        height = 8
    end
    if side == "Bottom"
        x -= 8
        width = 24
        height = 8
    end
    return Ahorn.Rectangle(x, y, width, height)
end

Ahorn.editingOptions(entity::DroneGate) = Dict{String, Any}(
    "side" => String["Left", "Right", "Top", "Bottom"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DroneGate, room::Maple.Room)
    side = get(entity.data, "side", "Left")
    directory = get(entity.data, "directory", "objects/XaphanHelper/DroneGate")

    if side == "Left"
        Ahorn.drawSprite(ctx, "$(directory)/closed00.png", 12, 20, rot=-pi / 2)
        Ahorn.drawSprite(ctx, "$(directory)/redL00.png", 4, 4)
    end
    if side == "Right"
        Ahorn.drawSprite(ctx, "$(directory)/closed00.png", 20, -4, rot=pi / 2)
        Ahorn.drawSprite(ctx, "$(directory)/redR00.png", 4, 4)
    end
    if side == "Top"
        Ahorn.drawSprite(ctx, "$(directory)/closed00.png", 4, 4)
        Ahorn.drawSprite(ctx, "$(directory)/redT00.png", 4, 4)
    end
    if side == "Bottom"
        Ahorn.drawSprite(ctx, "$(directory)/closed00.png", 28, 12, rot=pi)
        Ahorn.drawSprite(ctx, "$(directory)/redB00.png", 4, 4)
    end
end

end