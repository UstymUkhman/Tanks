using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float endDelay = 3.0f;
    public int numRoundsToWin = 5;
    public float startDelay = 3.0f;

    public Text messageText;
    public TankManager[] tanks;
    public GameObject tankPrefab;
    public CameraControl cameraControl;

    private int roundNumber = 0;
    private WaitForSeconds endWait;
    private WaitForSeconds startWait;

    private TankManager gameWinner = null;
    private TankManager roundWinner = null;


    private void Start()
    {
        endWait = new WaitForSeconds(endDelay);
        startWait = new WaitForSeconds(startDelay);
        
        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].instance = Instantiate(tankPrefab, tanks[i].spawnPoint.position, tanks[i].spawnPoint.rotation) as GameObject;
            tanks[i].playerNumber = i + 1;
            tanks[i].Setup();
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = tanks[i].instance.transform;
        }

        cameraControl.targets = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (gameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        
        cameraControl.SetStartPositionAndSize();
        messageText.text = "ROUND " + ++roundNumber;

        yield return startWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        messageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        roundWinner = GetRoundWinner();

        if (roundWinner != null)
        {
            roundWinner.wins++;
        }

        gameWinner = GetGameWinner();
        messageText.text = EndMessage();

        yield return endWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].instance.activeSelf)
            {
                numTanksLeft++;
            }
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].instance.activeSelf)
            {
                return tanks[i];
            }
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].wins == numRoundsToWin)
            {
                return tanks[i];
            }
        }

        return null;
    }

    private string EndMessage()
    {
        string message = "DRAW!";

        if (roundWinner != null)
        {
            message = roundWinner.coloredPlayerText + " WINS THE ROUND!";
        }

        message += "\n\n\n\n";

        for (int i = 0; i < tanks.Length; i++)
        {
            message += tanks[i].coloredPlayerText + ": " + tanks[i].wins + " WINS\n";
        }

        if (gameWinner != null)
        {
            message = gameWinner.coloredPlayerText + " WINS THE GAME!";
        }

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].Reset();
        }
    }
    
    private void EnableTankControl()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].DisableControl();
        }
    }
}