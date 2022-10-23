module XaphanHelperCustomBadelineBoss

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/CustomBadelineBoss" CustomBadelineBoss(x::Integer, y::Integer, patternIndex::Number=1, startHit::Bool=false, cameraPastY::Number=120.0, cameraLockY::Bool=true, canChangeMusic::Bool=true, hitParticleColor1::String="ff00b0", hitParticleColor2::String="ff84d9", shotTrailParticleColor1::String="ffced5", shotTrailParticleColor2::String="ff4f7d", beamDissipateParticleColor::String="e60022", MoveParticleColor1::String="ac3232", MoveParticleColor2::String="e05959", trailColor::String = "ac3232", spriteName::String="badeline_boss")

const placements = Ahorn.PlacementDict(
    "Custom Badeline Boss (Xaphan Helper)" => Ahorn.EntityPlacement(
        CustomBadelineBoss,
    )
)

Ahorn.editingOptions(entity::CustomBadelineBoss) = Dict{String, Any}(
    "patternIndex" => Maple.badeline_boss_shooting_patterns
)

Ahorn.nodeLimits(entity::CustomBadelineBoss) = 0, -1

sprite = "characters/badelineBoss/charge00.png"

function Ahorn.selection(entity::CustomBadelineBoss)
    nodes = get(entity.data, "nodes", ())
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(sprite, x, y)]
    
    for node in nodes
        nx, ny = Int.(node)

        push!(res, Ahorn.getSpriteRectangle(sprite, nx, ny))
    end

    return res
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::CustomBadelineBoss)
    px, py = Ahorn.position(entity)

    for node in get(entity.data, "nodes", ())
        nx, ny = Int.(node)

        Ahorn.drawArrow(ctx, px, py, nx, ny, Ahorn.colors.selection_selected_fc, headLength=6)
        Ahorn.drawSprite(ctx, sprite, nx, ny)

        px, py = nx, ny
    end
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::CustomBadelineBoss, room::Maple.Room)
    x, y = Ahorn.position(entity)
    Ahorn.drawSprite(ctx, sprite, x, y)
end

end