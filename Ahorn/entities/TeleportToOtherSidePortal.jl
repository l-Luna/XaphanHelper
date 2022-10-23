module XaphanHelperTeleportToOtherSidePortal

using ..Ahorn, Maple

@mapdef Entity "XaphanHelper/TeleportToOtherSidePortal" TeleportToOtherSidePortal(x::Integer, y::Integer, side::String="A-Side", requireCassetteCollected::Bool=false, wipeType::String="Fade", wipeDuration::Number=1.35, warpSfx::String="event:/game/xaphan/warp", teleportToStartingSpawnOfChapter::Bool=false, requireCSideUnlocked::Bool=false, flags::String="", registerCurrentSideAsCompelete::Bool=false)

const placements = Ahorn.PlacementDict(
    "Teleport To Other Side Portal (Xaphan Helper)" => Ahorn.EntityPlacement(
        TeleportToOtherSidePortal,
        "point",
        ),
    )

    function Ahorn.selection(entity::TeleportToOtherSidePortal)
        x, y = Ahorn.position(entity)
        side = get(entity.data, "side", "A-Side")
        if side == ""
            side = "A-Side"
        end
        return Ahorn.getSpriteRectangle("objects/XaphanHelper/TeleportToOtherSidePortal/$(side)00.png", x, y)
    end

    Ahorn.editingOptions(entity::TeleportToOtherSidePortal) = Dict{String, Any}(
    "side" => String["A-Side", "B-Side","C-Side"],
    "wipeType" => String["Spotlight", "Curtain", "Mountain", "Dream", "Starfield", "Wind", "Drop", "Fall", "KeyDoor", "Angled", "Heart", "Fade"]
)

    function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TeleportToOtherSidePortal, room::Maple.Room)
        side = get(entity.data, "side", "A-Side")
        if side == ""
            side = "A-Side"
        end
        Ahorn.drawSprite(ctx, "objects/XaphanHelper/TeleportToOtherSidePortal/$(side)00.png", 0, 0, jx=0.5, jy=0.5)
    end

end