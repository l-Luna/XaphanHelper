module XaphanHelperCollectableDoor

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CollectableDoor" CollectableDoor(x::Integer, y::Integer, width::Integer=24, height::Integer=24, requires::Number=1, orientation::String="Vertical", mode::String="TotalHearts", interiorColor::String="18668f", mistColor::String="ffffff", edgesColor::String="ffffff", iconsColor::String="ffffff", interiorParticlesColor::String="ffffff", openingParticlesColor1::String="baffff", openingParticlesColor2::String="5abce2", sliceColor::String="ffffff", sliceParticlesColor1::String="ffffff", sliceParticlesColor2::String="ffffff", directory::String="", flags::String="", checkDistance::Number=10, checkDisplaySpeed::Number=5, unlockSound::String="event:/game/09_core/frontdoor_unlock", fillSound::String="event:/game/09_core/frontdoor_heartfill", soundIndex::Number=32, beforeSliceDelay::Number=0.50, afterSliceDelay::Number=0.60, openSpeedMultiplier::Number=1.00, edges::String="All", edgesAnimationMode::String="Clockwise", mapIcon::String="", registerInSaveData::Bool=false)

const placements = Ahorn.PlacementDict(
    "Collectable Door (Xaphan Helper)" => Ahorn.EntityPlacement(
        CollectableDoor,
        "rectangle"
    )
)

Ahorn.minimumSize(entity::CollectableDoor) = 24, 24
Ahorn.resizable(entity::CollectableDoor) = true, true

Ahorn.editingOptions(entity::CollectableDoor) = Dict{String, Any}(
    "orientation" => String["Vertical", "Horizontal"],
    "mode" => String["TotalHearts", "CurrentChapterHeart", "CurrentSessionHeart", "TotalCassettes", "CurrentChapterCassette", "CurrentSessionCassette", "TotalStrawberries", "CurrentChapterStrawberries", "CurrentSessionStrawberries", "GoldenStrawberry", "Flags"],
    "edges" => String["All", "LeftRight", "TopBottom", "None"],
    "edgesAnimationMode" => String["Clockwise", "CounterClockwise", "Static"],
    "soundIndex" => Maple.tileset_sound_ids
)

Ahorn.editingOrder(entity::CollectableDoor) = String["x", "y", "width", "height", "orientation", "mode", "edges", "edgesAnimationMode", "directory", "mapIcon", "interiorColor", "mistColor", "iconsColor", "edgesColor", "interiorParticlesColor", "openingParticlesColor1", "openingParticlesColor2", "sliceColor", "sliceParticlesColor1", "sliceParticlesColor2", "requires", "flags", "openSpeedMultiplier", "beforeSliceDelay", "afterSliceDelay", "checkDistance", "checkDisplaySpeed", "unlockSound", "fillSound", "soundIndex", "registerInSaveData"]

const collectablePadding = 2

function Ahorn.selection(entity::CollectableDoor, room::Maple.Room)
    orientation = get(entity.data, "orientation", "Vertical")
    x, y = Ahorn.position(entity)
    width = get(entity.data, "width", 24)
    height = get(entity.data, "height", 24)
    nodes = get(entity.data, "nodes", ())

    if orientation == "Vertical"
        if isempty(nodes)
            return Ahorn.Rectangle(x, y - height, width, height * 2)
        else
            nx, ny = Int.(nodes[1])
            return [Ahorn.Rectangle(x, y - height, width, height * 2), Ahorn.Rectangle(nx - 8, ny, width + 16, 8)]
        end
    else
        if isempty(nodes)
            return Ahorn.Rectangle(x - width, y, width * 2, height)
        else
            nx, ny = Int.(nodes[1])
            return [Ahorn.Rectangle(x - width, y, width * 2, height), Ahorn.Rectangle(nx, ny - height - 16, 8, height + 16)]
        end
    end
end

function collectablesWidth(collectableSprite::Ahorn.Sprite, collectables::Integer)
    return collectables * (collectableSprite.width + collectablePadding) - collectablePadding
end

