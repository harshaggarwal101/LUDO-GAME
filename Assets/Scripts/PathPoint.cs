using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single tile or step on the game board.
/// Handles players landing on the tile, safe zones, capturing mechanics,
/// and repositioning multiple pieces that land on the same spot.
/// </summary>
public class PathPoint : MonoBehaviour
{
    public static PathPoint pathPoint;

    public PathObjectParent pathObjectParent;

    public List<PlayerController> players =
        new List<PlayerController>();

    private PathPoint[] pathPointsToMove;


    void Awake()
    {
        pathPoint = this;
    }


    void Start()
    {
        pathObjectParent =
            GetComponentInParent<PathObjectParent>();

        if (pathObjectParent == null)
        {
            pathObjectParent =
                FindAnyObjectByType<PathObjectParent>();
        }

        // Hide the visual path-point sprite
        SpriteRenderer sr =
            GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.enabled = false;
        }
    }

    // =========================================================
    // ADD PLAYER TO THIS PATH POINT
    // =========================================================

    public bool add(
        PlayerController player)
    {
        if (player == null)
            return true;


        // =====================================================
        // CENTER POINT
        // =====================================================

        if (name == "CenterPoint")
        {
            addPlayer(player);

            GameManager.instance.PlayHomeReachedSound();

            completed(player);

            return false;
        }


        // =====================================================
        // SAFE PATH
        //
        // No capture is allowed on safe path points.
        // =====================================================

        bool isSafe =
            pathObjectParent.safePathPoints.Contains(this);


        if (!isSafe)
        {
            // =================================================
            // THERE IS ONE PLAYER ALREADY ON THIS POINT
            // =================================================

            if (players.Count == 1)
            {
                PlayerController previousPlayer =
                    players[0];


                string previousColor =
                    GetPlayerColor(previousPlayer.name);


                string currentColor =
                    GetPlayerColor(player.name);


                // =================================================
                // DIFFERENT COLORS = CAPTURE
                // =================================================

                if (previousColor != currentColor)
                {
                    GameManager.instance.PlayPlayerCutSound();
                    previousPlayer.isReady = false;

                    previousPlayer.numberOfSteps = 0;


                    StartCoroutine(
                        revertToStart(previousPlayer)
                    );


                    removePlayer(previousPlayer);


                    addPlayer(player);


                    // false means:
                    // DON'T transfer dice.
                    // Capturing player gets another turn.
                    return false;
                }
            }
        }


        // =====================================================
        // NORMAL ADD
        // =====================================================

        addPlayer(player);

        return true;
    }


    // =========================================================
    // PLAYER COMPLETED PATH
    // =========================================================

    void completed(
        PlayerController player)
    {
        if (player.name.Contains("blue_piece"))
        {
            GameManager.instance.blueCompletePlayer += 1;
            GameManager.instance.blueOutPlayer -= 1;
        }
        else if (player.name.Contains("red_piece"))
        {
            GameManager.instance.redCompletePlayer += 1;
            GameManager.instance.redOutPlayer -= 1;
        }
        else if (player.name.Contains("green_piece"))
        {
            GameManager.instance.greenCompletePlayer += 1;
            GameManager.instance.greenOutPlayer -= 1;
        }
        else if (player.name.Contains("yellow_piece"))
        {
            GameManager.instance.yellowCompletePlayer += 1;
            GameManager.instance.yellowOutPlayer -= 1;
        }


        if (GameManager.instance.blueCompletePlayer == 4 &&
            GameManager.instance.redCompletePlayer == 4 &&
            GameManager.instance.greenCompletePlayer == 4 &&
            GameManager.instance.yellowCompletePlayer == 4)
        {
            // Game complete logic can be added here.
        }
    }


    // =========================================================
    // RETURN CAPTURED PLAYER TO BASE
    // =========================================================

    IEnumerator revertToStart(
        PlayerController player)
    {
        if (player == null)
            yield break;


        if (pathObjectParent == null)
        {
            Debug.LogError(
                "PathObjectParent is missing!"
            );

            yield break;
        }


        // =====================================================
        // SELECT CORRECT COLOR PATH
        // =====================================================

        if (player.name.Contains("blue_piece"))
        {
            if (GameManager.instance.blueOutPlayer > 0)
                GameManager.instance.blueOutPlayer--;

            pathPointsToMove =
                pathObjectParent.BluePathPoints;
        }
        else if (player.name.Contains("red_piece"))
        {
            if (GameManager.instance.redOutPlayer > 0)
                GameManager.instance.redOutPlayer--;

            pathPointsToMove =
                pathObjectParent.RedPathPoints;
        }
        else if (player.name.Contains("green_piece"))
        {
            if (GameManager.instance.greenOutPlayer > 0)
                GameManager.instance.greenOutPlayer--;

            pathPointsToMove =
                pathObjectParent.GreenPathPoints;
        }
        else if (player.name.Contains("yellow_piece"))
        {
            if (GameManager.instance.yellowOutPlayer > 0)
                GameManager.instance.yellowOutPlayer--;

            pathPointsToMove =
                pathObjectParent.YellowPathPoints;
        }
        else
        {
            Debug.LogError(
                "Could not determine player color: " +
                player.name
            );

            yield break;
        }


        // =====================================================
        // CHECK PATH
        // =====================================================

        if (pathPointsToMove == null ||
            pathPointsToMove.Length == 0)
        {
            Debug.LogError(
                "PathPoints array is empty for: " +
                player.name
            );

            yield break;
        }


        // =====================================================
        // PROTECT AGAINST INDEX OUT OF RANGE
        // =====================================================

        int startIndex =
            Mathf.Min(
                player.numberOfStepsAlready - 1,
                pathPointsToMove.Length - 1
            );


        Debug.Log(
            "Reverting: " +
            player.name +
            " | Steps: " +
            player.numberOfStepsAlready +
            " | Path Length: " +
            pathPointsToMove.Length +
            " | Start Index: " +
            startIndex
        );


        // =====================================================
        // MOVE BACKWARDS
        // =====================================================

        for (
            int i = startIndex;
            i >= 0;
            i--)
        {
            player.transform.position =
                pathPointsToMove[i]
                    .transform.position;


            yield return new WaitForSeconds(0.2f);
        }


        // =====================================================
        // FIND ORIGINAL BASE POSITION
        // =====================================================

        Transform basePoint =
            GetBasePoint(player.name);


        if (basePoint == null)
        {
            Debug.LogError(
                "Could not find BasePoint for player: " +
                player.name
            );

            yield break;
        }


        player.transform.position =
            basePoint.position;


        // Reset captured player
        player.numberOfSteps = 0;
        player.numberOfStepsAlready = 0;
        player.isReady = false;
    }


    // =========================================================
    // FIND BASE POSITION
    // =========================================================

    Transform GetBasePoint(
        string playerName)
    {
        if (pathObjectParent == null)
            return null;


        if (pathObjectParent.BasePoints == null ||
            pathObjectParent.BasePoints.Length == 0)
        {
            Debug.LogError(
                "BasePoints array is empty!"
            );

            return null;
        }


        // =====================================================
        // DETERMINE PIECE INDEX
        // =====================================================

        int pieceIndex = 0;


        if (playerName.EndsWith("(1)"))
        {
            pieceIndex = 1;
        }
        else if (playerName.EndsWith("(2)"))
        {
            pieceIndex = 2;
        }
        else if (playerName.EndsWith("(3)"))
        {
            pieceIndex = 3;
        }


        // =====================================================
        // DETERMINE COLOR
        // =====================================================

        string color =
            GetPlayerColor(playerName);


        if (color == "")
        {
            Debug.LogError(
                "Could not determine color for player: " +
                playerName
            );

            return null;
        }


        // =====================================================
        // FIND COLOR PARENT
        // =====================================================

        for (
            int i = 0;
            i < pathObjectParent.BasePoints.Length;
            i++)
        {
            Transform baseParent =
                pathObjectParent.BasePoints[i];


            if (baseParent == null)
                continue;


            if (baseParent.name == color)
            {
                // Check child exists
                if (pieceIndex >= baseParent.childCount)
                {
                    Debug.LogError(
                        "BasePoint child does not exist! " +
                        "Color: " +
                        color +
                        " | Piece Index: " +
                        pieceIndex
                    );

                    return null;
                }


                return baseParent.GetChild(
                    pieceIndex
                );
            }
        }


        Debug.LogError(
            "Could not find BasePoint parent: " +
            color
        );

        return null;
    }


    // =========================================================
    // GET PLAYER COLOR
    // =========================================================

    string GetPlayerColor(
        string playerName)
    {
        if (playerName.Contains("blue_piece"))
            return "Blue";


        if (playerName.Contains("red_piece"))
            return "Red";


        if (playerName.Contains("green_piece"))
            return "Green";


        if (playerName.Contains("yellow_piece"))
            return "Yellow";


        return "";
    }


    // =========================================================
    // ADD PLAYER
    // =========================================================

    public void addPlayer(
        PlayerController player)
    {
        if (player == null)
            return;


        if (!players.Contains(player))
        {
            players.Add(player);
        }


        reSize();
    }


    // =========================================================
    // REMOVE PLAYER
    // =========================================================

    public void removePlayer(
        PlayerController player)
    {
        if (player == null)
            return;


        players.Remove(player);

        reSize();
    }


    // =========================================================
    // RESIZE / REPOSITION PLAYERS
    // =========================================================

    public void reSize()
    {
        int playersCount =
            players.Count;


        if (playersCount == 0)
            return;


        if (pathObjectParent == null)
            return;


        // Prevent index out of range
        if (playersCount >
            pathObjectParent.scales.Length)
        {
            Debug.LogError(
                "Not enough scale values for " +
                playersCount +
                " players."
            );

            return;
        }


        if (playersCount >
            pathObjectParent.posDiff.Length)
        {
            Debug.LogError(
                "Not enough position offset values for " +
                playersCount +
                " players."
            );

            return;
        }


        float scale =
            pathObjectParent.scales[
                playersCount - 1
            ];


        float offset =
            pathObjectParent.posDiff[
                playersCount - 1
            ];


        // =====================================================
        // RESIZE PLAYERS
        // =====================================================

        for (
            int i = 0;
            i < playersCount;
            i++)
        {
            players[i].transform.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f
                );


            SpriteRenderer sr =
                players[i]
                    .GetComponentInChildren<SpriteRenderer>();


            if (sr != null)
            {
                sr.sortingOrder = 1;
            }
        }


        Vector3 center =
            transform.position;


        // =====================================================
        // POSITION PLAYERS
        // =====================================================

        switch (playersCount)
        {
            case 1:

                players[0].transform.position =
                    center;

                break;


            case 2:

                players[0].transform.position =
                    center +
                    new Vector3(
                        -offset,
                        0f,
                        0f
                    );


                players[1].transform.position =
                    center +
                    new Vector3(
                        offset,
                        0f,
                        0f
                    );

                break;


            case 3:

                players[0].transform.position =
                    center +
                    new Vector3(
                        0f,
                        offset,
                        0f
                    );


                players[1].transform.position =
                    center +
                    new Vector3(
                        -offset,
                        -offset,
                        0f
                    );


                players[2].transform.position =
                    center +
                    new Vector3(
                        offset,
                        -offset,
                        0f
                    );

                break;


            case 4:

                players[0].transform.position =
                    center +
                    new Vector3(
                        -offset,
                        offset,
                        0f
                    );


                players[1].transform.position =
                    center +
                    new Vector3(
                        offset,
                        offset,
                        0f
                    );


                players[2].transform.position =
                    center +
                    new Vector3(
                        offset,
                        -offset,
                        0f
                    );


                players[3].transform.position =
                    center +
                    new Vector3(
                        -offset,
                        -offset,
                        0f
                    );

                break;


            default:

                float radius = offset;


                for (
                    int i = 0;
                    i < playersCount;
                    i++)
                {
                    float angle =
                        (360f / playersCount) * i;


                    float x =
                        Mathf.Cos(
                            angle * Mathf.Deg2Rad
                        ) * radius;


                    float y =
                        Mathf.Sin(
                            angle * Mathf.Deg2Rad
                        ) * radius;


                    players[i].transform.position =
                        center +
                        new Vector3(
                            x,
                            y,
                            0f
                        );
                }

                break;
        }
    }
}