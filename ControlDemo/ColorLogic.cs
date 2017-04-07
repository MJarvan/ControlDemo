using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace ChartControls.Logic
{
    public class ColorLogic
    {
        public static LinkedList<Color> GetAllColor()
        {
            LinkedList<Color> colors = new LinkedList<Color>();
            Type t = typeof(System.Drawing.Color);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (property.PropertyType.ToString() == "System.Drawing.Color")
                {
                    colors.AddLast((Color)property.GetValue(t, null));
                }
            }
            return colors;
        }

        public static List<Color> GetColors(int count)
        {
            int interval = 40; //RGB差异均小于此值的颜色将被判断为相似颜色

            List<Color> Colors = new List<Color>();
            bool hasFinded = false;
            for (int i = 0; i < count; i++)
            {
                hasFinded = true;
                //判断是否有相似的颜色
                while (hasFinded)
                {
                    hasFinded = false;
                    Color colorToSearch = GetDarkerColor(GetRandomColor());
                    if (Math.Abs(colorToSearch.R - Color.White.R) < interval &&
                             Math.Abs(colorToSearch.G - Color.White.G) < interval &&
                             Math.Abs(colorToSearch.B - Color.White.B) < interval)
                    {
                        hasFinded = true;
                    }
                    else if (Math.Abs(colorToSearch.R - Color.Black.R) < interval &&
                             Math.Abs(colorToSearch.G - Color.Black.G) < interval &&
                             Math.Abs(colorToSearch.B - Color.Black.B) < interval)
                    {
                        hasFinded = true;
                    }
                    else
                    {
                        foreach (Color color in Colors)
                        {
                            if (Math.Abs(colorToSearch.R - color.R) < interval &&
                                Math.Abs(colorToSearch.G - color.G) < interval &&
                                Math.Abs(colorToSearch.B - color.B) < interval)
                            {

                                hasFinded = true;
                                break;

                            }
                        }
                    }

                    if (!hasFinded)
                    {
                        Colors.Add(colorToSearch);
                    }

                }

            }

            return Colors;

        }

        public static System.Drawing.Color GetRandomColor()
        {
            Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
            System.Threading.Thread.Sleep(randomNum_1.Next(1));
            int int_Red = randomNum_1.Next(255);

            Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
            int int_Green = randomNum_2.Next(255);

            Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());

            int int_Blue = randomNum_3.Next(255);
            int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;


            return GetDarkerColor(System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue));
        }

        //获取加深颜色
        public static Color GetDarkerColor(Color color)
        {
            const int max = 255;
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值


            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));

            return Color.FromArgb(r, g, b);
        }


    }
}

