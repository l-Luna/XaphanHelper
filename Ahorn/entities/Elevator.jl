module XaphanHelperElevator

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/Elevator" Elevator(x::Integer, y::Integer,  width::Integer=32, height::Integer=8, sprite::String="objects/XaphanHelper/Elevator", endPosition::Integer=0, canTalk::Bool=false, usableInSpeedrunMode::Bool=false, timer::Number=1.00, endAreaEntrance::Bool=false, toChapter::Integer =0, destinationRoom::String="", spawnRoomX::Integer=0, spawnRoomY::Integer=0, oneUse::Bool=false, flag::String="")

const placements = Ahorn.PlacementDict(
    "Elevator (Xaphan Helper)" => Ahorn.EntityPlacement(
        Elevator,
        "rectangle",
        Dict{String, Any}()
    ),
)

Ahorn.minimumSize(entity::Elevator) = 32, 8
Ahorn.resizable(entity::Elevator) = false, false

Ahorn.selection(entity::Elevator) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Elevator, room::Maple.Room)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/Elevator")
    if sprite == ""
        sprite = "objects/XaphanHelper/Elevator"
    end
    Ahorn.drawSprite(ctx, "$(sprite)/elevator00.png", 16, 4)
end

end