function collectablesPossible(width::Integer, edgeSprite::Ahorn.Sprite, collectableSprite::Ahorn.Sprite, required::Integer)
    rowWidth = width - 2 * edgeSprite.width

    for i in 0:required
        if collectablesWidth(collectableSprite, i) > rowWidth
            return i - 1
        end
    end
    
    return required
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::CollectableDoor, room::Maple.Room)
    orientation = get(entity.data, "orientation", "Vertical")
    x, y = Ahorn.position(entity)
    nodes = get(entity.data, "nodes", ())
    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    if orientation == "Vertical"
        if !isempty(nodes)
            nx, ny = Int.(nodes[1])
            dy = ny - y
            Ahorn.drawRectangle(ctx, x, ny, width, 1, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
            Ahorn.drawRectangle(ctx, x, 2 * y - ny, width, 1, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
            Ahorn.drawRectangle(ctx, nx - 8, ny, width + 16, 8, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
        end
    else
        if !isempty(nodes)
            nx, ny = Int.(nodes[1])
            dy = ny - y
            Ahorn.drawRectangle(ctx, nx, y, 1, height, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
            Ahorn.drawRectangle(ctx, 2 * x - nx, y, 1, height, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
            Ahorn.drawRectangle(ctx, nx, ny - height - 16, 8, height + 16, (1.0, 0.0, 0.0, 1.0), (0.0, 0.0, 0.0, 0.0))
        end
    end
end

# Not completely accurate on collectable positions, but good enough
function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::CollectableDoor, room::Maple.Room)
    orientation = get(entity.data, "orientation", "Vertical")
    x, y = Ahorn.position(entity)
    width = Int(get(entity.data, "width", 24))
    height = Int(get(entity.data, "height", 24))
    collectables = Int(get(entity.data, "requires", 1))
    mode = get(entity.data, "mode", "TotalHearts")
    edges = get(entity.data, "edges", "All")
    interiorColor = get(entity.data, "interiorColor", "18668f")
    edgesColor = get(entity.data, "edgesColor", "FFFFFF")
    iconsColor = get(entity.data, "iconsColor", "FFFFFF")
    directory = get(entity.data, "directory", "")
    icon = nothing
    iconTexture = Dict{String, String}(
    "heart" => "objects/heartdoor/icon00",
    "strawberries" => "objects/XaphanHelper/CollectableDoor/Strawberry/icon00",
    "golden" => "objects/XaphanHelper/CollectableDoor/GoldenStrawberry/icon00",
    "cassette" => "objects/XaphanHelper/CollectableDoor/Cassette/icon00",
    "flag" => "objects/XaphanHelper/CollectableDoor/Flag/icon00",
    "custom" => directory * "/icon00")
    edgeSprite = Ahorn.getSprite("objects/heartdoor/edge", "Gameplay")
    topSprite = Ahorn.getSprite("objects/heartdoor/top", "Gameplay")
    if directory != ""
        icon = "custom"
        edgeSprite = Ahorn.getSprite(directory * "/edge", "Gameplay")
        topSprite = Ahorn.getSprite(directory * "/top", "Gameplay")
    else
        if occursin("Heart", entity.mode)
            icon = "heart"
        elseif occursin("Strawberries", entity.mode)
            icon = "strawberries"
        elseif occursin("Golden", entity.mode)
            icon = "golden"
        elseif occursin("Cassette", entity.mode)
            icon = "cassette"
        elseif occursin("Flag", entity.mode)
            icon = "flag"
        end
    end
    collectableSprite = Ahorn.getSprite(iconTexture[icon], "Gameplay")
    wallColor = (parse(Int, interiorColor[1:2], base = 16),parse(Int, interiorColor[3:4], base = 16),parse(Int, interiorColor[5:6], base = 16), 255) ./ 255
    borderColor = (parse(Int, edgesColor[1:2], base = 16),parse(Int, edgesColor[3:4], base = 16),parse(Int, edgesColor[5:6], base = 16), 255) ./ 255
    symbolColor = (parse(Int, iconsColor[1:2], base = 16),parse(Int, iconsColor[3:4], base = 16),parse(Int, iconsColor[5:6], base = 16), 255) ./ 255
    
    if orientation == "Vertical"
        Ahorn.drawRectangle(ctx, x, y - height, width, height * 2, wallColor, (0.0, 0.0, 0.0, 0.0))
    else
        Ahorn.drawRectangle(ctx, x - width, y, width * 2, height, wallColor, (0.0, 0.0, 0.0, 0.0))
    end
    if orientation == "Vertical"
        if edges == "All" || edges == "LeftRight"
            for i in 0:edgeSprite.height:height * 2 - 8
                Ahorn.Cairo.save(ctx)

                Ahorn.drawImage(ctx, edgeSprite, x + width - edgeSprite.width, y - height + i, tint=borderColor)
                Ahorn.scale(ctx, -1, 1)
                Ahorn.drawImage(ctx, edgeSprite, -x - edgeSprite.width, y - height + i, tint=borderColor)

                Ahorn.Cairo.restore(ctx)
            end
        end
        if edges == "All" || edges == "TopBottom"
            for j in 0:topSprite.width:width - 8
                Ahorn.Cairo.save(ctx)

                Ahorn.drawImage(ctx, topSprite, x + topSprite.width - 8 + j, y - height, tint=borderColor)
                Ahorn.scale(ctx, 1, -1)
                Ahorn.drawImage(ctx, topSprite, x + topSprite.width - 8 + j, -y - height, tint=borderColor)

                Ahorn.Cairo.restore(ctx)
            end
        end
    else
        if edges == "All" || edges == "LeftRight"
            for i in 0:edgeSprite.height:height - 8
                Ahorn.Cairo.save(ctx)

                Ahorn.drawImage(ctx, edgeSprite, x + width - edgeSprite.width, y + i, tint=borderColor)
                Ahorn.scale(ctx, -1, 1)
                Ahorn.drawImage(ctx, edgeSprite, -x + width - edgeSprite.width, y + i, tint=borderColor)

                Ahorn.Cairo.restore(ctx)
            end
        end
        if edges == "All" || edges == "TopBottom"
            for j in 0:topSprite.width:width * 2 - 8
                Ahorn.Cairo.save(ctx)

                Ahorn.drawImage(ctx, topSprite, x - width + topSprite.width - 8 + j, y, tint=borderColor)
                Ahorn.scale(ctx, 1, -1)
                Ahorn.drawImage(ctx, topSprite, x - width + topSprite.width - 8 + j, -y - height, tint=borderColor)

                Ahorn.Cairo.restore(ctx)
            end
        end
    end

    if collectables > 0
        if mode == "GoldenStrawberry"
            collectables = 1
        end
        if orientation == "Vertical"
            fits = collectablesPossible(width, edgeSprite, collectableSprite, collectables)
        else
            fits = collectablesPossible(width * 2, edgeSprite, collectableSprite, collectables)
        end
        rows = ceil(Int, collectables / fits)

        for row in 1:rows
        if orientation == "Vertical"
            fits = collectablesPossible(width, edgeSprite, collectableSprite, collectables)
        else
            fits = collectablesPossible(width * 2, edgeSprite, collectableSprite, collectables)
        end
        drawWidth = collectablesWidth(collectableSprite, fits)

        startX = nothing
        startY = nothing
        if orientation == "Vertical"
            startX = x + round(Int, (width - drawWidth) / 2) + edgeSprite.width - 3
            startY = y - round(Int, rows / 2 * (collectableSprite.height + collectablePadding)) - collectablePadding - 1
        else
            startX = x - width + round(Int, (width * 2 - drawWidth) / 2) + edgeSprite.width - 3
            startY = y - round(Int, rows / 2 * (collectableSprite.height + collectablePadding)) - collectablePadding  + height / 2 - 1
        end
        for col in 1:fits
            drawX = (col - 1) * (collectableSprite.width + collectablePadding) - collectablePadding
            drawY = row * (collectableSprite.height + collectablePadding) - collectablePadding

            Ahorn.drawImage(ctx, collectableSprite, startX + drawX, startY + drawY, tint=symbolColor)
        end

        collectables -= fits
    end
end
end
end