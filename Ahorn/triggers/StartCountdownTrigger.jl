module XaphanHelperStartCountdownTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/StartCountdownTrigger" StartCountdownTrigger(x::Integer, y::Integer, time::Number=60.00, startFlag::String="", activeFlag::String="", shake::Bool=false, explosions::Bool=false, crossChapter::Bool=false, dialogID::String="", messageTimer::Number=5.00, fastMessageDisplay::Bool=false, messageColor::String="FFFFFF")

const placements = Ahorn.PlacementDict(
    "Start Countdown Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        StartCountdownTrigger,
        "rectangle"
    )
)

end