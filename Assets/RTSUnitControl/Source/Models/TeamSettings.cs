using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamSettings {

	private static readonly Dictionary<int, TeamSettings> teamSettings = new Dictionary<int, TeamSettings>{ 
		{0, new TeamSettings(0, Color.red)},
		{1, new TeamSettings(1, Color.blue)}
	}; 

	public int ID { get; set; }
	public Color Color { get; set; }

	public TeamSettings(int id, Color color) {
		ID = id;
		Color = color;
	}

	public static TeamSettings GetTeamSettingsForId(int id) {
		TeamSettings settings = null;
		teamSettings.TryGetValue (id, out settings);

		return settings;
	}
}
