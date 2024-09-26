using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }

   public void MakeOptimalMove()
{
    // Prioritize winning
    for (int i = 0; i < Rows; i++)
    {
        for (int j = 0; j < Columns; j++)
        {
            if (cells[j, i].current == PlayerOption.NONE)
            {
                cells[j, i].current = currentPlayer; // Temporarily make the move
                if (GetWinner() == currentPlayer)
                {
                    board.UpdateCellVisual(j, i, currentPlayer); // Update the board
                    return; // Exit the function as we have made the optimal move
                }
                cells[j, i].current = PlayerOption.NONE; // Undo the move
            }
        }
    }

    // Prioritize blocking the opponent
    PlayerOption opponent = (currentPlayer == PlayerOption.X) ? PlayerOption.O : PlayerOption.X;
    for (int i = 0; i < Rows; i++)
    {
        for (int j = 0; j < Columns; j++)
        {
            if (cells[j, i].current == PlayerOption.NONE)
            {
                cells[j, i].current = opponent; // Temporarily make the opponent's move
                if (GetWinner() == opponent)
                {
                    cells[j, i].current = currentPlayer; // Block the opponent
                    board.UpdateCellVisual(j, i, currentPlayer);
                    return; // Exit the function
                }
                cells[j, i].current = PlayerOption.NONE; // Undo the move
            }
        }
    }

    // Fallback to optimal opening move: Player 1 corner, Player 2 center
    if (currentPlayer == PlayerOption.X && cells[0, 0].current == PlayerOption.NONE)
    {
        cells[0, 0].current = currentPlayer;
        board.UpdateCellVisual(0, 0, currentPlayer);
    }
    else if (currentPlayer == PlayerOption.O && cells[1, 1].current == PlayerOption.NONE)
    {
        cells[1, 1].current = currentPlayer;
        board.UpdateCellVisual(1, 1, currentPlayer);
    }

    EndTurn(); // Change player turn
}


    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if(GetWinner() == PlayerOption.NONE)
            EndTurn();
        else
        {
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for(int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
