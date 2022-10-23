module XaphanHelperDroneSwitch

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/DroneSwitch" DroneSwitch(x::Integer, y::Integer, flag::String="", side::String="Left")

const placements = Ahorn.PlacementDict(
    "Drone Switch (Xaphan Helper)" => Ahorn.EntityPlacement(
        DroneSwitch,
        "point",
        ),
    )

Ahorn.minimumSize(entity::DroneSwitch) = 8, 8
Ahorn.resizable(entity::DroneSwitch) = false, false

Ahorn.selection(entity::DroneSwitch) = Ahorn.getEntityRectangle(entity)

Ahorn.editingOptions(entity::DroneSwitch) = Dict{String, Any}(
    "side" => String["Left", "Right", "Down"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DroneSwitch, room::Maple.Room)
    side = get(entity.data, "side", "Left")

    if side == "Left"
    Ahorn.drawSprite(ctx, "objects/XaphanHelper/DroneSwitch/idle00.png", 12, 4, rot=pi / 2)
    end
    if side == "Right"
        Ahorn.drawSprite(ctx, "objects/XaphanHelper/DroneSwitch/idle00.png", 4, 12, rot=-pi / 2)
    end
    if side == "Down"
        Ahorn.drawSprite(ctx, "objects/XaphanHelper/DroneSwitch/idle00.png", 12, 12, rot=pi)
    end
end

end