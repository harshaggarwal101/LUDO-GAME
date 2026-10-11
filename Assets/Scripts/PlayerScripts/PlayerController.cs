using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the individual state, movement animation, and logic for a single player piece.
/// Interacts heavily with the GameManager to coordinate turns and path progression.
/// </summary>

public class PlayerController : MonoBehaviour
{
    public bool move;
    public bool isReady;

    public int numberOfSteps;
    public int numberOfStepsAlready;

    public PathObjectParent pathParent;

    public PathPoint prevPathPoint;
    public PathPoint currPathPoint;

    private Coroutine playerMoveCoroutine;


    private void Awake()
    {
        pathParent = FindAnyObjectByType<PathObjectParent>();

        if (pathParent == null)
        {
            Debug.LogError(
                "PathObjectParent not found!",
                this
            );
        }
    }

    // =========================================================
    // BRING PIECE OUT OF BASE
    // =========================================================

    public void makePlayerReadyToMove(
        PathPoint[] pathPointsToMove)
    {
        if (GameManager.instance == null)
            return;


        if (pathPointsToMove == null ||
            pathPointsToMove.Length == 0)
        {
            Debug.LogError(
                "Path points are missing!",
                this
            );

            return;
        }


        if (!GameManager.instance.canPlayerMove)
            return;


        // Piece is now active
        isReady = true;


        // Put piece at its starting path point
        transform.position =
            pathPointsToMove[0].transform.position;
        GameManager.instance.PlayPlayerMoveSound();
        


        // The piece has entered the board
        numberOfStepsAlready = 1;


        prevPathPoint =
            pathPointsToMove[0];

        currPathPoint =
            pathPointsToMove[0];


        // Add player to path point
        currPathPoint.add(this);

        GameManager.instance.addPathPoint(
            currPathPoint
        );


        // A piece coming out happens on a 6.
        // Therefore same player gets to roll again.
        GameManager.instance.transferDice = false;


        // The 6 has now been consumed.
        GameManager.instance.numberOfSteps = 0;


        // Allow another dice roll
        GameManager.instance.rollDiceManager();
    }


    // =========================================================
    // START MOVING A PIECE
    // =========================================================

    public void movePlayer(
        PathPoint[] pathPointsToMove)
    {
        if (GameManager.instance == null)
            return;


        if (!GameManager.instance.canPlayerMove)
            return;


        if (!isReady)
            return;


        if (playerMoveCoroutine != null)
            return;


        if (pathPointsToMove == null ||
            pathPointsToMove.Length == 0)
        {
            Debug.LogError(
                "Path points are missing!",
                this
            );

            return;
        }


        playerMoveCoroutine =
            StartCoroutine(
                MoveSteps(pathPointsToMove)
            );
    }


    // =========================================================
    // MOVE PIECE STEP BY STEP
    // =========================================================

    IEnumerator MoveSteps(
        PathPoint[] pathPointsToMove)
    {
        numberOfSteps =
            GameManager.instance.numberOfSteps;


        // Save dice value BEFORE resetting it
        int diceValue =
            numberOfSteps;


        // =====================================================
        // CHECK WHETHER THIS PIECE CAN MOVE
        // =====================================================

        if (!isPlayerAvailableToMove(
            numberOfStepsAlready,
            numberOfSteps,
            pathPointsToMove))
        {
            // The player selected a piece that cannot move.
            // Keep the same dice result so they can choose
            // another valid piece.
            GameManager.instance.canPlayerMove = true;

            playerMoveCoroutine = null;

            yield break;
        }


        // =====================================================
        // MOVE STEP BY STEP
        // =====================================================

        for (
            int i = numberOfStepsAlready;
            i < numberOfStepsAlready + numberOfSteps;
            i++)
        {
            transform.position =
                pathPointsToMove[i].transform.position;
            GameManager.instance.PlayPlayerMoveSound();

            yield return new WaitForSeconds(0.3f);
        }


        // =====================================================
        // UPDATE PLAYER'S POSITION/PROGRESS
        // =====================================================

        numberOfStepsAlready += numberOfSteps;


        // =====================================================
        // REMOVE FROM PREVIOUS PATH POINT
        // =====================================================

        GameManager.instance.removePathPoint(
            prevPathPoint
        );

        prevPathPoint.removePlayer(this);


        // =====================================================
        // SET CURRENT PATH POINT
        // =====================================================

        currPathPoint =
            pathPointsToMove[
                numberOfStepsAlready - 1
            ];


        // =====================================================
        // ADD PLAYER TO CURRENT PATH POINT
        //
        // true  = normal move
        // false = capture / center point
        // =====================================================

        bool transfer =
            currPathPoint.add(this);


        GameManager.instance.addPathPoint(
            currPathPoint
        );


        prevPathPoint =
            currPathPoint;


        // =====================================================
        // DECIDE WHETHER TO TRANSFER THE TURN
        // =====================================================

        /*
         *
         * NORMAL MOVE + DICE != 6
         * ----------------------
         * Next player gets turn.
         *
         *
         * NORMAL MOVE + DICE == 6
         * -----------------------
         * Same player gets another turn.
         *
         *
         * CAPTURE
         * -------
         * Same player gets another turn.
         *
         *
         * CENTER POINT
         * ------------
         * Same player gets another turn
         * based on your existing PathPoint logic.
         *
         */

        if (transfer && diceValue != 6)
        {
            GameManager.instance.transferDice = true;
        }
        else
        {
            GameManager.instance.transferDice = false;
        }


        // =====================================================
        // RESET DICE RESULT
        // =====================================================

        GameManager.instance.numberOfSteps = 0;


        // =====================================================
        // PREPARE NEXT TURN / NEXT ROLL
        // =====================================================

        GameManager.instance.rollDiceManager();


        GameManager.instance.canPlayerMove = false;


        playerMoveCoroutine = null;
    }


    // =========================================================
    // CHECK WHETHER A PIECE CAN MOVE
    // =========================================================

    public bool isPlayerAvailableToMove(
        int numberOfStepsAlready,
        int numberOfSteps,
        PathPoint[] pathPointsToMove)
    {
        if (pathPointsToMove == null ||
            pathPointsToMove.Length == 0)
        {
            return false;
        }


        if (numberOfSteps <= 0)
        {
            return false;
        }


        // Final position after moving
        int targetIndex =
            numberOfStepsAlready +
            numberOfSteps -
            1;


        // Make sure target is still inside this player's path
        return targetIndex <
               pathPointsToMove.Length;
    }
}