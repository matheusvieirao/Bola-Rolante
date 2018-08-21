using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	private static LevelManager instance;
	public static LevelManager Instance{get{ return instance;}} //pesquisar para entender

	public GameObject pauseMenu;

	public Transform respawnPoint;
	private GameObject player;

	private float startTime;
	public float silverTime;
	public float goldTime;

	void Start () {
		instance = this;
		pauseMenu.SetActive (false);
		startTime = Time.time;
		player = GameObject.FindGameObjectWithTag ("Player");
		player.transform.position = respawnPoint.position;
	}

	void Update () {

		if (player.transform.position.y < -30) {
			Death ();
		}

	}

	public void TogglePauseMenu() {
		pauseMenu.SetActive (!pauseMenu.activeSelf);
	}

	public void ToMenu(){
		SceneManager.LoadScene ("Main Menu");
	}

	public void Death(){
		player.transform.position = respawnPoint.position;
		Rigidbody rigbod = player.GetComponent<Rigidbody> ();
		rigbod.angularVelocity = Vector3.zero;
		rigbod.velocity = Vector3.zero;
	}

	public void Victory() 	{
		float duration = Time.time - startTime;
		if (duration < goldTime) {
			GameManager.Instance.currency += 50;
		}
		else if (duration < silverTime) {
			GameManager.Instance.currency += 25;
		}
		else {
			GameManager.Instance.currency += 10;
		}
		GameManager.Instance.Save ();

		LevelData levelData = new LevelData (SceneManager.GetActiveScene().name);
		string saveString = "" ;
		saveString += duration.ToString ();
		saveString += "&";
		saveString += silverTime.ToString ();
		saveString += "&";
		saveString += goldTime.ToString ();
		if(levelData.BestTime == 0.0f || duration < levelData.BestTime)
			PlayerPrefs.SetString (SceneManager.GetActiveScene ().name, saveString);

		SceneManager.LoadScene ("Main Menu");
	}
}
