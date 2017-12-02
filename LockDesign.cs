using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public class LockDesign
    {
        static string TypedPasswords = "";

        public static Controller.ControllerView PreviousViewState { get; internal set; }
        public static List<NeoButton> buttons;
        public static Calculator calculator;
        static int x=272, y=223;
        static int selected = -1, hovered = -1;
        public static void Initialize()
        {
            GlobalClass.textboxAction = GlobalClass.TextBoxAction.LockPassword;
            GlobalClass.ShowTextBox(new PointF(272, 135), new SizeF(300, 25), "");
            calculator = new Calculator();
            if(buttons==null)
            {
                InitializeButtons();
            }
        }

        private static void InitializeButtons()
        {
            buttons = new List<NeoButton>();
            buttons.Add(new NeoButton("1", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("2", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("3", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("4", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("5", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("6", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("7", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("8", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("9", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("0", new Size(30, 30), new PointF(x, y))); x = 272; y += 30;

            buttons.Add(new NeoButton("+", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("-", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("/", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("*", new Size(30, 30), new PointF(x, y))); x += 30;
            buttons.Add(new NeoButton("unlock", new Size(60, 30), new PointF(x, y))); x += 60;
            buttons.Add(new NeoButton("clear", new Size(60, 30), new PointF(x, y))); x += 60;
            buttons.Add(new NeoButton("exit", new Size(60, 30), new PointF(x, y)));
        }

        internal static void MouseMove(Point location)
        {
            hovered = FindSelection(location);
        }

        private static int FindSelection(Point location)
        {
            RectangleF mouserect = new RectangleF(location, new Size(1, 1));
            for(int i=0;i<buttons.Count;i++)
            {
                if(buttons[i].rectangle.IntersectsWith(mouserect))
                {
                    return i;
                }
            }
            return -1;
        }

        internal static void KeyUp(Keys keyCode)
        {
        }

        internal static void MouseDown(Point location, MouseButtons button)
        {
            selected = FindSelection(location);
            if (selected != -1)
            {
                string text = buttons[selected].Value;
                switch (text)
                {
                    case "*":
                        calculator.FeedOperator(Calculator.Operation.Multiply);
                        break;

                    case "/":
                        calculator.FeedOperator(Calculator.Operation.Division);
                        break;

                    case "+":
                        calculator.FeedOperator(Calculator.Operation.Add);
                        break;

                    case "-":
                        calculator.FeedOperator(Calculator.Operation.Substraction);
                        break;
                    case "clear":
                        calculator = new Calculator();
                        break;
                    case "unlock":
                        calculator.Operate();
                        if (calculator.result == Properties.Settings.Default.PasswordMath)
                        {
                            Controller.SetView(PreviousViewState);
                            GlobalClass.Hidetextbox();
                        }
                        else
                        {
                            calculator = new Calculator();
                        }
                        break;
                    case "exit":
                        Controller.ExitProgram();
                        break;
                    default:
                        calculator.ConcatNewDigit(text);
                        break;
                }

            }
        }

        internal static void MouseUp(Point location)
        {
            selected = -1;
        }

        internal static void MouseDoubleClick(Point location)
        {

        }

        internal static void MouseWheel(int delta, Point p)
        {

        }

        internal static void KeyDown(Keys keyCode)
        {
           
        }

        private static void CheckPassword()
        {
            if (TypedPasswords == Properties.Settings.Default.Password)
            {

                Controller.SetView(PreviousViewState);
                GlobalClass.Hidetextbox();
            }
        }

        internal static void TextChanged(object sender, EventArgs e)
        {

            if (GlobalClass.textboxAction == GlobalClass.TextBoxAction.LockPassword)
            {
                if (sender == null)
                    Controller.ExitProgram();
                else
                {
                    TypedPasswords = sender.ToString();
                    CheckPassword();
                }
            }
        }

        internal static void Paint(Graphics graphics)
        {
           
           if(hovered!=-1)
                graphics.FillRectangle(Brushes.SkyBlue, buttons[hovered].rectangle);
            if (selected != -1)
                graphics.FillRectangle(Brushes.Gold, buttons[selected].rectangle);

            graphics.DrawImage(Properties.Resources.lockscreen, new Rectangle(0, 0, 748, 500));
            calculator.Paint(graphics, new Point(272, 290));
        }
    }

    public class NeoButton
    {
        public string Value;
        public SizeF size;
        public PointF Location;
        public RectangleF rectangle { get { return new RectangleF(Location, size); } }

        public NeoButton(string value, SizeF s, PointF loc)
        {
            this.Value = value;
            this.size = s;
            this.Location = loc;
        }
    }

    public class Calculator
    {
        public string result = "";
        public enum Operation
        {
            Add,
            Substraction,
            Multiply,
            Division,
            None,
        }
        string First, Second;
        Operation operation;
        public Calculator()
        {
            result = "";
            First = "";
            Second = "";
            operation = Operation.None;
        }
        public void ConcatNewDigit(string singledigit)
        {
            if (operation == Operation.None)
                First += singledigit;
            else
                Second += singledigit;
        }
        public void FeedOperator(Operation op)
        {
            if (First == "")
                return;
            if (operation != Operation.None)
            {
                Operate();
            }
     
            operation = op;
        }

        public void Operate()
        {
            if (First == "")
                return;

            long a = long.Parse(First);

            if (Second.Length == 0)
            {
                result = a.ToString();
                operation = Operation.None;
                First = result;
                Second = "";
                return;
            }

            long b = long.Parse(Second);
            switch (operation)
            {
                case Operation.Add:
                    result = (a + b).ToString();
                    break;
                case Operation.Substraction:
                    result = (a - b).ToString();
                    break;
                case Operation.Multiply:
                    result = (a * b).ToString();
                    break;
                case Operation.Division:
                    if (b == 0)
                        result = "0";
                    else
                    result = (a / b).ToString();
                    break;
            }
            First = result;
            Second = "";

        }

        internal void Paint(Graphics graphics, Point p)
        {
            if (operation == Operation.None)
                graphics.DrawString(First, GlobalClass.SmallfontBold, Brushes.SkyBlue, p);
            else
            graphics.DrawString(Second, GlobalClass.SmallfontBold, Brushes.SkyBlue, p);

            graphics.DrawString(result, GlobalClass.SmallfontBold, Brushes.SkyBlue, p.X, p.Y + 15);
        }
    }
}
