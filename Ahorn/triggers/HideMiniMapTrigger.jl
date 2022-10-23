module XaphanHelperHideMiniMapTrigger

using ..Ahorn, Maple

@mapdef Trigger "XaphanHelper/HideMiniMapTrigger" HideMiniMapTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16)

const placements = Ahorn.PlacementDict(
    "Hide Mini Map Trigger (Xaphan Helper)" => Ahorn.EntityPlacement(
        HideMiniMapTrigger,
        "rectangle"
    )
)

end