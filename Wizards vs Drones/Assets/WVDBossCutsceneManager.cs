using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDBossCutsceneManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    List<GameObject> _barriers;
    [SerializeField]
    WVDBoss _bossScript;
    [SerializeField]
    GameObject _player;
    WVDTutorialManager _tutorialManager;

    [Header("Starting Positions")]
    [SerializeField]
    List<Transform> _bossStartingPositions;
    [SerializeField]
    Transform _playerStartingPosition;

    [Header("Cameras")]
    [SerializeField]
    GameObject _cutsceneCamera;
    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    GameObject _canvas;
    [SerializeField]
    GameObject _powerUpIcon;
    [SerializeField]
    WVDPlayerPowerUpManager _powerUpManager;
    [SerializeField]
    GameObject _bossHealthBar;

    void Start()
    {
        _tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<WVDTutorialManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartBossCutscene()
    {
        WVDFunctionsCheck.InCutscene = true;
        _bossScript.CalculateMovementVectorToDoor();
        _bossScript.CurrentBossState = WVDBoss.BossState.DungeonFlyToDoor;
        _playerCamera.enabled = false;
        _cutsceneCamera.SetActive(true);
        _canvas.SetActive(false);
        _player.GetComponent<CharacterController>().enabled = false;
        _player.transform.position = _playerStartingPosition.position;
        _player.GetComponent<CharacterController>().enabled = true;
    }

    public void EndBossCutscene()
    {
        // i.e. put player next to main gate and boss in random corner and switch cameras back

        WVDFunctionsCheck.InCutscene = false;
        foreach(GameObject barrier in _barriers)
        {
            barrier.SetActive(true);
        }
        int rand = Random.Range(0, _bossStartingPositions.Count);
        _bossScript.gameObject.transform.position =_bossStartingPositions[rand].position;
        _bossScript.SetCurrentBattleWayPoint(_bossStartingPositions[rand]);
        _playerCamera.enabled = true;
        _cutsceneCamera.SetActive(false);
        _canvas.SetActive(true);
        _bossHealthBar.SetActive(true);
        _powerUpIcon.SetActive(false);
        _powerUpManager.ResetPowerUps();
        _tutorialManager.DisplayTutorial(WVDTutorialManager.TutorialPart.Boss, 1.0f);
    }
}
