using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CHaser;

namespace u16asahikawaBotv2
{
    /// <summary>
    /// 命令一覧
    /// </summary>
    public enum Funcs
    {
        WalkRight, WalkLeft, WalkUp, WalkDown,
        LookRight, LookLeft, LookUp, LookDown,
        SearchRight, SearchLeft, SearchUp, SearchDown,
        PutRight, PutLeft, PutUp, PutDown, None
    };



    //移動可能箇所

    /// <summary>
    /// メインクラス
    /// </summary>
    class Program
    {
        const int Floor = 0, Enemy = 1, Block = 2, Item = 3;
        private static List<Funcs> movableLocation = new List<Funcs>() { Funcs.WalkLeft, Funcs.WalkUp, Funcs.WalkRight, Funcs.WalkDown };

        private static Client c = Client.Create();

        static void Main(string[] args)
        {
            int[] value = new int[9];


            while (true)
            {
                //ランダム関数を使う
                Random rnd = new Random();
                int rand = rnd.Next(1, 5);
                rand = 2 * rand - 1;



                //敵を倒す
                for (int i = 0; i < value.Length; i++)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    {
                        func(enemyCheck(value));
                        movableLocation = enemyLocCalcRemoveList(i, movableLocation);
                    }
                }

                //アイテムを取る
                for (int i = 0; i < value.Length; i++)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    {
                        func(itemGet(value));
                    }
                    movableLocation = enemyLocCalcRemoveList(i, movableLocation);
                }
                //壁をかわす
                for (int i = 0; i < value.Length; i++)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    {
                        func(kawasu(value));
                    }
                    movableLocation = enemyLocCalcRemoveList(i, movableLocation);
                }

                if (value[rand] != 2)
                {

                    switch (rand)
                    {

                        case 1:
                            value = c.WalkUp();
                            break;

                        case 3:
                            value = c.WalkLeft();
                            break;

                        case 5:
                            value = c.WalkRight();
                            break;

                        case 7:
                            value = c.WalkDown();
                            break;
                    }
                }

            }

        }

        //敵を倒す関数
        private static Funcs enemyCheck(int[] value)
        {
            Funcs action = Funcs.None;
            if (value[1] == Enemy) action = Funcs.PutUp;
            if (value[3] == Enemy) action = Funcs.PutLeft;
            if (value[5] == Enemy) action = Funcs.PutRight;
            if (value[7] == Enemy) action = Funcs.PutDown;

            return action;
        }


        //アイテムとる関数
        private static Funcs itemGet(int[] value)
        {
            Funcs action = Funcs.None;
            if (value[1] == Item) action = Funcs.WalkUp;
            if (value[3] == Item) action = Funcs.WalkLeft;
            if (value[5] == Item) action = Funcs.WalkRight;
            if (value[7] == Item) action = Funcs.WalkDown;

            return action;
        }


        //壁をよける関数
        private static Funcs kawasu(int[] value)
        {
            Funcs action = Funcs.None;
            if (value[1] == Block) enemyLocCalcRemoveList(1, movableLocation);
            if (value[3] == Block) enemyLocCalcRemoveList(3, movableLocation);
            if (value[5] == Block) enemyLocCalcRemoveList(5, movableLocation);
            if (value[7] == Block) enemyLocCalcRemoveList(7, movableLocation);

            return action;
        }
        private static List<Funcs> enemyLocCalcRemoveList(int direction, List<Funcs> movableLocation)
        {

            switch (direction)
            {
                case 0:
                    movableLocation.Remove(Funcs.WalkLeft);
                    movableLocation.Remove(Funcs.WalkUp);
                    break;
                case 1:
                    movableLocation.Remove(Funcs.WalkUp);
                    break;
                case 2:
                    movableLocation.Remove(Funcs.WalkUp);
                    movableLocation.Remove(Funcs.WalkRight);
                    break;
                case 3:
                    movableLocation.Remove(Funcs.WalkLeft);
                    break;
                case 4: // 敵が乗っかってる？
                    movableLocation.Remove(Funcs.WalkRight);
                    movableLocation.Remove(Funcs.WalkUp);
                    movableLocation.Remove(Funcs.WalkLeft);
                    movableLocation.Remove(Funcs.WalkDown);
                    break;
                case 5:
                    movableLocation.Remove(Funcs.WalkLeft);
                    break;
                case 6:
                    movableLocation.Remove(Funcs.WalkLeft);
                    movableLocation.Remove(Funcs.WalkDown);
                    break;
                case 7:
                    movableLocation.Remove(Funcs.WalkDown);
                    break;
                case 8:
                    movableLocation.Remove(Funcs.WalkDown);
                    movableLocation.Remove(Funcs.WalkRight);
                    break;
                default:
                    break;
            }

            return movableLocation;
        }




        private static int[] func(Funcs data)
        {
            int[] result = new int[9];
            switch (data)
            {
                case Funcs.WalkRight:
                    result = c.WalkRight();
                    break;
                case Funcs.WalkLeft:
                    result = c.WalkLeft();
                    break;
                case Funcs.WalkUp:
                    result = c.WalkUp();
                    break;
                case Funcs.WalkDown:
                    result = c.WalkDown();
                    break;
                case Funcs.LookRight:
                    result = c.LookRight();
                    break;
                case Funcs.LookLeft:
                    result = c.LookLeft();
                    break;
                case Funcs.LookUp:
                    result = c.LookUp();
                    break;
                case Funcs.LookDown:
                    result = c.LookDown();
                    break;
                case Funcs.SearchRight:
                    result = c.SearchRight();
                    break;
                case Funcs.SearchLeft:
                    result = c.SearchLeft();
                    break;
                case Funcs.SearchUp:
                    result = c.SearchUp();
                    break;
                case Funcs.SearchDown:
                    result = c.SearchDown();
                    break;
                case Funcs.PutRight:
                    result = c.PutRight();
                    break;
                case Funcs.PutLeft:
                    result = c.PutLeft();
                    break;
                case Funcs.PutUp:
                    result = c.PutUp();
                    break;
                case Funcs.PutDown:
                    result = c.PutDown();
                    break;

            }
            return result;
        }
    }
}