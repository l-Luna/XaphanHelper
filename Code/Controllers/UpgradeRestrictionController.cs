using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers;

using Upgrade = XaphanModule.Upgrades;

[Tracked]
[CustomEntity("XaphanHelper/UpgradeRestrictionController")]
public class UpgradeRestrictionController : Entity{
	// basic idea: a list of grouped upgrades + list of ignored upgrades
	public readonly ISet<ISet<Upgrade>> Groups = new HashSet<ISet<Upgrade>>();
	public readonly ISet<Upgrade> Ignored = new HashSet<Upgrade>();
	public readonly int Max;

	public UpgradeRestrictionController(EntityData data, Vector2 offset) : base(data.Position + offset){
		Max = data.Int("max");
		
		string groups = data.Attr("groups").Trim();
		string ignored = data.Attr("ignored").Trim();

		if(groups.Length > 0)
			foreach(string group in groups.Split(',')){
				string[] upgrades = group.Trim().Split('+');
				ISet<Upgrade> newGroup = new HashSet<Upgrade>();
				foreach(var upgradeName in upgrades)
					if(Enum.TryParse(upgradeName, true, out Upgrade upgrade))
						newGroup.Add(upgrade);
					else
						Logger.Log(LogLevel.Warn, "XaphanHelper",
							$"Upgrade Restriction Controller #${data.ID} mentions invalid upgrade ${upgradeName}, ignoring");

				Groups.Add(newGroup);
			}

		if(ignored.Length > 0)
			foreach(var ignoredUpgradeName in ignored.Split(','))
				if(Enum.TryParse(ignoredUpgradeName, true, out Upgrade upgrade))
					Ignored.Add(upgrade);
				else
					Logger.Log(LogLevel.Warn, "XaphanHelper",
						$"Upgrade Restriction Controller #${data.ID} mentions invalid upgrade ${ignoredUpgradeName}, ignoring");
	}

	public static UpgradeRestrictionController GetFrom(Scene s){
		return s.Tracker.GetEntity<UpgradeRestrictionController>();
	}

	public static List<Upgrade> Collected() => (
		from upgradeType
			in XaphanModule.Instance.UpgradeHandlers
		where upgradeType.Value.GetValue() != upgradeType.Value.GetDefaultValue()
		select upgradeType.Key
	).ToList();

	public static bool CanObtainIn(Scene s, Upgrade u, List<Upgrade> existing){
		return GetFrom(s)?.AllowsObtaining(u, existing) ?? true;
	}
	
	public bool AllowsObtaining(Upgrade u, List<Upgrade> existing){
		// if `u` is an ignored upgrade, we don't care
		if(Ignored.Contains(u))
			return true;
		// otherwise, check if adding it would exceed the maximum
		return SlotsUsed(new List<Upgrade>(existing){ u }) <= Max;
	}

	public int SlotsUsed(List<Upgrade> upgrades){
		// when we encounter an upgrade in a group, add all of the other upgrades in that group as freebies
		// TODO: upgrades in multiple groups
		ISet<Upgrade> seen = new HashSet<Upgrade>();
		int used = 0;
		foreach(Upgrade upgrade in upgrades)
			if(!Ignored.Contains(upgrade) && !seen.Contains(upgrade)){
				seen.Add(upgrade);
				used++;

				foreach(ISet<Upgrade> group in Groups)
					if(group.Contains(upgrade))
						seen.UnionWith(group);
			}
		return used;
	}
}