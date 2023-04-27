local FlagOnInteractTrigger = {}

FlagOnInteractTrigger.name = "XaphanHelper/FlagOnInteractTrigger"
FlagOnInteractTrigger.fieldOrder = {
    "x", "y", "width", "height", "reqFlags", "setFlag", "state"
}
FlagOnInteractTrigger.placements = {
    name = "FlagOnInteractTrigger",
    data = {
        reqFlags = "",
        setFlag = "",
        state = true
    }
}

return FlagOnInteractTrigger