module XaphanHelperUpgradeCollectable

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/UpgradeCollectable" UpgradeCollectable(x::Integer, y::Integer, width::Integer=16, height::Integer=16, collectSound::String="event:/game/07_summit/gem_get", newMusic::String="", upgrade::String="Map", nameColor::String="FFFFFF", descColor::String="FFFFFF", particleColor::String="FFFFFF", customName::String="", customSprite::String="", mapShardIndex::Integer=0)

const placements = Ahorn.PlacementDict(
    "Upgrade Collectable (Xaphan Helper)" => Ahorn.EntityPlacement(
        UpgradeCollectable,
        "point",
        ),
    )

Ahorn.editingOptions(entity::UpgradeCollectable) = Dict{String, Any}(
    "upgrade" => String["Map", "MapShard", "Binoculars", "Bombs", "ClimbingKit", "DashBoots", "DroneTeleport", "EtherealDash", "GoldenFeather", "GravityJacket", "HoverBoots", "IceBeam", "LightningDash", "LongBeam", "MegaBombs", "PortableStation", "PowerGrip", "PulseRadar", "RemoteDrone", "ScrewAttack", "SpaceJump", "SpiderMagnet", "VariaJacket", "WaveBeam"]
)

Ahorn.minimumSize(entity::UpgradeCollectable) = 16, 16
Ahorn.resizable(entity::UpgradeCollectable) = false, false

Ahorn.selection(entity::UpgradeCollectable) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::UpgradeCollectable, room::Maple.Room)
    sprite = lowercasefirst(get(entity.data, "upgrade", "Map"))
    customSprite = get(entity.data, "customSprite", "")
    if sprite == "mapShard"
        sprite = "map"
    end
    if customSprite == ""
        customSprite = "collectables/XaphanHelper/UpgradeCollectable"
    end
    Ahorn.drawSprite(ctx, "$(customSprite)/$(sprite)00.png", 8, 8)
end

end