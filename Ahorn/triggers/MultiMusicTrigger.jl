module XaphanHelperMultiMusicTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/MultiMusicTrigger" MultiMusicTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16,flagA::String="", flagB::String="", trackNone::String="", trackA::String="", trackB::String="", trackBoth::String="")

const placements = Ahorn.PlacementDict(
    "Multi Music Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        MultiMusicTrigger,
        "rectangle"
    )
)

end