module XaphanHelperUpgradeController

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/UpgradeController" UpgradeController(x::Integer, y::Integer,
startWithPowerGrip::Bool=false, startWithClimbingKit::Bool=false, startWithSpiderMagnet::Bool=false, startWithHoverBoots::Bool=false, startWithLightningDash::Bool=false,
startWithDroneTeleport::Bool=false, startWithScrewAttack::Bool=false, startWithVariaJacket::Bool=false, startWithGravityJacket::Bool=false, startWithBombs::Bool=false,
startWithRemoteDrone::Bool=false, startWithGoldenFeather::Bool=false, startWithBinoculars::Bool=false, startWithEtherealDash::Bool=false, startWithDashBoots::Bool=false,
startWithSpaceJump::Bool=false, startWithPortableStation::Bool=false, startWithPulseRadar::Bool=false, startWithMegaBombs::Bool=false, startWithLongBeam::Bool=false,
startWithIceBeam::Bool=false, startWithWaveBeam::Bool=false,
goldenStartWithPowerGrip::Bool=false, goldenStartWithClimbingKit::Bool=false, goldenStartWithSpiderMagnet::Bool=false, goldenStartWithHoverBoots::Bool=false, goldenStartWithLightningDash::Bool=false,
goldenStartWithDroneTeleport::Bool=false, goldenStartWithScrewAttack::Bool=false, goldenStartWithVariaJacket::Bool=false, goldenStartWithGravityJacket::Bool=false, goldenStartWithBombs::Bool=false,
goldenStartWithRemoteDrone::Bool=false, goldenStartWithGoldenFeather::Bool=false, goldenStartWithBinoculars::Bool=false, goldenStartWithEtherealDash::Bool=false, goldenStartWithDashBoots::Bool=false,
goldenStartWithSpaceJump::Bool=false, goldenStartWithPortableStation::Bool=false, goldenStartWithPulseRadar::Bool=false, goldenStartWithMegaBombs::Bool=false, goldenStartWithLongBeam::Bool=false,
goldenStartWithIceBeam::Bool=false, goldenStartWithWaveBeam::Bool=false,
onlyAllowStartingUpgrades::Bool=false, upgradesAreTemporary::Bool=false, disableStatusScreen::Bool=false)

const placements = Ahorn.PlacementDict(
    "Upgrade Controller (Xaphan Helper)" => Ahorn.EntityPlacement(
        UpgradeController
    )
)

function Ahorn.selection(entity::UpgradeController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 10, y - 10, 20, 20)
end

Ahorn.editingOrder(entity::UpgradeController) = String["x", "y",
    "startWithPowerGrip", "goldenStartWithPowerGrip",
    "startWithClimbingKit", "goldenStartWithClimbingKit",
    "startWithSpiderMagnet", "goldenStartWithSpiderMagnet",
    "startWithDashBoots", "goldenStartWithDashBoots",
    "startWithSpaceJump", "goldenStartWithSpaceJump",
    "startWithHoverBoots", "goldenStartWithHoverBoots",
    "startWithLightningDash", "goldenStartWithLightningDash",
    "startWithLongBeam", "goldenStartWithLongBeam",
    "startWithIceBeam", "goldenStartWithIceBeam",
    "startWithWaveBeam", "goldenStartWithWaveBeam",
    "startWithDroneTeleport", "goldenStartWithDroneTeleport",
    "startWithVariaJacket", "goldenStartWithVariaJacket",
    "startWithGravityJacket", "goldenStartWithGravityJacket",
    "startWithBombs", "goldenStartWithBombs",
    "startWithMegaBombs", "goldenStartWithMegaBombs",
    "startWithRemoteDrone", "goldenStartWithRemoteDrone",
    "startWithGoldenFeather", "goldenStartWithGoldenFeather",
    "startWithEtherealDash", "goldenStartWithEtherealDash",
    "startWithScrewAttack", "goldenStartWithScrewAttack",
    "startWithBinoculars", "goldenStartWithBinoculars",
    "startWithPortableStation", "goldenStartWithPortableStation",
    "startWithPulseRadar", "goldenStartWithPulseRadar",
    "onlyAllowStartingUpgrades", "upgradesAreTemporary", "disableStatusScreen"]

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::UpgradeController, room::Maple.Room)
    Ahorn.Cairo.save(ctx)

    Ahorn.set_antialias(ctx, 1)
    Ahorn.set_line_width(ctx, 1);

    Ahorn.drawCircle(ctx, 0, 0, 10, (1.0, 1.0, 1.0, 1.0))

    Ahorn.Cairo.restore(ctx)

    Ahorn.drawSprite(ctx, "util/XaphanHelper/upgradeController", 0, 0)
end

end