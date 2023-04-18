local MultiMusicTrigger = {}

MultiMusicTrigger.name = "XaphanHelper/MultiMusicTrigger"
MultiMusicTrigger.fieldOrder = {
    "x", "y", "width", "height", "flagA", "flagB", "trackNone", "trackA", "trackB", "trackBoth", "removeWhenOutside"
}
MultiMusicTrigger.placements = {
    name = "MultiMusicTrigger",
    data = {
        flagA = "",
        flagB = "",
        trackNone = "",
        trackA = "",
        trackB= "",
        trackBoth = "",
        removeWhenOutside = false
    }
}

return MultiMusicTrigger