using UnityEngine;

public class YellowController : PlayerController
{
    private RollingDice yellowRollingDice;

    void Start()
    {
        YellowPieces yellowPieces =
            GetComponentInParent<YellowPieces>();

        if (yellowPieces == null)
        {
            Debug.LogError(
                "YellowPieces component not found!",
                this
            );

            return;
        }

        yellowRollingDice =
            yellowPieces.rollingDice;


        if (yellowRollingDice == null)
        {
            Debug.LogError(
                "Yellow rolling dice is not assigned!",
                this
            );
        }
    }


    private void OnMouseDown()
    {
        if (GameManager.instance == null)
            return;


        if (GameManager.instance.rollingDice == null)
            return;


        // Make sure it is actually Yellow's turn
        if (GameManager.instance.rollingDice != yellowRollingDice)
            return;


        // =====================================================
        // PIECE IS STILL IN BASE
        // =====================================================

        if (!isReady)
        {
            // A piece can leave base only with 6
            if (GameManager.instance.numberOfSteps == 6)
            {
                makePlayerReadyToMove(
                    pathParent.YellowPathPoints
                );

                return;
            }

            return;
        }


        // =====================================================
        // PIECE IS ALREADY ON THE BOARD
        // =====================================================

        movePlayer(
            pathParent.YellowPathPoints
        );
    }
}