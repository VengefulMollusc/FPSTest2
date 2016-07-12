using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	GameObject playerUIPrefab;
	private GameObject playerUIInstance;

	Camera sceneCamera;

	void Start (){
		// if we're not the local player, disable all the components in the array
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemoteLayer ();
		} else {
			sceneCamera = Camera.main;
			if (sceneCamera != null){
				sceneCamera.gameObject.SetActive (false);
			}

			// disable local player graphics
			SetLayerRecursive(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			// create player ui
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;
		}

		GetComponent<Player> ().Setup ();
	}

	void SetLayerRecursive(GameObject obj, int newLayer){
		obj.layer = newLayer;

		foreach (Transform child in obj.transform) {
			SetLayerRecursive (child.gameObject, newLayer);
		}
	}

	public override void OnStartClient(){
		base.OnStartClient ();

		string _netID = GetComponent<NetworkIdentity> ().netId.ToString();
		Player _player = GetComponent<Player> ();

		GameManager.RegisterPlayer (_netID, _player);
	}

	void AssignRemoteLayer(){
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void DisableComponents (){
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void OnDisable(){
		Destroy (playerUIInstance);

		if (sceneCamera != null){
			sceneCamera.gameObject.SetActive (true);
		}

		GameManager.UnregisterPlayer (transform.name);
	}

}
