module XaphanHelperTimedDashSwitch

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TimedDashSwitch" TimedDashSwitch(x::Integer, y::Integer, width::Integer=16, height::Integer=8, side::String="Up", spriteName::String="default", timer::Integer=10, mode::String="add", tickingType::String="Tick only", particleColor1::String="99E550", particleColor2::String="D9FFB5", flag::String="", inWall::Bool=false)

const placements = Ahorn.PlacementDict(
    "Timed Dash Switch (Xaphan Helper)" => Ahorn.EntityPlacement(
        TimedDashSwitch,
        "point",
        ),
    )

function Ahorn.selection(entity::TimedDashSwitch)
    x, y = Ahorn.position(entity)
    side = get(entity.data, "side", "Up")
    inWall = get(entity.data, "inWall", false)
    if side == "Left"
        return Ahorn.Rectangle(x - (inWall ? 4 : 0), y, 10, 16)
    end
    if side == "Right"
        return Ahorn.Rectangle(x - 2 + (inWall ? 4 : 0), y, 10, 16)
    end
    if side == "Up"
        return Ahorn.Rectangle(x, y - 4 + (inWall ? 4 : 0), 16, 12)
    end
    if side == "Down"
        return Ahorn.Rectangle(x, y - (inWall ? 4 : 0), 16, 12)
    end
end

Ahorn.editingOptions(entity::TimedDashSwitch) = Dict{String, Any}(
    "side" => String["Left", "Right", "Up", "Down"],
    "mode" => String["Add", "Set"],
    "tickingType" => String["On top", "Tick only", "None"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TimedDashSwitch, room::Maple.Room)
    sprite = get(entity.data, "spriteName", "default")
    side = get(entity.data, "side", "Up")
    inWall = get(entity.data, "inWall", false)
    texture = sprite == "default" ? "objects/temple/dashButton00.png" : "objects/temple/dashButtonMirror00.png"
    if side == "Left"
        Ahorn.drawSprite(ctx, texture, inWall ? 16 : 20, 26, rot=pi)
    end
    if side == "Right"
        Ahorn.drawSprite(ctx, texture, inWall ? 12 : 8, 8)
    end
    if side == "Up"
        Ahorn.drawSprite(ctx, texture, 27, inWall ? 11 : 7, rot=pi / 2)
    end
    if side == "Down"
        Ahorn.drawSprite(ctx, texture, 9, inWall ? 16 : 20, rot=-pi / 2)
    end
end

end