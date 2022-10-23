module XaphanHelperHeatParticles

using ..Ahorn, Maple

@mapdef Effect "XaphanHelper/HeatParticles" HeatParticles(only::String="*", exclude::String="", particlesColors::String="FF0000,FFA500", particlesAmount::Integer=50, noMist::Bool=false)

placements = HeatParticles

function Ahorn.canFgBg(HeatParticles)
    return true, true
end

end