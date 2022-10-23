local MergeChaptersController = {}

MergeChaptersController.name = "XaphanHelper/MergeChaptersController"
MergeChaptersController.depth = -100000
MergeChaptersController.fieldOrder = {
    "x", "y", "displayFlags", "hideFlag", "type", "direction", "tileCords", "removeWhenReachedByPlayer"
}
MergeChaptersController.fieldInformation = {
    mode = {
        options = {"Classic", "Rooms", "Warps"},
        editable = false
    }
}
MergeChaptersController.placements = {
    name = "MergeChaptersController",
    data = {
        mode = "Classic",
        keepPrologueSeparated = false
    }
}

MergeChaptersController.texture = "util/XaphanHelper/Loenn/mergeChaptersController"

return MergeChaptersController