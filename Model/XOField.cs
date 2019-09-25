using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XOWPF.Model
{

    class XOField
    {
        //Класс инкапсулирующий понятие игорового поля
        readonly int?[,] gameField; //Массив представляющий поле(? - здесь определяет nullable int, это значит что каждый элемет массива может иметь значение null)
        //Если в ячейке игрового поля лежит null значит ни один игрок туда не сходил
        public int GetNullCells
        {
            get
            {
                int counter = 0;
                for (int i = 0; i <= gameField.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= gameField.GetUpperBound(1); j++)
                    {
                        if (gameField[i, j] == null)
                            counter++;
                    }
                }
                return counter;
            }
        }
        public int FieldSize { get; private set; } //Свойство задающее размер игрового поля
        public delegate void ShowFieldHandler(XOField field); //Делегат(ссылка на функцию) для определения метода который будет выводить содержимое поля на экран
        public ShowFieldHandler show; //Экземпляр делегата
        public PlayerType? this[Point p] //Индексатор, доступ к элементу поля через обьект Point
        {
            get => (PlayerType?)gameField[(int)p.X, (int)p.Y];

            set => gameField[(int)p.X, (int)p.Y] = (int?)value;
        }
        public (string, List<PlayerType?>) this[string name]
        {
            get
            {

                if (LinesName.Contains(name))
                {
                    var line = (name, new List<PlayerType?>());
                    foreach (var e in Lines[name])
                    {
                        line.Item2.Add(this[e]);
                    }
                    return line;
                }
                else throw new IndexOutOfRangeException("Такой линии не существует.");
                
            }
        }
        //Имена диагоналей
        public const string d1 = "d1";
        public const string d2 = "d2";
        //Диагонали всегда две
        string[] linesName; //Здесь будут имена горизонталей и вертикалей игрового поля
        public string[] LinesName { get => linesName; private set { linesName = value; } }
        readonly private Dictionary<string, Point[]> lines;
        public Dictionary<string, Point[]> Lines { get => lines;} //А здесь координаты всех точек на вертикалях и горизонталях
        public XOField(int size = 3) //Конструктор(срабатывает при создании экземпляра класса)
        {
            FieldSize = size;
            gameField = new int?[FieldSize, FieldSize];
            lines = new Dictionary<string, Point[]>();
            LinesName = new string[FieldSize*2 + 2];
            SetLines();
        }
        private void SetLines()
        {
            
            for (int i = 0; i < FieldSize; i++)
            {
                LinesName[i] = "v" + i;
                LinesName[FieldSize + i] = "g" + i;
            }
            Lines[d1] = new Point[FieldSize];
            Lines[d2] = new Point[FieldSize];
            for (int i = 0; i < (2 * FieldSize); i++)
            {
                Lines[LinesName[i]] = new Point[FieldSize];
            }

            for (int i = 0; i < FieldSize; i++)
            {
                Lines[d1][i].X = i;
                Lines[d1][i].Y = i;
                Lines[d2][i].X = FieldSize-i-1;
                Lines[d2][i].Y = i;
            }
            for (int col = 0; col < FieldSize; col++)
            {
                for (int row = 0; row < FieldSize; row++)
                {
                    Lines[LinesName[col]][row].X = col;
                    Lines[LinesName[col]][row].Y = row;
                    Lines[LinesName[col + FieldSize]][row].X = row;
                    Lines[LinesName[col + FieldSize]][row].Y = col ;
                }
            }
            LinesName[LinesName.GetUpperBound(0)] = d2;
            LinesName[LinesName.GetUpperBound(0)-1] = d1;

        } //Метод определяющий координаты точек на каждой "линии"
        public void RegisterHandler(ShowFieldHandler show)
        {
            this.show = show;
        } //Регистрация функции которая будет выводить содержимое игрового поля на экран
        public void Show()
        {
            show?.Invoke(this);
        } //Вызов функции из делегата show
        public void ClearField()
        {
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    gameField[i, j] = null;
                }
            }
        } //Очистка игрового поля
        public string FindWinner(out PlayerType? s)
        {

            var Lines = this.Lines;
            foreach (var e in Lines)
            {
                bool IsXWIn = true;
                bool IsOWin = true;
                for (int i = 0; i < FieldSize; i++)
                {
                    IsXWIn &= this[e.Value[i]] == PlayerType.x;
                    IsOWin &= this[e.Value[i]] == PlayerType.o;
                }
                if (IsXWIn)
                {
                    s = PlayerType.x;
                    return e.Key;
                }
                if (IsOWin)
                {
                    s = PlayerType.o;
                    return e.Key;
                }
            }
            s = null;
            return null;
        }//Определение победителя
    }
}
