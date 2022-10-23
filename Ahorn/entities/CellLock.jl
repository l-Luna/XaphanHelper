module XaphanHelperCellLock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CellLock" CellLock(x::Integer, y::Integer, width::Integer=24, height::Integer=32, sprite::String="objects/XaphanHelper/CellLock", color::String="Blue", flag::String="", registerInSaveData::Bool=false, sound::String="", cellInside::Bool=false, keepCell::Bool=false, slotSound::String="event:/game/05_mirror_temple/button_activate", instant::Bool=false, type::String="Normal")

function swapFinalizer(entity)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))
end

const placements = Ahorn.PlacementDict(
    "Cell Lock Statue (Xaphan Helper)" => Ahorn.EntityPlacement(
        CellLock,
        "point",
        ),
    )

Ahorn.resizable(entity::CellLock) = false, false

Ahorn.editingOptions(entity::CellLock) = Dict{String, Any}(
    "color" => String["Blue", "Red", "Green", "Yellow", "Grey"],
    "type" => String["Normal", "Floating"]
)

function Ahorn.selection(entity::CellLock)
    x, y = Ahorn.position(entity)
    color = lowercase(get(entity.data, "color", "Blue"))
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/CellLock")
    if sprite == ""
        sprite = "objects/XaphanHelper/CellLock"
    end
    return Ahorn.getSpriteRectangle("$(sprite)/$(color)00.png", x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CellLock, room::Maple.Room)
    color = lowercase(get(entity.data, "color", "Blue"))
    cellInside = get(entity.data, "cellInside", false)
    sprite = get(entity.data, "sprite", "objects/XaphanHelper/CellLock")
    type = lowercase(get(entity.data, "type", "Normal"))
    if sprite == ""
        sprite = "objects/XaphanHelper/CellLock"
    end
    Ahorn.drawSprite(ctx, "$(sprite)/$(type)00.png", 0, 0)
    Ahorn.drawSprite(ctx, "$(sprite)/$(color)00.png", 0, 0)
    if cellInside == true
        Ahorn.drawSprite(ctx, "$(sprite)/bgCell00.png", 0, 5)
        Ahorn.drawSprite(ctx, "$(sprite)/lever00.png", 0, 5)
    end
end

end