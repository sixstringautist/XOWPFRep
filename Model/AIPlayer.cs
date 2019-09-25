using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
namespace XOWPF.Model
{
    class AIPlayer : Player
    {

        /// <summary>
        /// Структура описывающая ход
        /// </summary>
        private struct Move
        {
            public Move(Point p, int score=0)
            {
                this.p = p;
                this.score = score;
            }
            public Point p;
            public int score;

            public static explicit operator int(Move m)
            {
                return m.score;
            }
        }
        /// <summary>
        /// Структура описывающая уровень сложности
        /// </summary>
        public struct Difficulty
        {   public enum Diff
            {
                easy = 3,
                normal = 6,
                hard = 9
            }
            private Diff ai;


            private Difficulty(Diff d)
            {
                ai = d;
            }
            public Difficulty(string _diff)
            {
                _diff = _diff.ToLower();
                switch (_diff)
                {
                    case "easy":
                        ai = Diff.easy;
                        break;
                    case "normal":
                        ai = Diff.normal;
                        break;
                    case "Hard":
                        ai = Diff.hard;
                        break;
                    default:
                        ai = Diff.normal;
                        break;
                }
            }
            public static implicit operator string(Difficulty dif)
            {
                return dif.ai.ToString().ToUpper();
            }
            public static implicit operator Difficulty(string s)
            {
                s = s.ToLower();
                switch (s)
                {
                    case "easy":
                        return new Difficulty(Diff.easy);
                    case "normal":
                        return new Difficulty(Diff.normal);
                    case "hard":
                        return new Difficulty(Diff.hard);
                    default:
                        throw new Exception($"{s} is not Difficulty");
                }
            }
            public static explicit operator int(Difficulty _diff)
            {
                return (int)_diff.ai;
            }

        }

        /// <summary>
        /// Координаты хода для ИИ
        /// </summary>
        
        private Difficulty _diff;
        public Difficulty Diff { get => _diff; set
            {
                _diff = value;
            }
        }
        private int depth;
        private int INF;
        /// <summary>
        /// Генерация всех возможных ходов для поля
        /// </summary>
        /// <param name="field">Поле для которого генерируются ходы</param>
        /// <returns>Список возможных ходов</returns>
        private List<Move> GenerateMoves(XOField field)
        {
            List<Move> Moves = new List<Move>();
            for (int y = 0; y < field.FieldSize; y++)
            {
                for (int x = 0; x < field.FieldSize; x++)
                {
                    Point p = new Point(x, y);
                    if(field[p] == null)
                    Moves.Add(new Move(p));
                }
            }
            return Moves;
        }
      

        private int Evaluate(PlayerType? type, PlayerType? NotType, XOField field)
        {
            int PTypeLines = 0;
            int NotPTypeLines = 0;

            var Lines = field.LinesName;
            foreach (var name in Lines)
            {
                int counterType = 0;
                int counterNotType = 0;
                var Line = field[name];
                foreach (var el in Line.Item2)
                {
                    if (el == NotType)
                        counterNotType++;
                    else if (el == Type)
                        counterType++;
                }
                if (counterType > 0 && counterNotType==0)
                    PTypeLines++;
                if (counterNotType > 0 && counterType == 0)
                    NotPTypeLines++;
            }

            return PTypeLines - NotPTypeLines;
        }
        public AIPlayer(XOField field, Difficulty diff, PlayerType p = PlayerType.o) : base(field, p)
        {
            INF = 1000;
            this.Diff = diff;
            base.IsAI = true;
        }
        private void SetMove()
        {
            Move move=new Move();
            BestMove(this.field, this.Type, (int)Diff, ref move);
            this.PointToMove = move.p;
        }
        private int BestMove(XOField field, PlayerType? Type, int depth, ref Move M)
        {       
            PlayerType? NotType;
            if (Type == PlayerType.x)
                NotType = PlayerType.o;
            else NotType = PlayerType.x;
            PlayerType? win;
            field.FindWinner(out win);
            if ( win == Type)
            {
                return INF;
            }
            if (win == NotType)
            {
                return -INF;
            }
            if (depth <= 0)
            {
                return Evaluate(Type, NotType, field);
            }


            var Moves = GenerateMoves(field);
            var Arr = Moves.ToArray();
            if (Arr.Length == 0) return 0;
            int tmp;
            for (int i = 0; i < Arr.Length; i++)
            {
                
                field[Arr[i].p] = Type;
                tmp = -BestMove(field, NotType, depth - 1, ref M);
                Arr[i].score = tmp;
                field[Arr[i].p] = null;
            }

            var Max = Arr.Max(r => r.score);
            for (int i = 0; i < Arr.Length; i++)
            {
                if (Arr[i].score == Max)
                { M = Arr[i]; break; }
            }

            return  Arr.Max(r => r.score);
        }
        public void WriteField(XOField field, int tmp, int depth, PlayerType? type)
        {
            StreamWriter writer = new StreamWriter("Output.txt",true);
            string ToPrint="";
            ToPrint =$"Текущий слой:{depth}";
            writer.WriteLine(ToPrint);
            writer.WriteLine("Оцекнка состояния:" + tmp);
            writer.WriteLine($"Ходит игрок:{type.ToString()}");
            ToPrint = "";
            for (int x = 0; x < field.FieldSize; x++)
            {
                for (int y = 0; y < field.FieldSize; y++)
                {
                    ToPrint +=$"{field[new Point(y, x)]} ";
                }

                writer.WriteLine(ToPrint);
                ToPrint = "";
            }
            writer.Close();
        }
        public override void DoMove()
        {
            SetMove();
            field[base.PointToMove] = base.Type;
        }
    }
}
