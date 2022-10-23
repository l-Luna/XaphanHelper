local drawableSprite = require("structs.drawable_sprite")

local Cell = {}

Cell.name = "XaphanHelper/Cell"
Cell.depth = 100
Cell.fieldOrder = {
    "x", "y", "tutorial", "sprite", "emitLight", "lightColor", "flag", "dropWhenDreamDash", "throwForceMultiplier", "throwUpForceMultiplier", "postThrowNoGravityTimer", "gravityMultiplier", "frictionMultiplier", "bounceMultiplier", "killPlayerOnDeath", "deathColor", "hitSidesSound", "hitGroundSound", "deathSound"
}
Cell.fieldInformation = {
    deathColor = {
        fieldType = "color"
    },
    lightColor = {
        fieldType = "color"
    }
}
Cell.placements = {
    name = "Cell",
    data = {
        tutorial = false,
        flag = "",
        sprite = "objects/XaphanHelper/Cell",
        dropWhenDreamDash = false,
        bounceMultiplier = 0.40,
        throwForceMultiplier = 1.00,
        throwUpForceMultiplier = 0.40,
        gravityMultiplier = 1.00,
        frictionMultiplier = 1.00,
        postThrowNoGravityTimer = 0.10,
        deathColor = "0088E8",
        killPlayerOnDeath = true,
        emitLight = true,
        lightColor= "FFFFFF",
        hitSidesSound = "event:/game/05_mirror_temple/crystaltheo_hit_side",
        hitGroundSound = "event:/game/05_mirror_temple/crystaltheo_hit_ground",
        deathSound = "event:/char/madeline/death"
    }
}

function Cell.sprite(room, entity)
    local texture = entity.sprite or "objects/XaphanHelper/Cell"
    local sprite = drawableSprite.fromTexture(texture .. "/cell00", entity)
    sprite:addPosition(0, -8)

    return sprite
end

return Cell