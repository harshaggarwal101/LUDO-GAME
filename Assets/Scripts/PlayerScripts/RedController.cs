using UnityEngine;

public class RedController : PlayerController
{
    private void OnMouseDown()
    {
        if (GameManager.instance == null)
            return;

        // It must be Red's turn
        if (GameManager.instance.rollingDice !=
            GameManager.instance.manageRollingDice[1])
        {
            return;
        }

        // A piece in base
        if (!isReady)
        {
            // Can only come out with 6
            if (GameManager.instance.numberOfSteps == 6)
            {
                makePlayerReadyToMove(
                    pathParent.RedPathPoints
                );
            }

            return;
        }

        // Piece is already on the board
        movePlayer(
            pathParent.RedPathPoints
        );
    }
}