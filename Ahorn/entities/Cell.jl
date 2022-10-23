module XaphanHelperCell

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/Cell" Cell(x::Integer, y::Integer, tutorial::Bool=false, flag::String="", sprite::String="objects/XaphanHelper/Cell", dropWhenDreamDash::Bool=false, throwForceMultiplier::Number=1.00, throwUpForceMultiplier::Number=0.40, postThrowNoGravityTimer::Number=0.10, gravityMultiplier::Number=1.00, frictionMultiplier::Number=1.00, bounceMultiplier::Number=0.40, killPlayerOnDeath::Bool=true, deathColor::String="0088E8", emitLight::Bool=true, lightColor::String="FFFFFF", hitSidesSound::String="event:/game/05_mirror_temple/crystaltheo_hit_side", hitGroundSound::String="event:/game/05_mirror_temple/crystaltheo_hit_ground", deathSound::String="event:/char/madeline/death")

const placements = Ahorn.PlacementDict(
    "Cell (Xaphan Helper)" => Ahorn.EntityPlacement(
        Cell,
        "point",
        ),
    )

    function Ahorn.selection(entity::Cell)
        x, y = Ahorn.position(entity)
        sprite = get(entity.data, "sprite", "objects/XaphanHelper/Cell")
        if sprite == ""
            sprite = "objects/XaphanHelper/Cell"
        end
        return Ahorn.getSpriteRectangle("$(sprite)/cell00.png", x, y - 8)
    end

    function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Cell, room::Maple.Room)
        sprite = get(entity.data, "sprite", "objects/XaphanHelper/Cell")
        if sprite == ""
            sprite = "objects/XaphanHelper/Cell"
        end
        Ahorn.drawSprite(ctx, "$(sprite)/cell00.png", 0, -8, jx=0.5, jy=0.5)
    end

end