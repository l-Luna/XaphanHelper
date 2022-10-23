module XaphanHelperBubbleDoor

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/BubbleDoor" BubbleDoor(x::Integer, y::Integer, width::Integer=8, height::Integer=40, side::String="Left", directory::String="objects/XaphanHelper/BubbleDoor", color::String="Blue", flags::String="", forceLockedFlag::String="", openSound::String="", closeSound::String="",unlockSound::String="", lockSound::String="")

const placements = Ahorn.PlacementDict(
    "Bubble Door (Xaphan Helper)" => Ahorn.EntityPlacement(
        BubbleDoor,
        "point",
        ),
    )

    function Ahorn.selection(entity::BubbleDoor)
        x, y = Ahorn.position(entity)
        side = get(entity.data, "side", "Left")
        if side == "Left"
            return Ahorn.Rectangle(x, y, 8, 40)
        end
        if side == "Right"
            return Ahorn.Rectangle(x, y, 8, 40)
        end
        if side == "Top"
            return Ahorn.Rectangle(x, y, 40, 8)
        end
        if side == "Bottom"
            return Ahorn.Rectangle(x, y, 40, 8)
        end
    end

Ahorn.editingOptions(entity::BubbleDoor) = Dict{String, Any}(
    "side" => String["Left", "Right", "Top", "Bottom"],
    "color" => String["Blue", "Red", "Green", "Yellow", "Grey"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BubbleDoor, room::Maple.Room)
    side = get(entity.data, "side", "Left")
    color = get(entity.data, "color", "Blue")
    directory= get(entity.data, "directory", "objects/XaphanHelper/BubbleDoor")
    if side == "Left"
        Ahorn.drawSprite(ctx, "$(directory)/$(color)/closed00.png", 3, 20)
        Ahorn.drawSprite(ctx, "$(directory)/struct00.png", -4, 20)
    end
    if side == "Right"
        Ahorn.drawSprite(ctx, "$(directory)/$(color)/closed00.png", 11, 60, rot=pi)
        Ahorn.drawSprite(ctx, "$(directory)/struct00.png", 20, 60, rot=pi)
    end
    if side == "Top" 
        Ahorn.drawSprite(ctx, "$(directory)/$(color)/closed00.png", 43, 20, rot=pi/2)
        Ahorn.drawSprite(ctx, "$(directory)/struct00.png", 44, 12, rot=pi/2)
    end
    if side == "Bottom"
        Ahorn.drawSprite(ctx, "$(directory)/$(color)/closed00.png", 3, 28, rot=-pi/2)
        Ahorn.drawSprite(ctx, "$(directory)/struct00.png", 4, 36, rot=-pi/2)
    end
end

end