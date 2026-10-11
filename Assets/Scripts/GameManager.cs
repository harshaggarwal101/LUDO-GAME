using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core controller for managing the game's state, player turns, UI transitions, 
/// and global audio playback. Handles the setup for 2, 3, or 4 player modes.
/// </summary>


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("UI Panels")]
    public GameObject playerSelectionPanel;
    public GameObject gamePanel;

    [Header("Audio")]
    public AudioSource gameAudioSource;
    public AudioClip homeReachedSound;
    public AudioClip playerCutSound;
    public AudioClip playerMoveSound;
    public AudioClip buttonPressSound;
    
    [Header("Game State")]
    public int numberOfSteps;
    public RollingDice rollingDice;
    public bool canPlayerMove;
    public List<PathPoint> pathPointList = new List<PathPoint>();
    public bool canRoll = true;
    public RollingDice[] manageRollingDice;
    public bool transferDice;
    public bool selfDice;
    public int totalPlayers = 4;

    // =========================================================
    // PLAYER COUNTERS
    // =========================================================
    public int blueOutPlayer;
    public int redOutPlayer;
    public int greenOutPlayer;
    public int yellowOutPlayer;

    public int blueCompletePlayer;
    public int redCompletePlayer;
    public int greenCompletePlayer;
    public int yellowCompletePlayer;

    // =========================================================
    // PLAYER LISTS
    // =========================================================
    public List<PlayerController> bluePlayerList;
    public List<PlayerController> redPlayerList;
    public List<PlayerController> greenPlayerList;
    public List<PlayerController> yellowPlayerList;
    public List<GameObject> playerHome;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canPlayerMove = false;
        canRoll = true;
        transferDice = false;
        
        // Ensure we start on the selection screen if panels are assigned
        if(playerSelectionPanel != null && gamePanel != null)
        {
            playerSelectionPanel.SetActive(true);
            gamePanel.SetActive(false);
        }
    }

    // =========================================================
    // START GAME FROM UI
    // =========================================================
    public void StartGameWithPlayers(int players)
    {
        totalPlayers = players;
        Debug.Log("Starting game with " + totalPlayers + " players");

        // Swap the active panels
        playerSelectionPanel.SetActive(false);
        gamePanel.SetActive(true);

        // Hide the unused pieces and set up the dice
        SetupBoard();
    }
    
    private void SetupBoard()
    {
        // 1. Activate all pieces as a baseline
        TogglePlayerPieces(bluePlayerList, true);
        TogglePlayerPieces(redPlayerList, true);
        TogglePlayerPieces(greenPlayerList, true);
        TogglePlayerPieces(yellowPlayerList, true);

        // 2. Hide pieces based on the selected player count
        if (totalPlayers == 2)
        {
            // 2-player mode uses index 0 and 2. Hide Red (1) and Yellow (3).
            TogglePlayerPieces(redPlayerList, false);
            TogglePlayerPieces(yellowPlayerList, false);
        }
        else if (totalPlayers == 3)
        {
            // 3-player mode uses index 0, 1, and 2. Hide Yellow (3).
            TogglePlayerPieces(yellowPlayerList, false);
        }

        // 3. Reset the dice so only the first player's dice is visible at the start
        for (int i = 0; i < manageRollingDice.Length; i++)
        {
            if (manageRollingDice[i] != null)
            {
                manageRollingDice[i].gameObject.SetActive(false);
            }
        }
        
        // Activate Player 1's dice
        if (manageRollingDice.Length > 0 && manageRollingDice[0] != null)
        {
            manageRollingDice[0].gameObject.SetActive(true);
            rollingDice = manageRollingDice[0];
        }
    }

    // Helper function to easily turn a list of pieces on or off
    private void TogglePlayerPieces(List<PlayerController> piecesList, bool isVisible)
    {
        foreach (PlayerController piece in piecesList)
        {
            if (piece != null)
            {
                piece.gameObject.SetActive(isVisible);
            }
        }
    }

    // =========================================================
    // PATH POINT MANAGEMENT
    // =========================================================
    public void addPathPoint(PathPoint pathPoint)
    {
        if (pathPoint == null) return;
        if (!pathPointList.Contains(pathPoint))
        {
            pathPointList.Add(pathPoint);
        }
    }

    public void removePathPoint(PathPoint pathPoint)
    {
        if (pathPoint == null) return;
        if (pathPointList.Contains(pathPoint))
        {
            pathPointList.Remove(pathPoint);
        }
    }

    // =========================================================
    // PREPARE DICE FOR NEXT ROLL
    // =========================================================
    public void rollDiceManager()
    {
        if (transferDice)
        {
            shiftDice();
        }

        canRoll = true;
        canPlayerMove = false;
        transferDice = false;
        numberOfSteps = 0;
    }

    // =========================================================
    // MOVE DICE TO NEXT PLAYER
    // =========================================================
    public void shiftDice()
    {
        int nextDice;

        // TWO PLAYERS
        if (totalPlayers == 2)
        {
            if (rollingDice == manageRollingDice[0])
            {
                manageRollingDice[0].gameObject.SetActive(false);
                manageRollingDice[2].gameObject.SetActive(true);
                rollingDice = manageRollingDice[2];
            }
            else
            {
                manageRollingDice[0].gameObject.SetActive(true);
                manageRollingDice[2].gameObject.SetActive(false);
                rollingDice = manageRollingDice[0];
            }
            return;
        }

        // THREE PLAYERS
        if (totalPlayers == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (rollingDice == manageRollingDice[i])
                {
                    nextDice = (i == 2) ? 0 : i + 1;
                    
                    manageRollingDice[i].gameObject.SetActive(false);
                    manageRollingDice[nextDice].gameObject.SetActive(true);
                    rollingDice = manageRollingDice[nextDice];
                    break;
                }
            }
            return;
        }

        // FOUR PLAYERS
        if (totalPlayers == 4)
        {
            for (int i = 0; i < manageRollingDice.Length; i++)
            {
                if (rollingDice == manageRollingDice[i])
                {
                    // Fixed the typo here
                    nextDice = (i == manageRollingDice.Length - 1) ? 0 : i + 1;
                    
                    manageRollingDice[i].gameObject.SetActive(false);
                    manageRollingDice[nextDice].gameObject.SetActive(true);
                    rollingDice = manageRollingDice[nextDice];
                    break;
                }
            }
        }
    }

    // =========================================================
    // AUDIO WRAPPERS
    // =========================================================
    
    /// Triggers the success chime when a piece docks in the center base.
    public void PlayHomeReachedSound()
    {
        if (gameAudioSource != null && homeReachedSound != null)
        {
            gameAudioSource.PlayOneShot(homeReachedSound);
        }
    }
    /// Triggers the specific sound effect used when an opponent piece is captured.
    public void PlayPlayerCutSound()
    {
        if (gameAudioSource != null && playerCutSound != null)
        {
            gameAudioSource.PlayOneShot(playerCutSound);
        }
    }
    /// Triggers the basic step audio for normal piece movement.
    public void PlayPlayerMoveSound()
    {
        if (gameAudioSource != null && playerMoveSound != null)
        {
            gameAudioSource.PlayOneShot(playerMoveSound);
        }
    }

    public void PlayButtonClickSound()
    {
        gameAudioSource.PlayOneShot(buttonPressSound);
    }
}