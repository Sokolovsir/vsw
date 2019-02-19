using SimpleGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleGame.Controllers
{
    public class HomeController : Controller
    {
        //массивы для расчетов ходов
        private static int[] freq = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static string[] xo = { "", "", "", "", "", "", "", "" };
        //все возможные ходы кладем в bigmassiv
        private static int[][] bigmassiv = new int[8][] 
        { 
            new int[ ] {1, 2, 3}, 
            new int[ ] {4, 5, 6}, 
            new int[ ] {7, 8, 9},
            new int[ ] {1, 4, 7},
            new int[ ] {2, 5, 8},
            new int[ ] {3, 6, 9},
            new int[ ] {1, 5, 9},
            new int[ ] {7, 5, 3}
        };
        
        private Models.SGDBEntities db = new Models.SGDBEntities();
        public static string gamestatus;
        public static string gamehistory;

        public ActionResult Index(string cell)
        {

            var Item = db.CurrentGame;

            return View(Item);

        }

        public ActionResult Game(int cell)
        {
            gamestatus = "";
            var Item = db.CurrentGame;
            if (cell == 0)
            {
                EmptyMass();
                for (int i = 1; i < 10; i++)
                {
                    var v = db.CurrentGame.Where(x => x.Cell == i).FirstOrDefault();
                    v.Value = "";
                }
                db.SaveChanges();                
            }
            else
            {
                var X = db.CurrentGame.Where(x => x.Cell == cell).FirstOrDefault();
                X.Value = "x";
                db.SaveChanges();
                MarkLines(cell, "x");
                if (db.CurrentGame.Where(x => x.Value != "").Count() != 9)
                {
                    int ocell = GetCelForO(cell);
                    var O = db.CurrentGame.Where(x => x.Cell == ocell).FirstOrDefault();
                    O.Value = "o";
                    db.SaveChanges();
                    MarkLines(ocell, "o");
                }
                else
                {
                    gamestatus = "Ничья";
                    SaveResult();
                }
            }
            ViewData["Gamestatus"] = gamestatus;
            return View(Item);
        }

        private int GetCelForO(int xcel) //метод пытается найти наиболее подходящую ячейку для вставки нолика
        {
            int ocel = 0;
            int temp;
            int[] m;
            int n;
            int arrsum = 0; //нужна для того. чтобы понять, есть ли у ноликов еще ходы

            for (int i = 0; i < 8; i++)
            {
                //freq[i-1] = 0;
                m = bigmassiv[i];
                //если в данной линии еще не стоит крестик или нолик
                if (xo[i] == "")
                {
                    for (n = 0; n < 3; n++)
                    {
                        temp = m[n];
                        freq[temp - 1]++;
                        arrsum++;
                    }
                }
                //если в данной линии уже есть нолик
                if (xo[i] == "o")
                {
                    for (n = 0; n < 3; n++)
                    {
                        temp = m[n];
                        if (db.CurrentGame.Where(x => x.Cell == temp).FirstOrDefault().Value == "") { freq[temp - 1]++; }
                    }
                    arrsum++;
                }
                if (xo[i] == "xx")
                {
                    for (n = 0; n < 3; n++)
                    {
                        temp = m[n];
                        if (db.CurrentGame.Where(x => x.Cell == temp).FirstOrDefault().Value == "") { ocel = temp; }
                    }
                }
                if (xo[i] == "oo")
                {
                    for (n = 0; n < 3; n++)
                    {
                        temp = m[n];
                        if (db.CurrentGame.Where(x => x.Cell == temp).FirstOrDefault().Value == "")
                        {
                            ocel = temp;
                            xo[i] = "ooo";
                            gamestatus = "Победа O";
                            SaveResult();
                            arrsum++;
                            i = 7; //exit
                        }

                    }
                }
            }
            //если для ноликов уже нет вариантов, ставим в свободную ячейку
            if (arrsum == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    m = bigmassiv[i];
                    for (n = 0; n < 3; n++)
                    {
                        temp = m[n];
                        if (db.CurrentGame.Where(x => x.Cell == temp).FirstOrDefault().Value == "") { ocel = temp; }
                    }
                }
            }
            //если до этого момента мы не определили лучшую ячейку, то ставим в ближайшую с наибольшим количеством ходов
            if (ocel == 0)
            {
                do
                {
                    ocel++;
                }
                while (freq[ocel - 1] != freq.Max());
            }
            Array.Clear(freq, 0, freq.Length);
            return ocel;
        }
        private void MarkLines(int cell, string XorO) //отмечаем в массиве возможные ходы
        {
            for (int i = 0; i < 8; i++)
            {
                int[] m = bigmassiv[i];
                if (m.Contains(cell))
                {
                    if (XorO == "x" & xo[i] == "oo") { xo[i] = "oox"; }
                    if (XorO == "o" & xo[i] == "xx") { xo[i] = "xxo"; }
                    if (XorO == "x" & xo[i] == "xx")
                    {
                        xo[i] = "xxx";
                        gamestatus = "Победа X";
                        SaveResult();

                    }

                    if (xo[i] == "" | xo[i] == XorO)
                    {
                        xo[i] = xo[i] + XorO;
                    }
                    if (xo[i].Length == 1 & xo[i] != XorO)
                    {
                        xo[i] = xo[i] + XorO;
                    }

                }
            }            
            gamehistory = gamehistory + Convert.ToString(cell) + XorO + " ";
        }

        //*******************************************************************************************************************************
        public void EmptyMass() // очистка массивов
        {
            gamehistory = "";
            gamestatus = "";
            
            Array.Clear(freq, 0, freq.Length);
            for (int i = 0; i < 8; i++)
            {
                xo[i] = "";
            }

            Array.Clear(freq, 0, freq.Length);

        }
        public void SaveResult()
        {
            //gamenumber = db.Result.Where(x => x.Game != null).Count() + 1;
            Result sr = new Result
            {
                Id = db.Result.Where(x => x.Game != null).Count() + 1,
                Game = 0,
                Gamer = gamehistory,
                Gameresult = gamestatus
            };
            db.Result.Add(sr);
            db.SaveChanges();
        }
    }
}