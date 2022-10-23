module XaphanHelperStopAdjustInGameMapTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/StopAdjustInGameMapTrigger" StopAdjustInGameMapTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16)

const placements = Ahorn.PlacementDict(
    "Stop Adjust In-Game Map Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        StopAdjustInGameMapTrigger,
        "rectangle"
    )
)

end