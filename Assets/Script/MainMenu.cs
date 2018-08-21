using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelData {
	public LevelData (string levelName) {
		string data = PlayerPrefs.GetString (levelName);
		if (data == "") {
			BestTime = 0;
			return;
		}
		
		string[] allData = data.Split('&');
		BestTime = float.Parse (allData [0]);
		SilverTime = float.Parse (allData [1]);
		GoldTime = float.Parse (allData [2]);
	}

	public float BestTime { set; get; }
	public float SilverTime { set; get; }
	public float GoldTime { set; get; }

}

public class MainMenu : MonoBehaviour {
	private const float CAMERA_TRANSITION_SPEED = 4.0f;

	public GameObject levelButtonPrefab;
	public GameObject levelButtonContainer;
	public GameObject shopButtonPrefab;
	public GameObject shopButtonContainer;
	public Text currencyText;

	public Material playerMaterial;

	private Transform cameraTransform;
	private Transform cameraDesiredLookAt = null;

	private bool nextLevelLocked = false;

	private int[] skinCosts = { 0, 150, 150, 150, 300, 300, 300, 300, 500, 500, 500, 500, 1000, 1500, 2000, 5000 };

	private void Start() {
		ChangePlayerSkin (GameManager.Instance.currentSkinIndex);
		currencyText.text = "Golpinhos: " + GameManager.Instance.currency.ToString ();
		cameraTransform = Camera.main.transform;

		Sprite[] thumbnails = Resources.LoadAll<Sprite> ("Levels");
		//loop nos leveis
		foreach (Sprite thumbnail in thumbnails) {
			GameObject button = Instantiate (levelButtonPrefab) as GameObject;
			button.GetComponent<Image> ().sprite = thumbnail;
			button.transform.SetParent (levelButtonContainer.transform, false);
			LevelData levelData = new LevelData (thumbnail.name);
			button.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = (levelData.BestTime != 0.0f) ? levelData.BestTime.ToString("f") : "";

			button.transform.GetChild (1).GetComponent<Image> ().enabled = nextLevelLocked; //mostrar imagem cinza nos leveis fechados
			button.GetComponent<Button> ().interactable = !nextLevelLocked; //nao poder clicar nos leveis fechados.

			if (levelData.BestTime == 0.0f) {
				nextLevelLocked = true;
			}

			string sceneName = thumbnail.name;
			button.GetComponent<Button> ().onClick.AddListener (() => LoadLevel(sceneName) );
		}

		//loop nas skins
		int textureIndex = 0;
		Sprite[] textures = Resources.LoadAll<Sprite> ("Player");
		foreach (Sprite texture in textures) {
			GameObject button = Instantiate (shopButtonPrefab) as GameObject;
			button.GetComponent<Image> ().sprite = texture;
			button.transform.SetParent (shopButtonContainer.transform, false);

			int index = textureIndex;
			button.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skinCosts[index].ToString(); //preço das skins
			button.GetComponent<Button> ().onClick.AddListener(() => ChangePlayerSkin(index)); //mudar skin do jogador
			if ((GameManager.Instance.skinAvailability & 1 << index) == 1 << index) {
				button.transform.GetChild (0).gameObject.SetActive (false);
			}
			textureIndex++;
		}
	}

	private void LoadLevel(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void LookAtMenu(Transform menuTransform) {
		cameraDesiredLookAt = menuTransform;
	}

	private void Update() {

		if (cameraDesiredLookAt != null) {
			if (cameraTransform.rotation != cameraDesiredLookAt.rotation) {
				cameraTransform.rotation = Quaternion.Slerp (cameraTransform.rotation, cameraDesiredLookAt.rotation, CAMERA_TRANSITION_SPEED * Time.deltaTime);
			}
		}
	}

	private void ChangePlayerSkin(int index) {
		//usou bitflag para saber quais itens já foram compros 
		if ((GameManager.Instance.skinAvailability & 1 << index) == 1 << index) {
			float x = (index % 4) * 0.25f;
			float y = ((int)index / 4) * 0.25f;

			y = 0.75f - y;

			playerMaterial.SetTextureOffset ("_MainTex", new Vector2 (x, y));
			GameManager.Instance.currentSkinIndex = index;
			GameManager.Instance.Save ();
		} else {
			//voce nao tem a skin. quer comprar?
			int cost = skinCosts[index];

			if (GameManager.Instance.currency >= cost) {
				GameManager.Instance.currency -= cost;
				GameManager.Instance.skinAvailability += 1 << index; 
				GameManager.Instance.Save ();
				currencyText.text = "Golpinhos: " + GameManager.Instance.currency.ToString ();
				if (shopButtonContainer.transform.childCount > 0){
					shopButtonContainer.transform.GetChild (index).GetChild (0).gameObject.SetActive (false);
				}
				ChangePlayerSkin (index);
			}
		}
	}
}
