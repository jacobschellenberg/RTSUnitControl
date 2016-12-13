using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour {

	private static GameController instance;
	public static GameController Instance {
		get { 
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> () as GameController;

			return instance;
		}
	}

	public PlayerController ClientPlayer { get; private set; }
	public List<PlayerController> Players { get; private set; }

	[SerializeField] private PlayerController clientPlayer;
	[SerializeField] private List<PlayerController> players;

	private void Awake() {
		Players = players != null ? players : new List<PlayerController> ();
		ClientPlayer = Players.FirstOrDefault(_ => _ == clientPlayer);
	}

	private void Start() {
		Players.ForEach (_ => {
			_.SetTeam(TeamSettings.GetTeamSettingsForId(_.TeamId));
		});
	}
}
