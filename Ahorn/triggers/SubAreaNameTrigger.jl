module XaphanHelperSubAreaNameTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/SubAreaNameTrigger" TextTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, dialogID::String="", timer::Number=3.00, textPositionX::String="Right", textPositionY::Integer=1040)

const placements = Ahorn.PlacementDict(
    "Sub-Area Name Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        TextTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(Trigger::TextTrigger) = Dict{String, Any}(
    "textPositionX" => String["Left", "Middle", "Right"]
)

end