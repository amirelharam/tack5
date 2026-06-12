
namespace Tack5_ExaminationSystem

{
    enum Level { Easy, Medium, Hard }

    
    abstract class Question(string header, int marks, Level level)
    {
        public string Header { get; set; } = header;
        public int Marks { get; set; } = marks;
        public Level Level { get; set; } = level;

        public abstract void Display();
        public abstract bool CheckAnswer();
    }

    class TrueFalseQuestion(string header, int marks, Level level, bool correctAnswer)
        : Question(header, marks, level)
    {
        public bool CorrectAnswer { get; set; } = correctAnswer;

        public override void Display()
        {
            Console.WriteLine($"{Header}\n1- True\n2- False");
        }

        public override bool CheckAnswer()
        {
           
            if (int.TryParse(Console.ReadLine(), out int answer))
            {
                return (answer == 1 && CorrectAnswer) || (answer == 2 && !CorrectAnswer);
            }
            return false;
        }
    }

    class ChooseOneQuestion(string header, int marks, Level level, List<string> choices, int correctChoice)
        : Question(header, marks, level)
    {
        public List<string> Choices { get; set; } = choices;
        public int CorrectChoice { get; set; } = correctChoice;

        public override void Display()
        {
            Console.WriteLine(Header);
            for (int i = 0; i < Choices.Count; i++) Console.WriteLine($"{i + 1}- {Choices[i]}");
        }

        public override bool CheckAnswer() =>
            int.TryParse(Console.ReadLine(), out int answer) && answer == CorrectChoice;
    }

    class MultipleChoiceQuestion(string header, int marks, Level level, List<string> choices, string correctAnswers)
        : Question(header, marks, level)
    {
        public List<string> Choices { get; set; } = choices;
        public string CorrectAnswers { get; set; } = correctAnswers;

        public override void Display()
        {
            Console.WriteLine(Header);
            for (int i = 0; i < Choices.Count; i++) Console.WriteLine($"{i + 1}- {Choices[i]}");
            Console.WriteLine("Enter answers separated by comma (e.g., 1,3):");
        }

        public override bool CheckAnswer() => Console.ReadLine() == CorrectAnswers;
    }

    class Program
    {
        private static readonly List<Question> QuestionBank = [];

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n===== Main Menu =====\n1- Doctor Mode\n2- Student Mode\n3- Exit");
                if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 3) break;

                if (choice == 1) DoctorMode();
                else if (choice == 2) StudentMode();
            }
        }

        static void DoctorMode()
        {
            Console.Write("Enter Number Of Questions: ");
            if (!int.TryParse(Console.ReadLine(), out int count)) return;

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("\n1- True/False\n2- Choose One\n3- Multiple Choice");
                if (!int.TryParse(Console.ReadLine(), out int type)) continue;

                Console.Write("Header: "); string header = Console.ReadLine() ?? "";
                Console.Write("Marks: "); int.TryParse(Console.ReadLine(), out int marks);
                Console.Write("Level (Easy, Medium, Hard): ");
                if (!Enum.TryParse(Console.ReadLine(), true, out Level level)) level = Level.Easy;

                
                switch (type)
                {
                    case 1:
                        Console.Write("Correct Answer (true/false): ");
                        bool.TryParse(Console.ReadLine(), out bool ans);
                        QuestionBank.Add(new TrueFalseQuestion(header, marks, level, ans));
                        break;

                    case 2 or 3: 
                        List<string> choices = [];
                        for (int j = 0; j < 4; j++)
                        {
                            Console.Write($"Choice {j + 1}: ");
                            choices.Add(Console.ReadLine() ?? "");
                        }

                        if (type == 2)
                        {
                            Console.Write("Correct Choice Number: ");
                            int.TryParse(Console.ReadLine(), out int correct);
                            QuestionBank.Add(new ChooseOneQuestion(header, marks, level, choices, correct));
                        }
                        else
                        {
                            Console.Write("Correct Answers (1,3): ");
                            string answers = Console.ReadLine() ?? "";
                            QuestionBank.Add(new MultipleChoiceQuestion(header, marks, level, choices, answers));
                        }
                        break;
                }
            }
        }

        static void StudentMode()
        {
            Console.WriteLine("1- Practical\n2- Final");
            if (!int.TryParse(Console.ReadLine(), out int examType)) return;

            Console.Write("Choose Level (Easy, Medium, Hard): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Level level)) return;

            // استخدام LINQ لاختصار تصفية الأسئلة بدلاً من الـ Foreach التقليدية
            var selectedQuestions = QuestionBank.Where(q => q.Level == level).ToList();
            int count = examType == 1 ? selectedQuestions.Count / 2 : selectedQuestions.Count;

            if (count == 0) { Console.WriteLine("No questions available for this level."); return; }

            int totalMarks = 0, studentMarks = 0;

            for (int i = 0; i < count; i++)
            {
                selectedQuestions[i].Display();
                totalMarks += selectedQuestions[i].Marks;

                if (selectedQuestions[i].CheckAnswer())
                    studentMarks += selectedQuestions[i].Marks;
            }

            Console.WriteLine($"\nYour Result = {studentMarks}/{totalMarks}");
        }
    }
}