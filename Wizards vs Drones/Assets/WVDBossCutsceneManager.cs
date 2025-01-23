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

    [Header("Starting Positions")]
    [SerializeField]
    List<Transform> _bossStartingPositions;
    [SerializeField]
    Transform _playerStartingPosition;

    [Header("Cameras")]
    [SerializeField]
    GameObject _cutsceneCamera;
    [SerializeField]
    GameObject _playerCamera;
    [SerializeField]
    GameObject _canvas;
    void Start()
    {
        
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
        _playerCamera.SetActive(false);
        _cutsceneCamera.SetActive(true);
        _canvas.SetActive(false);
    }

    public void EndBossCutscene()
    {
        // i.e. put player next to main gate and boss in random corner and switch cameras back

        WVDFunctionsCheck.InCutscene = false;
        foreach(GameObject barrier in _barriers)
        {
            barrier.SetActive(true);
        }
        _bossScript.gameObject.transform.position =_bossStartingPositions[Random.Range(0, _bossStartingPositions.Count)].position;
        _player.GetComponent<CharacterController>().enabled = false;
        _player.transform.position = _playerStartingPosition.position;
        _player.GetComponent<CharacterController>().enabled = true;
        _playerCamera.SetActive(true);
        _cutsceneCamera.SetActive(false);
        _canvas.SetActive(true);
    }
}
