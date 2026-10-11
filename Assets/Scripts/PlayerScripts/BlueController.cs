using UnityEngine;

public class BlueController : PlayerController
{
    private void OnMouseDown()
    {
        if (GameManager.instance == null)
            return;

        // It must be Blue's turn
        if (GameManager.instance.rollingDice !=
            GameManager.instance.manageRollingDice[0])
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
                    pathParent.BluePathPoints
                );
            }

            return;
        }

        // Piece is already on the board
        movePlayer(
            pathParent.BluePathPoints
        );
    }
}