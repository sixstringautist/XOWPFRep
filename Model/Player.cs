
using System.Windows;
namespace XOWPF.Model
{
    class Player
    {
        //TODO: сюда прикрутить ИИ
        public PlayerType Type { get; set; } //Свойство определяюще тип игрока(крестик или нолик)
        public int Points { get; set; }// Свойство для хранения количества побед игрока
        protected readonly XOField field;
        public bool IsAI { get; set; }
        public Point PointToMove { get; set; }

        public static implicit operator string(Player p)
        {
            if (p.Type == PlayerType.x)
                return "X";
            else return "Y";
        }//Перегрузка оператора приведения типов, преобразует поле Type к строковому представлению(Игрок либо X либо O)

        public Player(XOField field, PlayerType type = PlayerType.x) 
        {
            Type = type;
            this.field = field;
            IsAI = false;

        }//Конструктор(Срабатывает при создании экземпляра класса)
        //Задает тип игрока
        public virtual void DoMove() => field[PointToMove] = Type; //Ход игрока по координатам
    }
}
