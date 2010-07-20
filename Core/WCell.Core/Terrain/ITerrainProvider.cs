using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCell.Constants.World;

namespace WCell.Core.TerrainAnalysis
{
	public interface ITerrainProvider
	{
		ITerrain CreateTerrain(MapId id);
	}
}