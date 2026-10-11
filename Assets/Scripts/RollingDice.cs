using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the dice rolling mechanics, animations, and determines 
/// if a player has any valid moves available based on the roll.
/// </summary>
public class RollingDice : MonoBehaviour
{
    // Audio components for the dice sound effects
    public AudioSource audioSource;
    public AudioClip diceRollSound;
    
    // Visual components for the dice faces and animations
    public Sprite[] sprites;
    public SpriteRenderer rollingDiceAnimation;
    public SpriteRenderer numberHolder;

    // Dice roll variables
    public int number;
    public float rollDuration = 0.8f;

    // Player state trackers
    private int outPlayer;
    private int completePlayer;

    // List maintaining the current player's pieces
    public List<PlayerController> playerList = new List<PlayerController>();

    // Path tracking for the active player
    private PathPoint[] currentPathPoints;
    private PlayerController currentPlayers;
    private PathObjectParent pathObjectParent;

    /// <summary>
    /// Initializes the path object parent reference on startup.
    /// </summary>
    void Start()
    {
        // Locate the main path manager in the scene
        pathObjectParent = FindAnyObjectByType<PathObjectParent>();

        // Safety check to ensure the path manager exists
        if (pathObjectParent == null)
        {
            Debug.LogError("PathObjectParent not found!");
        }
    }

    void Update()
    {
        // Reserved for future per-frame logic updates
    }

    /// <summary>
    /// Triggered when the user clicks on the dice object.
    /// Initiates the roll if the game state allows it.
    /// </summary>
    public void OnMouseDown()
    {
        // Abort if the GameManager instance hasn't been initialized
        if (GameManager.instance == null) return;

        // Abort if it is not currently a valid time to roll the dice
        if (!GameManager.instance.canRoll) return;

        // Begin the dice rolling coroutine
        StartCoroutine(RollDiceAnim());
    }

    /// <summary>
    /// Handles the visual delay, audio playing, and random number generation 
    /// for the dice roll, followed by move validation.
    /// </summary>
    IEnumerator RollDiceAnim()
    {
        // Lock game state to prevent multiple actions during the animation
        GameManager.instance.canRoll = false;
        GameManager.instance.canPlayerMove = false;

        // Swap sprites to show the rolling animation instead of the static number
        rollingDiceAnimation.gameObject.SetActive(true);
        numberHolder.gameObject.SetActive(false);

        // Play the rolling sound effect if components are assigned
        if (audioSource && diceRollSound)
        {
            audioSource.PlayOneShot(diceRollSound);
        }
        
        // Wait for the duration of the rolling animation
        yield return new WaitForSeconds(rollDuration);
        
        // Generate a random index between 0 and 5
        number = Random.Range(0, 6);
        
        // Update the visual sprite to match the rolled index
        numberHolder.sprite = sprites[number];

        // Increment to convert from 0-based index to standard 1-6 dice value
        number++;

        // Update the GameManager with the current roll value and dice reference
        GameManager.instance.numberOfSteps = number;
        GameManager.instance.rollingDice = this;

        // Swap visual states back to the static rolled number
        rollingDiceAnimation.gameObject.SetActive(false);
        numberHolder.gameObject.SetActive(true);

        // Identify and load the current player's pieces into the tracking list
        OutPlayer();

        // Evaluate whether the player has any legal moves with the current roll
        bool canMove = playerCanMove();

        // If a move is possible, unlock the player movement state and wait for their input
        if (canMove)
        {
            GameManager.instance.canPlayerMove = true;
            yield break; // Exit the coroutine early; do not transfer the turn yet
        }

        // If no move is possible, keep movement locked
        GameManager.instance.canPlayerMove = false;

        // Brief pause so the player can see the roll before the turn is skipped
        yield return new WaitForSeconds(1f);

        // Flag the system to pass the turn to the next player
        GameManager.instance.transferDice = true;
        GameManager.instance.rollDiceManager();
    }

