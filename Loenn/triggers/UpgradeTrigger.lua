local UpgradeTrigger = {}

UpgradeTrigger.name = "XaphanHelper/UpgradeTrigger"
UpgradeTrigger.fieldInformation = {
    upgrade = {
        options = {"Binoculars", "Bombs", "ClimbingKit", "DashBoots", "DroneTeleport", "EtherealDash", "GoldenFeather", "GravityJacket", "HoverBoots", "IceBeam", "JumpBoost", "LightningDash", "LongBeam", "MegaBombs", "MissilesModule", "PortableStation", "PowerGrip", "PulseRadar", "RemoteDrone", "ScrewAttack", "SpaceJump", "SpiderMagnet", "SuperMissilesModule", "VariaJacket", "WaveBeam"},
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