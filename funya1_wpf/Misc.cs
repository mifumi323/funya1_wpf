namespace funya1_wpf
{
    public class Misc
    {
        private int C = 0;

        public static void Change123(ref int x, int Min, int Max)
        {
            x++;
            if (x > Max) x = Min;
        }

        public void Change12321(ref int x, int Min, int Max)
        {
            x += C;
            if (C == 1 && x >= Max) C = -1;
            if (C == 0) C = 1;
            if (C == -1 && x <= Min) C = 1;
        }
    }
}