    /// <summary>
    /// Determines which color's turn it is, fetches their specific path points, 
    /// and populates the playerList with all pieces belonging to that color.
    /// </summary>
    public void OutPlayer()
    {
        // Reset tracking variables for a fresh evaluation
        playerList.Clear();
        outPlayer = 0;
        completePlayer = 0;
        currentPlayers = null;

        // Ensure prerequisite managers exist
        if (GameManager.instance == null) return;
        if (pathObjectParent == null)
        {
            Debug.LogError("PathObjectParent is missing!");
            return;
        }

        string requiredColor = "";

        // Determine active player color based on which dice was rolled
        // -- BLUE PLAYER SETUP --
        if (GameManager.instance.rollingDice == GameManager.instance.manageRollingDice[0])
        {
            requiredColor = "blue_piece";
            currentPathPoints = pathObjectParent.BluePathPoints;
            outPlayer = GameManager.instance.blueOutPlayer;
            completePlayer = GameManager.instance.blueCompletePlayer;
        }
        // -- RED PLAYER SETUP --
        else if (GameManager.instance.rollingDice == GameManager.instance.manageRollingDice[1])
        {
            requiredColor = "red_piece";
            currentPathPoints = pathObjectParent.RedPathPoints;
            outPlayer = GameManager.instance.redOutPlayer;
            completePlayer = GameManager.instance.redCompletePlayer;
        }
        // -- GREEN PLAYER SETUP --
        else if (GameManager.instance.rollingDice == GameManager.instance.manageRollingDice[2])
        {
            requiredColor = "green_piece";
            currentPathPoints = pathObjectParent.GreenPathPoints;
            outPlayer = GameManager.instance.greenOutPlayer;
            completePlayer = GameManager.instance.greenCompletePlayer;
        }
        // -- YELLOW PLAYER SETUP --
        else if (GameManager.instance.rollingDice == GameManager.instance.manageRollingDice[3])
        {
            requiredColor = "yellow_piece";
            currentPathPoints = pathObjectParent.YellowPathPoints;
            outPlayer = GameManager.instance.yellowOutPlayer;
            completePlayer = GameManager.instance.yellowCompletePlayer;
        }

        // Failsafe in case the dice doesn't match any known player array index
        if (requiredColor == "")
        {
            Debug.LogError("Could not determine current player's color!");
            return;
        }

        // Fetch every player controller currently in the scene
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        // Filter out pieces that belong to the current active player color
        foreach (PlayerController player in allPlayers)
        {
            if (player.name.Contains(requiredColor))
            {
                playerList.Add(player);
            }
        }

        // Debug logging for tracking piece detection
        Debug.Log("Current player: " + requiredColor + " | Pieces found: " + playerList.Count);
    }

    /// <summary>
    /// Checks all pieces of the active player to see if at least one 
    /// is capable of making a valid move based on the current dice roll.
    /// </summary>
    bool playerCanMove()
    {
        // If path points are missing, movement is inherently impossible
        if (currentPathPoints == null || currentPathPoints.Length == 0)
        {
            return false;
        }

        // Iterate through all active pieces for the current player
        foreach (PlayerController player in playerList)
        {
            // Skip null references
            if (player == null) continue;

            // Only check pieces that are currently active/ready on the board
            if (!player.isReady) continue;

            // Consult the PlayerController to verify if the specific step count is legal
            bool canMove = player.isPlayerAvailableToMove(
                player.numberOfStepsAlready,
                GameManager.instance.numberOfSteps,
                currentPathPoints
            );

            // If even a single piece can move, return true immediately
            if (canMove)
            {
                currentPlayers = player;
                return true;
            }
        }

        // Calculate how many pieces have already been activated or finished
        int piecesAlreadyUsed = outPlayer + completePlayer;

        // If a 6 was rolled and the player still has pieces in the base, they can spawn a new piece
        if (GameManager.instance.numberOfSteps == 6 && piecesAlreadyUsed < 4)
        {
            return true;
        }

        // Default fallback: no valid moves exist
        return false;
    }
}