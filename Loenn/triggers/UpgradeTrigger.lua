local UpgradeTrigger = {}

UpgradeTrigger.name = "XaphanHelper/UpgradeTrigger"
UpgradeTrigger.fieldInformation = {
    upgrade = {
        options = {"Binoculars", "Bombs", "ClimbingKit", "DashBoots", "DroneTeleport", "EtherealDash", "GoldenFeather", "GravityJacket", "HoverBoots", "IceBeam", "LightningDash", "LongBeam", "MegaBombs", "PortableStation", "PowerGrip", "PulseRadar", "RemoteDrone", "ScrewAttack", "SpaceJump", "SpiderMagnet", "VariaJacket", "WaveBeam"},
        editable = false
    }
}
UpgradeTrigger.placements = {
    name = "UpgradeTrigger",
    data = {
        upgrade = "Bombs",
        disable = false
    }
}

return UpgradeTrigger