local ResetFlagsTrigger = {}

ResetFlagsTrigger.name = "XaphanHelper/ResetFlagsTrigger"
ResetFlagsTrigger.fieldOrder = {
    "x", "y", "width", "height", "setTrueFlags", "setFalseFlags", "conditionFlags", "transitionUpdate", "removeWhenOutside", "registerInSaveData"
}
ResetFlagsTrigger.placements = {
    name = "ResetFlagsTrigger",
    data = {
        setTrueFlags = "",
        setFalseFlags = "",
        transitionUpdate = false,
        removeWhenOutside = false,
        registerInSaveData = false,
        conditionFlags = ""
    }
}

return ResetFlagsTrigger