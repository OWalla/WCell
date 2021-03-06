using WCell.RealmServer.Entities;
using WCell.RealmServer.Instances;
using WCell.Util.Commands;
using WCell.RealmServer.Global;
using WCell.Constants.World;
using WCell.Constants;
using System.Collections.Generic;

namespace WCell.RealmServer.Commands
{
	public class InstanceCommand : RealmServerCommand
	{
		protected InstanceCommand() { }

		protected override void Initialize()
		{
			Init("Instance", "Inst");
			EnglishDescription = "Provides some Commands to manage and use Instances.";
		}

		public static InstancedRegion GetInstance(CmdTrigger<RealmServerCmdArgs> trigger)
		{
			if (!trigger.Text.HasNext)
			{
				trigger.Reply("No MapId specified.");
			}

			var mapId = trigger.Text.NextEnum(MapId.End);
			if (mapId == MapId.End)
			{
				trigger.Reply("Invalid MapId.");
				return null;
			}

			if (!trigger.Text.HasNext)
			{
				trigger.Reply("No Instance-Id specified.");
			}

			var id = trigger.Text.NextUInt();
			var instance = World.GetInstance(mapId, id);
			if (instance == null)
			{
				trigger.Reply("Instance does not exist: {0} (#{1})", mapId, id);
			}
			return instance;
		}

		#region List
		public class InstanceListAllCommand : SubCommand
		{
			protected InstanceListAllCommand() { }

			protected override void Initialize()
			{
				Init("List", "L");
				EnglishParamInfo = "[<MapId>]";
				EnglishDescription = "Lists all active Instances, or those of the given Map.";
			}

			public override void Process(CmdTrigger<RealmServerCmdArgs> trigger)
			{
				var list = new List<InstancedRegion>(50);
				if (trigger.Text.HasNext)
				{
					var mapId = trigger.Text.NextEnum(MapId.End);
					if (mapId == MapId.End)
					{
						trigger.Reply("Invalid MapId.");
						return;
					}
					var instances = World.GetInstances(mapId);
					foreach (var inst in instances)
					{
						if (inst != null)
						{
							list.Add(inst);
						}
					}
				}
				else
				{
					var instances = World.GetAllInstances();

					foreach (var arr in instances)
					{
						if (arr == null)
						{
							continue;
						}
						foreach (var inst in arr)
						{
							if (inst != null && !inst.IsBattleground)
							{
								list.Add(inst);
							}
						}
					}
				}

				trigger.Reply("== [ Current Instances: {0} ] ==", list.Count);
				for (var i = 0; i < list.Count; i++)
				{
					var inst = list[i];
					trigger.Reply(inst.ToString());
				}
			}
		}
		#endregion

		#region Create
		public class InstanceCreateCommand : SubCommand
		{
			protected InstanceCreateCommand() { }

			protected override void Initialize()
			{
				Init("Create", "C");
				EnglishParamInfo = "[-e] <MapId>";
				EnglishDescription = "Creates a new Instance of the given Map. -e enters it right away.";
			}

			public override void Process(CmdTrigger<RealmServerCmdArgs> trigger)
			{
				var chr = trigger.Args.Target as Character;
				if (chr == null)
				{
					trigger.Reply("Must use this command on a Character.");
					return;
				}
				var mod = trigger.Text.NextModifiers();
				var mapid = trigger.Text.NextEnum(MapId.End);
				if (mapid == MapId.End)
				{
					trigger.Reply("Invalid MapId.");
					return;
				}

				var region = World.GetRegionInfo(mapid);

				if (region != null && region.IsInstance)
				{
					var instance = InstanceMgr.CreateInstance(chr, region.InstanceTemplate, chr.GetInstanceDifficulty(region.IsRaid));
					if (instance != null)
					{
						trigger.Reply("Instance created: " + instance);
						if (mod == "e")
						{
							if (trigger.Args.Target is Character)
							{
								instance.TeleportInside((Character)trigger.Args.Target);
							}
						}
					}
					else
					{
						trigger.Reply("Unable to create Instance of: " + region);
					}
				}
				else
				{
					trigger.Reply("Invalid MapId.");
				}
			}
		}
		#endregion

		#region Enter
		public class InstanceEnterCommand : SubCommand
		{
			protected InstanceEnterCommand() { }

			protected override void Initialize()
			{
				Init("Enter", "E");
				EnglishParamInfo = "<MapId> <InstanceId> [<entrance>]";
			}

			public override void Process(CmdTrigger<RealmServerCmdArgs> trigger)
			{
				var instance = GetInstance(trigger);
				if (instance != null)
				{
					var entrance = trigger.Text.NextInt(0);
					instance.TeleportInside(trigger.Args.Character, entrance);
				}
			}
		}
		#endregion

		#region Delete
		public class InstanceDeleteCommand : SubCommand
		{
			protected InstanceDeleteCommand() { }

			protected override void Initialize()
			{
				Init("Delete", "Del");
				EnglishParamInfo = "[<MapId> <InstanceId>]";
				EnglishDescription = "Delets the Instance of the given Map with the given Id, or the current one if no arguments are supplied.";
			}

			public override void Process(CmdTrigger<RealmServerCmdArgs> trigger)
			{
				InstancedRegion instance;
				if (!trigger.Text.HasNext && trigger.Args.Character != null)
				{
					instance = trigger.Args.Character.Region as InstancedRegion;
					if (instance == null)
					{
						trigger.Reply("Current Region is not an Instance.");
						return;
					}
				}
				else
				{
					instance = GetInstance(trigger);
					if (instance == null)
					{
						return;
					}
				}

				instance.Delete();
				trigger.Reply("Instance Deleted");
			}
		}
		#endregion
	}
}