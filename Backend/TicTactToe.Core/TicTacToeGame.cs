namespace TicTactToe.Core;

public enum GameMode
{
    TwoPlayer,
    Computer
}

public enum Player
{
    X,
    O
}

public enum GameStatus
{
    InProgress,
    Won,
    Draw
}

public readonly record struct Cell(int Row, int Column);

public sealed record Move(int MoveNumber, Player Player, int Row, int Column);

public sealed class Scoreboard
{
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    internal void RecordWin(Player player)
    {
        if (player == Player.X)
        {
            XWins++;
        }
        else
        {
            OWins++;
        }
    }

    internal void RecordDraw() => Draws++;

    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
    }
}

public sealed class TicTacToeGame
{
    private static readonly Cell[] WinningLines =
    [
        new(1, 1), new(1, 2), new(1, 3),
        new(2, 1), new(2, 2), new(2, 3),
        new(3, 1), new(3, 2), new(3, 3),
        new(1, 1), new(2, 1), new(3, 1),
        new(1, 2), new(2, 2), new(3, 2),
        new(1, 3), new(2, 3), new(3, 3),
        new(1, 1), new(2, 2), new(3, 3),
        new(1, 3), new(2, 2), new(3, 1)
    ];

    private static readonly Cell[] Corners =
    [
        new(1, 1), new(1, 3), new(3, 1), new(3, 3)
    ];

    private readonly List<Move> moves = [];
    private readonly Scoreboard scoreboard;
    private Player?[,] board = new Player?[3, 3];

    public TicTacToeGame(GameMode mode, Scoreboard? scoreboard = null)
    {
        Mode = mode;
        this.scoreboard = scoreboard ?? new Scoreboard();
    }

    public Guid GameId { get; } = Guid.NewGuid();
    public GameMode Mode { get; }
    public Player CurrentPlayer { get; private set; } = Player.X;
    public GameStatus Status { get; private set; } = GameStatus.InProgress;
    public Player? Winner { get; private set; }
    public IReadOnlyList<Cell> WinningCells { get; private set; } = [];
    public IReadOnlyList<Move> Moves => moves;
    public Scoreboard Scoreboard => scoreboard;

    public Player? GetCell(int row, int column)
    {
        ValidatePosition(row, column);
        return board[row - 1, column - 1];
    }

    public bool TryMove(Player player, int row, int column, out string error)
    {
        error = string.Empty;

        if (Status != GameStatus.InProgress)
        {
            error = "The game is already complete.";
            return false;
        }

        if (player != CurrentPlayer)
        {
            error = "It is not this player's turn.";
            return false;
        }

        if (Mode == GameMode.Computer && player == Player.O)
        {
            error = "The computer controls player O.";
            return false;
        }

        if (row is < 1 or > 3 || column is < 1 or > 3)
        {
            error = "The row and column must be between 1 and 3.";
            return false;
        }

        if (board[row - 1, column - 1] is not null)
        {
            error = "The selected cell is already occupied.";
            return false;
        }

        ApplyMove(player, new Cell(row, column));

        if (Status == GameStatus.InProgress && Mode == GameMode.Computer)
        {
            ApplyMove(Player.O, ChooseComputerMove());
        }

        return true;
    }

    public bool Undo(out string error)
    {
        error = string.Empty;

        if (Status != GameStatus.InProgress)
        {
            error = "Undo is disabled after the game is complete.";
            return false;
        }

        if (moves.Count == 0)
        {
            error = "There are no moves to undo.";
            return false;
        }

        var movesToRemove = Mode == GameMode.Computer ? Math.Min(2, moves.Count) : 1;
        moves.RemoveRange(moves.Count - movesToRemove, movesToRemove);
        RebuildState();
        return true;
    }

    public void Reset()
    {
        moves.Clear();
        RebuildState();
    }

    private void ApplyMove(Player player, Cell cell)
    {
        board[cell.Row - 1, cell.Column - 1] = player;
        moves.Add(new Move(moves.Count + 1, player, cell.Row, cell.Column));
        WinningCells = FindWinningCells(player);

        if (WinningCells.Count > 0)
        {
            Status = GameStatus.Won;
            Winner = player;
            scoreboard.RecordWin(player);
            return;
        }

        if (moves.Count == 9)
        {
            Status = GameStatus.Draw;
            Winner = null;
            WinningCells = [];
            scoreboard.RecordDraw();
            return;
        }

        CurrentPlayer = player == Player.X ? Player.O : Player.X;
    }

    private void RebuildState()
    {
        board = new Player?[3, 3];
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];

        foreach (var move in moves)
        {
            board[move.Row - 1, move.Column - 1] = move.Player;
            CurrentPlayer = move.Player == Player.X ? Player.O : Player.X;
        }
    }

    private Cell ChooseComputerMove()
    {
        var winningMove = FindTacticalMove(Player.O);
        if (winningMove is not null)
        {
            return winningMove.Value;
        }

        var blockingMove = FindTacticalMove(Player.X);
        if (blockingMove is not null)
        {
            return blockingMove.Value;
        }

        if (IsAvailable(new Cell(2, 2)))
        {
            return new Cell(2, 2);
        }

        var corner = Corners.FirstOrDefault(IsAvailable);
        if (corner != default)
        {
            return corner;
        }

        return AvailableCells().First();
    }

    private Cell? FindTacticalMove(Player player)
    {
        foreach (var cell in AvailableCells())
        {
            board[cell.Row - 1, cell.Column - 1] = player;
            var wins = FindWinningCells(player).Count > 0;
            board[cell.Row - 1, cell.Column - 1] = null;
            if (wins)
            {
                return cell;
            }
        }

        return null;
    }

    private IReadOnlyList<Cell> FindWinningCells(Player player)
    {
        for (var line = 0; line < WinningLines.Length; line += 3)
        {
            var cells = WinningLines.Skip(line).Take(3).ToArray();
            if (cells.All(cell => board[cell.Row - 1, cell.Column - 1] == player))
            {
                return cells;
            }
        }

        return [];
    }

    private IEnumerable<Cell> AvailableCells()
    {
        for (var row = 1; row <= 3; row++)
        {
            for (var column = 1; column <= 3; column++)
            {
                var cell = new Cell(row, column);
                if (IsAvailable(cell))
                {
                    yield return cell;
                }
            }
        }
    }

    private bool IsAvailable(Cell cell) => board[cell.Row - 1, cell.Column - 1] is null;

    private static void ValidatePosition(int row, int column)
    {
        if (row is < 1 or > 3 || column is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "The row and column must be between 1 and 3.");
        }
    }
}
