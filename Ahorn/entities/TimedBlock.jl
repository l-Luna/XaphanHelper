module XaphanHelperTimedBlock

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TimedBlock" TimedBlock(x::Integer, y::Integer, width::Integer=8, height::Integer=8, startPressed::Bool=false, color::String="FFFFFF", directory::String="objects/XaphanHelper/TimedBlock", soundIndex::Integer=35)

const placements = Ahorn.PlacementDict(
    "Timed Block (Xaphan Helper)" => Ahorn.EntityPlacement(
        TimedBlock,
        "rectangle",
        Dict{String, Any}()
    ),
)

Ahorn.minimumSize(entity::TimedBlock) = 16, 16
Ahorn.resizable(entity::TimedBlock) = true, true

Ahorn.selection(entity::TimedBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.editingOptions(entity::TimedBlock) = Dict{String, Any}(
    "soundIndex" => Maple.tileset_sound_ids
)

const borderMultiplier = (0.9, 0.9, 0.9, 1)

function getTimedBlockRectangles(room::Maple.Room)
    entities = filter(e -> e.name == "XaphanHelper/TimedBlock", room.entities)
    rects = Dict{Int, Array{Ahorn.Rectangle, 1}}()

    for e in entities
        index = get(e.data, "index", 0)
        rectList = get!(rects, index) do
            Ahorn.Rectangle[]
        end
        
        push!(rectList, Ahorn.Rectangle(
            Int(get(e.data, "x", 0)),
            Int(get(e.data, "y", 0)),
            Int(get(e.data, "width", 8)),
            Int(get(e.data, "height", 8))
        ))
    end
        
    return rects
end

# Is there a casette block we should connect to at the offset?
function notAdjacent(entity::TimedBlock, ox, oy, rects)
    x, y = Ahorn.position(entity)
    rect = Ahorn.Rectangle(x + ox + 4, y + oy + 4, 1, 1)

    for r in rects
        if Ahorn.checkCollision(r, rect)
            return false
        end
    end

    return true
end

function drawTimedBlock(ctx::Ahorn.Cairo.CairoContext, entity::TimedBlock, room::Maple.Room)
    frame = get(entity.data, "directory", "objects/XaphanHelper/TimedBlock") * "/solid"
    if frame == "/solid"
        frame = "objects/XaphanHelper/TimedBlock/solid"
    end
    jumpBlockRectangles = getTimedBlockRectangles(room)

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    tileWidth = ceil(Int, width / 8)
    tileHeight = ceil(Int, height / 8)

    index = Int(get(entity.data, "index", 0))
    
    colorCode=get(entity.data, "color", "FFFFFF")
    if colorCode == ""
        colorCode == FFFFFF
    end
    colorRed = parse(Int, colorCode[1:2], base = 16)
    colorGreen = parse(Int, colorCode[3:4], base = 16)
    colorBlue = parse(Int, colorCode[5:6], base = 16)
    color = (colorRed, colorGreen, colorBlue, 255) ./ 255

    rect = Ahorn.Rectangle(x, y, width, height)
    rects = get(jumpBlockRectangles, index, Ahorn.Rectangle[])

    if !(rect in rects)
        push!(rects, rect)
    end

    for x in 1:tileWidth, y in 1:tileHeight
        drawX, drawY = (x - 1) * 8, (y - 1) * 8

        closedLeft = !notAdjacent(entity, drawX - 8, drawY, rects)
        closedRight = !notAdjacent(entity, drawX + 8, drawY, rects)
        closedUp = !notAdjacent(entity, drawX, drawY - 8, rects)
        closedDown = !notAdjacent(entity, drawX, drawY + 8, rects)
        completelyClosed = closedLeft && closedRight && closedUp && closedDown

        if completelyClosed
            if notAdjacent(entity, drawX + 8, drawY - 8, rects)
                Ahorn.drawImage(ctx, frame, drawX, drawY, 24, 0, 8, 8, tint=color)

            elseif notAdjacent(entity, drawX - 8, drawY - 8, rects)
                Ahorn.drawImage(ctx, frame, drawX, drawY, 24, 8, 8, 8, tint=color)

            elseif notAdjacent(entity, drawX + 8, drawY + 8, rects)
                Ahorn.drawImage(ctx, frame, drawX, drawY, 24, 16, 8, 8, tint=color)

            elseif notAdjacent(entity, drawX - 8, drawY + 8, rects)
                Ahorn.drawImage(ctx, frame, drawX, drawY, 24, 24, 8, 8, tint=color)

            else
                Ahorn.drawImage(ctx, frame, drawX, drawY, 8, 8, 8, 8, tint=color)
            end

        else
            if closedLeft && closedRight && !closedUp && closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 8, 0, 8, 8, tint=color)

            elseif closedLeft && closedRight && closedUp && !closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 8, 16, 8, 8, tint=color)

            elseif closedLeft && !closedRight && closedUp && closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 16, 8, 8, 8, tint=color)

            elseif !closedLeft && closedRight && closedUp && closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 0, 8, 8, 8, tint=color)

            elseif closedLeft && !closedRight && !closedUp && closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 16, 0, 8, 8, tint=color)

            elseif !closedLeft && closedRight && !closedUp && closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 0, 0, 8, 8, tint=color)

            elseif !closedLeft && closedRight && closedUp && !closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 0, 16, 8, 8, tint=color)

            elseif closedLeft && !closedRight && closedUp && !closedDown
                Ahorn.drawImage(ctx, frame, drawX, drawY, 16, 16, 8, 8, tint=color)
            end
        end
    end
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TimedBlock, room::Maple.Room) = drawTimedBlock(ctx, entity, room)

end