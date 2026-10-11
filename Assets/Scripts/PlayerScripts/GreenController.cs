using UnityEngine;

public class GreenController : PlayerController
{
    private void OnMouseDown()
    {
        if (GameManager.instance == null)
            return;

        // It must be Green's turn
        if (GameManager.instance.rollingDice !=
            GameManager.instance.manageRollingDice[2])
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
                    pathParent.GreenPathPoints
                );
            }

            return;
        }

        // Piece is already on the board
        movePlayer(
            pathParent.GreenPathPoints
        );
    }
}