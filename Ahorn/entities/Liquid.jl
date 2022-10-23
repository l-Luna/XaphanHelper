module XaphanHelperLiquid

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/Liquid" Liquid(x::Integer, y::Integer,  width::Integer=8, height::Integer=8, lowPosition::Int=0, liquidType::String="acid", frameDelay::Number=0.15, color::String="", transparency::Number=0.65, foreground::Bool=false, riseDelay::Number=0.00, riseDistance::Number=0, riseSpeed::Number=10, riseShake::Bool=false, riseFlag::String="", riseEndFlag::String="", riseSound::Bool=false, directory::String="objects/XaphanHelper/liquid", surfaceHeight::Number=0, visualOnly::Bool=false, canSwim::Bool=false)

const placements = Ahorn.PlacementDict(
    "Liquid (Xaphan Helper)" => Ahorn.EntityPlacement(
        Liquid,
        "rectangle",
        Dict{String, Any}()
    ),
)

Ahorn.editingOptions(entity::Liquid) = Dict{String, Any}(
    "liquidType" => String["acid", "acid_b", "lava", "quicksand", "water"]
)

Ahorn.minimumSize(entity::Liquid) = 16, 8
Ahorn.resizable(entity::Liquid) = true, true

Ahorn.selection(entity::Liquid) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Liquid, room::Maple.Room)
    x = Int(get(entity.data, "x", 0))
    y = Int(get(entity.data, "y", 0))

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    liquid = get(entity.data, "liquidType", "acid")

    colorCode = get(entity.data, "color", "")
    
    if colorCode == ""
        if liquid == "acid"
            colorCode = "88C098"
        end
        if liquid == "acid_b"
            colorCode = "88C098"
        end
        if liquid == "lava"
            colorCode = "F85818"
        end
        if liquid == "quicksand"
            colorCode = "C8B078"
        end
        if liquid == "water"
            colorCode = "669CEE"
        end
    end

    if colorCode != ""
        colorRed = parse(Int, colorCode[1:2], base = 16)
        colorGreen = parse(Int, colorCode[3:4], base = 16)
        colorBlue = parse(Int, colorCode[5:6], base = 16)
        edgeColor = (colorRed, colorGreen, colorBlue, 255) ./ 255
        centerColor = (colorRed, colorGreen, colorBlue, 102) ./ 255

        Ahorn.drawRectangle(ctx, 0, 0, width, height, centerColor, edgeColor)
    end
end

end