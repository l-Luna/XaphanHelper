module XaphanHelperSlope

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/Slope" Slope(x::Integer, y::Integer, side::String="Left", gentle::Bool=false, soundIndex::Integer=8, slopeHeight::Integer=1, tilesTop::String="Horizontal", tilesBottom::String="Horizontal", customDirectory::String="objects/XaphanHelper/Slope", texture::String="cement", canSlide::Bool=false, upsideDown::Bool=false, noRender::Bool=false, stickyDash::Bool=false, rainbow::Bool=false, canJumpThrough::Bool=false)

const placements = Ahorn.PlacementDict(
   "Slope (Xaphan Helper)" => Ahorn.EntityPlacement(
	Slope,
		"point",
		Dict{String, Any}()
	),
)

Ahorn.resizable(entity::Slope) =  false, false

function Ahorn.selection(entity::Slope)
    x, y = Ahorn.position(entity)
    side = get(entity.data, "side", "Left")
    gentle = get(entity.data, "gentle", false)
    slopeHeight = get(entity.data, "slopeHeight", 1)
    tilesTop = get(entity.data, "tilesTop", "Horizontal")
    tilesBottom = get(entity.data, "tilesBottom", "Horizontal")
    upsideDown= get(entity.data, "upsideDown", false)
    offsetX = 0
    offsetY = 0
    if side == "Right"
        if gentle == false
            offsetX = 8 - 8 * (slopeHeight - 1)
        end
        if gentle == true
            offsetX = 0 - 16 * (slopeHeight - 1)
        end
    end
    if side == "Left"
        if tilesTop == "Vertical"
            offsetX = 8
        end
    end
    width = 8 + 8 * slopeHeight
    if gentle
        width = 8 + 16 * slopeHeight
    end
    if upsideDown
        offsetY = 8 - 8 * (slopeHeight - 1)
        #= if tilesBottom != "Horizontal"
            offsetY = offsetY - 8
        end =#
    end
    return Ahorn.Rectangle(x + offsetX, y + offsetY, width + (tilesTop == "Vertical" ? -8 : 0), 8 * slopeHeight#= + (tilesBottom != "Horizontal" ? 8 : 0) =#)
end

Ahorn.editingOptions(entity::Slope) = Dict{String, Any}(
    "side" => String["Left", "Right"],
    "soundIndex" => Maple.tileset_sound_ids,
    "tilesTop" => String["Horizontal", "Horizontal Corner", "Vertical", "Edge", "Edge Corner", "Small Edge", "Small Edge Corner"],
    "tilesBottom" => String["Horizontal", "Vertical", "Vertical Corner", "Edge", "Edge Corner", "Small Edge", "Small Edge Corner"],
    "texture" => String["cement", "cliffside", "cliffsideAlt", "core", "deadgrass", "dirt", "girder", "grass", "lostlevels", "poolEdges", "reflection", "reflectionAlt", "rock", "scifi", "snow", "starJump", "stone", "summit", "summitNoSnow", "templeA", "templeB", "tower", "wood", "woodStoneEdges"]
)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Slope, room::Maple.Room)
    gentle = get(entity.data, "gentle", false)
    side = get(entity.data, "side", "Left")
    type = "slope"
    slopeHeight = get(entity.data, "slopeHeight", 1)
    tilesTop = get(entity.data, "tilesTop", "Horizontal")
    tilesBottom = get(entity.data, "tilesBottom", "Horizontal")
    upsideDown= get(entity.data, "upsideDown", false)
    value = 8
    spriteYAdd = 4
    if gentle
        type = "gentleSlope"
        value = 16
    end
    if side == "Right"
        value = -value
    end
    if upsideDown
        Ahorn.scale(ctx, 1, -1)
        spriteYAdd = -12
    end
    if tilesTop == "Vertical"
        Ahorn.drawSprite(ctx, "util/XaphanHelper/$(type)$(side)Half.png", 12, spriteYAdd)
    else
        Ahorn.drawSprite(ctx, "util/XaphanHelper/$(type)$(side).png", 12, spriteYAdd)
    end
    for i in 1:(slopeHeight - 1)
        Ahorn.drawSprite(ctx, "util/XaphanHelper/$(type)$(side).png", 12 + i * value, spriteYAdd + i * 8)
    end
    #= if tilesBottom != "Horizontal"
        Ahorn.drawSprite(ctx, "util/XaphanHelper/slopeSolid.png", 12 + (slopeHeight - (gentle ? 0 : 1)) * value + (gentle ? (value / 2) + (side == "Right" ? 16 : -16) : 0), spriteYAdd + slopeHeight * 8)
    end =#
end

end