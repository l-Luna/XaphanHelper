module XaphanHelperGlow

using ..Ahorn, Maple

@mapdef Effect "XaphanHelper/Glow" Glow(only::String="*", exclude::String="", color::String="000000")

placements = Glow

function Ahorn.canFgBg(Glow)
    return true, true
end

end