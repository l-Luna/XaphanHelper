module XaphanHelperStopCountdownTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/StopCountdownTrigger" StopCountdownTrigger(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Stop Countdown Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        StopCountdownTrigger,
        "rectangle"
    )
)

end