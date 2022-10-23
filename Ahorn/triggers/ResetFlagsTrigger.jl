module XaphanHelperResetFlagsTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/ResetFlagsTrigger" ResetFlagsTrigger(x::Integer, y::Integer, setTrueFlags::String="", setFalseFlags::String="", transitionUpdate::Bool=false, removeWhenOutside::Bool=false, registerInSaveData::Bool=false)

const placements = Ahorn.PlacementDict(
    "Reset Flags Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        ResetFlagsTrigger,
        "rectangle"
    )
)

end