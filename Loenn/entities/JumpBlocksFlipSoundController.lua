local JumpBlocksFlipSoundController = {}

JumpBlocksFlipSoundController.name = "XaphanHelper/JumpBlocksFlipSoundController"
JumpBlocksFlipSoundController.depth = -100000
JumpBlocksFlipSoundController.fieldOrder = {"x", "y", "onSound", "offSound"}
JumpBlocksFlipSoundController.placements = {
    name = "JumpBlocksFlipSoundController",
    data = {
        onSound = "",
        offSound = ""
    }
}

JumpBlocksFlipSoundController.texture = "@Internal@/sound_source"

return JumpBlocksFlipSoundController