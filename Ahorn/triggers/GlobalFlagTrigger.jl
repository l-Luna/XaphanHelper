module XaphanHelperGlobalFlagTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/GlobalFlagTrigger" GlobalFlagTrigger(x::Integer, y::Integer, flag::String="", state::Bool=true, switchFlag::Bool=false, levelSet::String="")

const placements = Ahorn.PlacementDict(
    "Global Flag Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        GlobalFlagTrigger,
        "rectangle"
    )
)

end