using System;
using System.IO;
using System.Linq;

namespace Motor
{
    class myMotor
    {
        protected int   I,//Момент инерции 
                        T;// Темп. перегрева
        protected float Hm,//Коэф. зависимости скорости нагрева от крутящего момента
                        Hv,//Коэф. зависимости скорости нагрева от скорости вращения коленвала
                         C;//Коэф. зависимости скорости охл. от температуры двигателя и окружающей среды 
    }

    class benzMotor : myMotor
    {
        private  int[] M,//Крутящий момент             |  Кусочно-линейная зависимость. Кр момент зависит 
                      V;//Скорость вращения коленвала | от скорости вращения

        public benzMotor(string path)
        {//Инициализация класса с файла
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                I = int.Parse(sr.ReadLine().ToString());
                M = sr.ReadLine().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).
                    Select(x => int.Parse(x)).ToArray();
                V = sr.ReadLine().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).
                    Select(x => int.Parse(x)).ToArray();
                T = int.Parse(sr.ReadLine().ToString());
               Hm = float.Parse(sr.ReadLine().ToString());
               Hv = float.Parse(sr.ReadLine().ToString());
                C = float.Parse(sr.ReadLine().ToString());
            }
        }
        public benzMotor(int I, int[] M, int[] V, int T, float Hm, float Hv, float C)
        {//Инициализация класса 
                this.I = I;
                this.M = M;
                this.V = V;
                this.T = T;
                this.Hm = Hm;
                this.Hv = Hv;
                this.C = C;
            }
        
        public float Test(float Tc)
        {//Тестирование мотра на перегрев
            float Tm = Tc,//Температура мотора
                Vcur = 0,//Текущая скорость коленвала
                time = 0,//Время нагрева
                Mcur = 0,//Текущий момент коленвала
                       a,//Ускорение  колевнала = M/I
                      am,//Изменение момента
                      Vh,//Скорость нагрева двигателя =M*Hm+V^2*Hv
                      Vc;//Скорость охлаждения двигателя =C*(Tср-Tдв)
            //===
            int i;
            for (i = 0, Mcur = M[i], Vcur = V[i];//Перемещение по гравику линейной зависимости момента от скорости
                  Tm < T && i + 1 < M.Length;    //До тех по пока не будет перегев или не пройдем весь график
                  i++)
            {
                am = (V[i + 1] - V[i]) / (M[i + 1] - M[i]);//Изменение момента коленвала
                for (; (M[i] < M[i + 1]) ? Mcur < M[i + 1] : Mcur > M[i + 1];//В зависимотси от возрастания/убывания момента ставим соответствующее условие
                    time++//Счетчик времени
                    )
                {
                    a = Mcur / I;//Ускорение коленвала с сооветвующим значением коленвала
                    Vcur += a;//Увеление скорости
                    Vh = Mcur * Hm + (float)Math.Pow(Vcur, 2.0) * Hv;//Скорость нагрева
                    Vc = C * (Tc - Tm);//Скорость охлажения
                    Tm += (Vh - Vc);//Изменение температуры двигателя
                    if (Tm >= T)//Если двигатель перегрелся
                        break;
                    
                    if (M[i] < M[i + 1])//Изменение коленвала
                        Mcur += am;//увелечение оборотов
                    else
                        Mcur -= am;//уменьшение оборота
                }
            }
            //==
            return time;
        }
    }
    class Stand {
        static public float benzTest (benzMotor motor,float Tc)
        {//Тестирование бензинового мотора на перегрев
            return motor.Test(Tc);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            benzMotor motor = new benzMotor("data.txt");
            Console.WriteLine("Введите темп. окружающей среды: ");
            float Tc = float.Parse(Console.ReadLine());// Температура среды
            Console.WriteLine("time=" + Stand.benzTest(motor,Tc)+" sec.");
        }
    }
}
