using System.Windows;
namespace XOWPF.Model
{
    enum PlayerType
    {
        x,
        o
    }
    class XOModel
    {
        //Класс управляющий ходом игры
        private Player currentPlayer;//Игрок который в данный момент ходит
        public Player CurrentPlayer { get => currentPlayer; }
        private Player nextPlayer;//Игрок который ходит в следующем ходу
        private Player firstPlayer;//Первый игрок
        private Player secondPlayer;//Второй игрок
        private Player _winner;//Победитель в игре
        private AIPlayer AI { get; set; }
        public string SetAiDiff { set { AI.Diff = value; } }
        private bool _AIon;
        public bool OnAI
        {
            get => _AIon;
            set
            {
                _AIon = value;
            }

        }
        private bool _gameState = false;//Состояние игры(Завершена или нет)
        public int XPlayerPoints
        {
            get
            {
                if (firstPlayer.Type == PlayerType.x)
                    return FirstPlayerPoints;
                else {
                    return SecondPlayerPoints;
                }
            }
        }
        public int OPlayerPoints {
            get {
                if (firstPlayer.Type == PlayerType.o)
                    return FirstPlayerPoints;
                else return SecondPlayerPoints;
            }
        }
        private int FirstPlayerPoints
        {
            get => firstPlayer.Points;
        }
        private int SecondPlayerPoints
        {
            get => secondPlayer.Points;
        }
        public bool GameState { get => _gameState; set { _gameState = value; } } //Свойство для установки _gameState
        public XOField Field { get; }//Экземпляр игрового поля
        public XOModel(PlayerType FirstPlayerType = PlayerType.x, PlayerType SecondPlayerType = PlayerType.o)
        {
            Field = new XOField();
            AI = new AIPlayer(Field, "Normal");
            if (FirstPlayerType != SecondPlayerType)
            {
                firstPlayer = new Player(Field, FirstPlayerType);
                secondPlayer = new Player(Field, SecondPlayerType);
                WhoFirst();
            }

        }
        //Конструктор(Срабатывает при создании экземпляра класса)
        private void WhoFirst()
        {
            if (OnAI)
            {
                switch (firstPlayer.Type)
                {
                    case PlayerType.x:
                        currentPlayer = firstPlayer;
                        nextPlayer = AI;
                        break;
                    case PlayerType.o:
                        currentPlayer = AI;
                        nextPlayer = firstPlayer;
                        break;
                }
            }
            else
            {
                switch (firstPlayer.Type)
                {
                    case PlayerType.x:
                        currentPlayer = firstPlayer;
                        nextPlayer = secondPlayer;
                        break;
                    case PlayerType.o:
                        currentPlayer = secondPlayer;
                        nextPlayer = firstPlayer;
                        break;
                }
            }

        } //Определяем чей ход первый(Первый всегда крестик)
        public void ChangePlayerTypes(PlayerType f)
        {
            if (OnAI)
            {
                if (f == PlayerType.x)
                {
                    firstPlayer.Type = f;
                    AI.Type = PlayerType.o;
                }
                else
                {
                    firstPlayer.Type = f;
                    AI.Type = PlayerType.x;
                }
                WhoFirst();
                return;
            }

            if (f == PlayerType.x)
            {
                firstPlayer.Type = f;
                secondPlayer.Type = PlayerType.o;
            }
            else
            {
                firstPlayer.Type = f;
                secondPlayer.Type = PlayerType.x;
            }
            WhoFirst();

        }//Сменить тип игрока(X на O или наоборот)
        public void ResetGame()
        {
            GameState = true;
            Field.ClearField();
            WhoFirst();
        }//Рестарт игры
        private void SwapPlayers()
        {
            Player tmp = currentPlayer;
            currentPlayer = nextPlayer;
            nextPlayer = tmp;
        }
        public void CurrentMove()
        {
            if (GameState)
            {
                currentPlayer.DoMove();
                PlayerType? P;
                var line = Field.FindWinner(out P);
                if (line != null)
                {
                    _winner = currentPlayer;
                    _winner.Points++;
                    GameState = false;
                }
                if (Field.GetNullCells == 0)
                {
                    GameState = false;
                }
                SwapPlayers();
            }
        }
        public void CurrentMove(Point p)
        {
            
            if (Field[p] == null && GameState)
            {
                CurrentPlayer.PointToMove = p;
                currentPlayer.DoMove();
                PlayerType? P;
                var line = Field.FindWinner(out P);
                if (line != null)
                {
                    _winner = currentPlayer;
                    _winner.Points++;
                    GameState = false;
                }
                if(Field.GetNullCells == 0)
                {
                    GameState = false;
                }
                SwapPlayers();
            }
        }//Текущий ход

    }
}
