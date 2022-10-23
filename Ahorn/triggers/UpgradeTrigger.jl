module XaphanHelperUpgradeTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/UpgradeTrigger" UpgradeTrigger(x::Integer, y::Integer, upgrade::String="Bombs", disable::Bool=true)

const placements = Ahorn.PlacementDict(
    "Upgrade Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        UpgradeTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(entity::UpgradeTrigger) = Dict{String, Any}(
    "upgrade" => String["Binoculars", "Bombs", "ClimbingKit", "DashBoots", "DroneTeleport", "EtherealDash", "GoldenFeather", "GravityJacket", "HoverBoots", "IceBeam", "LightningDash", "LongBeam", "MegaBombs", "PortableStation", "PowerGrip", "PulseRadar", "RemoteDrone", "ScrewAttack", "SpaceJump", "SpiderMagnet", "VariaJacket", "WaveBeam"]
)

end