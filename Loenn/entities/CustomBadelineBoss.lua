local enums = require("consts.celeste_enums")

local CustomBadelineBoss = {}

CustomBadelineBoss.name = "XaphanHelper/CustomBadelineBoss"
CustomBadelineBoss.depth = 0
CustomBadelineBoss.nodeLineRenderType = "line"
CustomBadelineBoss.texture = "characters/badelineBoss/charge00"
CustomBadelineBoss.nodeLimits = {0, -1}
CustomBadelineBoss.fieldOrder = {
    "x", "y", "cameraPastY", "patternIndex", "spriteName", "trailColor", "MoveParticleColor1", "MoveParticleColor2", "hitParticleColor1", "hitParticleColor2", "shotTrailParticleColor1", "shotTrailParticleColor2", "beamDissipateParticleColor", "startHit", "cameraLock", "cameraLockY", "canChangeMusic", "drawProjectilesOutline"
}
CustomBadelineBoss.fieldInformation = {
    patternIndex = {
        fieldType = "integer",
        options = enums.badeline_boss_shooting_patterns,
        editable = false
    },
    hitParticleColor1 = {
        fieldType = "color"
    },
    hitParticleColor2 = {
        fieldType = "color"
    },
    shotTrailParticleColor1 = {
        fieldType = "color"
    },
    shotTrailParticleColor2 = {
        fieldType = "color"
    },
    beamDissipateParticleColor = {
        fieldType = "color"
    },
    MoveParticleColor1 = {
        fieldType = "color"
    },
    MoveParticleColor2 = {
        fieldType = "color"
    },
    trailColor = {
        fieldType = "color"
    }
}
CustomBadelineBoss.placements = {
    name = "CustomBadelineBoss",
    data = {
        patternIndex = 1,
        startHit = false,
        cameraPastY = 120.0,
        cameraLockY = true,
        canChangeMusic = true,
        hitParticleColor1 = "ff00b0",
        hitParticleColor2 = "ff84d9",
        shotTrailParticleColor1 = "ffced5",
        shotTrailParticleColor2 = "ff4f7d",
        beamDissipateParticleColor = "e60022",
        MoveParticleColor1 = "ac3232",
        MoveParticleColor2 = "e05959",
        trailColor = "ac3232",
        spriteName = "badeline_boss",
        cameraLock = true,
        drawProjectilesOutline = true
    }
}

return CustomBadelineBoss