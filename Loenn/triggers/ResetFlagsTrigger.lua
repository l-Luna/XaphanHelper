local ResetFlagsTrigger = {}

ResetFlagsTrigger.name = "XaphanHelper/ResetFlagsTrigger"
ResetFlagsTrigger.fieldOrder = {
    "x", "y", "width", "height", "setTrueFlags", "setFalseFlags", "conditionFlags", "transitionUpdate", "removeWhenOutside", "registerInSaveData", "inverted"
}
ResetFlagsTrigger.placements = {
    name = "ResetFlagsTrigger",
    data = {
        setTrueFlags = "",
        setFalseFlags = "",
        transitionUpdate = false,
        removeWhenOutside = false,
        registerInSaveData = false,
        conditionFlags = "",
        inverted = false
    }
}

return ResetFlagsTrigger