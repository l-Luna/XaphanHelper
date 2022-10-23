module XaphanHelperMagneticCeiling

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/MagneticCeiling" MagneticCeiling(x::Integer, y::Integer, width::Integer=8, directory::String="objects/XaphanHelper/MagneticCeiling", animationSpeed::Number=0.20, canJump::Bool=false, noStaminaDrain::Bool=false)

const placements = Ahorn.PlacementDict(
   "Magnetic Ceiling (Xaphan Helper)" => Ahorn.EntityPlacement(
	MagneticCeiling,
		"rectangle",
		Dict{String, Any}()
	),
)

Ahorn.minimumSize(entity::MagneticCeiling) = 8, 8
Ahorn.resizable(entity::MagneticCeiling) =  true, false

function Ahorn.selection(entity::MagneticCeiling)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))

    return return Ahorn.Rectangle(x, y, width, 8)
end

function renderCeiling(ctx::Ahorn.Cairo.CairoContext, x::Number, y::Number, width::Number, directory::String)
    frameA = "$(directory)/idle_a00"
    frameB = "$(directory)/idle_b00"
    
    tilesWidth = div(width, 8)

    for i in 1:tilesWidth
        if iseven(i)
            Ahorn.drawImage(ctx, frameB, x + (i - 1) * 8, y, 0, 0, 8, 8)
        else
            Ahorn.drawImage(ctx, frameA, x + (i - 1) * 8, y, 0, 0, 8, 8)
        end
    end
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::MagneticCeiling, room::Maple.Room)
    directory = get(entity.data, "directory", "objects/XaphanHelper/MagneticCeiling")
    startX, startY = Int(entity.data["x"]), Int(entity.data["y"])

    width = Int(get(entity.data, "width", 8))

    renderCeiling(ctx, startX, startY, width, directory)
end

end