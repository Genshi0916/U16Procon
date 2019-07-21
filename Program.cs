using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CHaser;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace CHaser
{
    /// <summary> 
    /// 命令一覧 
    /// </summary> 
    public enum Funcs
    {
        WalkRight,
        WalkLeft,
        WalkUp,
        WalkDown,
        LookRight,
        LookLeft,
        LookUp,
        LookDown,
        SearchRight,
        SearchLeft,
        SearchUp,
        SearchDown,
        PutRight,
        PutLeft,
        PutUp,
        PutDown,
        None /**/
    };


//移動可能箇所 
    /// <summary> 
    /// メインクラス 
    /// </summary> 
    class Program
    {
        const int Floor = 0, Enemy = 1, Block = 2, Item = 3;
        private static List<Funcs> movableLocation;
        private static List<Point> path;
        private static bool isWaiting = false;
        private static Dictionary<Point, int> map = new Dictionary<Point, int>();

        private static Client c;

        private static Point genten = new Point(0, 0);


        static void Main(string[] args)
        {
            int[] value = new int[9];
            bool isAstar = false;
            List<Point> route = new List<Point>();

            c = Client.Create();
            path = new List<Point>();
            path.Add(genten); //原点を追加
            
            if (File.Exists("test.csv"))
            {
                load();
                isAstar = true;
            }


            while (true)
            {
                //ランダム関数を使う 
                Random rnd = new Random((int) DateTime.Now.Ticks);
                movableLocation = new List<Funcs>() {Funcs.WalkLeft, Funcs.WalkUp, Funcs.WalkRight, Funcs.WalkDown};

                value = c.GetReady();
                isWaiting = false; //壁をかわす 
                kawasu(value);
                func(enemyCheck(value)); //アイテムを取る 
                if (!isWaiting || !isAstar)
                {
                    itemGet(value);
                    yukarin(rnd);
                }
                
                if (isAstar)
                {
                    //Astar mode.
                    if (route.Count == 0)
                    {
                        Astar.Init(genten);
                        route = Astar.GetRoute();
                    }
                    else if (movableLocation.Count > 0)
                    {
                        Point moveTo = route[0];
                        if (moveTo.X - genten.X == 1)
                        {
                            //右移動.
                            func(Funcs.WalkRight);
                        }
                        else if (moveTo.X - genten.X == -1)
                        {
                            func(Funcs.WalkLeft);
                        }
                        else if (moveTo.Y - genten.Y == 1)
                        {
                            func(Funcs.WalkDown);
                        }
                        else if (moveTo.Y - genten.Y == -1)
                        {
                            func(Funcs.WalkUp);
                        }
                        else
                        {
                            //例外処理
                            route.Clear();
                        }
                        
                        if (route.Count > 0) route.RemoveAt(0);
                    }
                }

                foreach (var v in movableLocation)
                {
                    Console.WriteLine(v);
                }

                path.Add(genten); //移動経路を保存
                display();
            }
        }

        //敵を倒す関数 
        private static Funcs enemyCheck(int[] value)
        {
            Funcs action = Funcs.None;
            if (value[1] == Enemy) return Funcs.PutUp;
            if (value[3] == Enemy) return Funcs.PutLeft;
            if (value[5] == Enemy) return Funcs.PutRight;
            if (value[7] == Enemy) return Funcs.PutDown;
            //敵を見つけたら待つ 
            if (value[0] == Enemy)
            {
                action = Funcs.LookUp;
                isWaiting = true;
            }


            else if (value[2] == Enemy)
            {
                action = Funcs.LookRight;
                isWaiting = true;
            }


            else if (value[6] == Enemy)
            {
                action = Funcs.LookLeft;
                isWaiting = true;
            }


            else if (value[8] == Enemy)
            {
                action = Funcs.LookDown;
                isWaiting = true;
            }


            return action;
        }

        //アイテムとる関数 
        private static void itemGet(int[] value)
        {
            Funcs action = Funcs.None;
            Random rand = new Random();

            //上下左右にアイテムゲットなのじゃ 
            if (value[1] == Item)
            {
                //アイテムをとって自滅しない
                if (value[0] == Block && value[2] == Block)
                {
                    value = func(Funcs.LookUp);
                    if (value[4] == Block)
                    {
                        movableLocation.Remove(Funcs.WalkUp);
                        c.GetReady();
                        func(Funcs.PutUp);
                    }
                    else
                    {
                        c.GetReady();
                        func(Funcs.WalkUp);
                    }
                }
                else
                {
                    value = func(Funcs.WalkUp);
                }
            }


            else if (value[3] == Item)
            {
                //アイテムをとって自滅しない
                if (value[0] == Block && value[6] == Block)
                {
                    value = func(Funcs.LookLeft);
                    if (value[4] == Block)
                    {
                        movableLocation.Remove(Funcs.WalkLeft);
                        c.GetReady();
                        func(Funcs.PutLeft);
                    }
                    else
                    {
                        c.GetReady();
                        func(Funcs.WalkLeft);
                    }
                }
                else
                {
                    value = func(Funcs.WalkLeft);
                }
            }


            else if (value[5] == Item)
            {
                //アイテムをとって自滅しない
                if (value[2] == Block && value[8] == Block)
                {
                    value = func(Funcs.LookRight);
                    if (value[4] == Block)
                    {
                        movableLocation.Remove(Funcs.WalkRight);
                        c.GetReady();
                        func(Funcs.PutRight);
                    }
                    else
                    {
                        c.GetReady();
                        func(Funcs.WalkRight);
                    }
                }
                else
                {
                    value = func(Funcs.WalkRight);
                }
            }


            else if (value[7] == Item)
            {
                //アイテムをとって自滅しない
                if (value[6] == Block && value[8] == Block)
                {
                    value = func(Funcs.LookDown);
                    if (value[4] == Block)
                    {
                        movableLocation.Remove(Funcs.WalkDown);
                        c.GetReady();
                        func(Funcs.PutDown);
                    }
                    else
                    {
                        c.GetReady();
                        func(Funcs.WalkDown);
                    }
                }
                else
                {
                    value = func(Funcs.WalkDown);
                }
            }

            //ななめアイテム 
            else if (value[0] == Item)
            {
                if (value[1] != Block && value[3] != Block)
                {
                    if (rand.Next(1) == 0)
                    {
                        func(Funcs.WalkUp);
                    }
                    else
                    {
                        func(Funcs.WalkLeft);
                    }
                }
                else if (value[1] != Block)
                {
                    func(Funcs.WalkUp);
                }
                else if (value[3] != Block)
                {
                    func(Funcs.WalkLeft);
                }
                else
                {
                    //yukarin関数. 
                    yukarin(rand);
                }
            }


            else if (value[2] == Item)
            {
                if (value[1] != Block && value[5] != Block)
                {
                    if (rand.Next(1) == 0)
                    {
                        func(Funcs.WalkUp);
                    }
                    else
                    {
                        func(Funcs.WalkRight);
                    }
                }
                else if (value[1] != Block)
                {
                    func(Funcs.WalkUp);
                }
                else if (value[5] != Block)
                {
                    func(Funcs.WalkRight);
                }
                else
                {
                    //yukarin関数. 
                    yukarin(rand);
                }
            }


            else if (value[6] == Item)
            {
                if (value[3] != Block && value[7] != Block)
                {
                    if (rand.Next(1) == 0)
                    {
                        func(Funcs.WalkLeft);
                    }
                    else
                    {
                        func(Funcs.WalkDown);
                    }
                }
                else if (value[3] != Block)
                {
                    func(Funcs.WalkLeft);
                }
                else if (value[7] != Block)
                {
                    func(Funcs.WalkDown);
                }
                else
                {
                    //yukarin関数. 
                    yukarin(rand);
                }
            }


            else if (value[8] == Item)
            {
                if (value[5] != Block && value[7] != Block)
                {
                    if (rand.Next(1) == 0)
                    {
                        func(Funcs.WalkRight);
                    }
                    else
                    {
                        func(Funcs.WalkDown);
                    }
                }
                else if (value[5] != Block)
                {
                    func(Funcs.WalkRight);
                }
                else if (value[7] != Block)
                {
                    func(Funcs.WalkDown);
                }
                else
                {
                    //yukarin関数. 
                    yukarin(rand);
                }
            }
        }

        //壁をよける関数 
        private static void kawasu(int[] value)
        {
            if (value[1] == Block) movableLocation.Remove(Funcs.WalkUp);
            if (value[3] == Block) movableLocation.Remove(Funcs.WalkLeft);
            if (value[5] == Block) movableLocation.Remove(Funcs.WalkRight);
            if (value[7] == Block) movableLocation.Remove(Funcs.WalkDown);
        }


        private static void yukarin(Random yukari)
        {
            if (movableLocation.Count > 0)
            {
                List<Funcs> map = new List<Funcs>();


                foreach (Funcs fun in movableLocation)
                {
                    switch (fun)
                    {
                        case Funcs.WalkUp:
                            //未探索領域をマップに追加
                            if (!isExist(new Point(genten.X, genten.Y - 1)))
                            {
                                
                                map.Add(Funcs.WalkUp);
                            }

                            break;

                        case Funcs.WalkLeft:
                            if (!isExist(new Point(genten.X - 1, genten.Y)))
                            {
                                map.Add(Funcs.WalkLeft);
                            }

                            break;

                        case Funcs.WalkRight:
                            if (!isExist(new Point(genten.X + 1, genten.Y)))
                            {
                                map.Add(Funcs.WalkRight);
                            }

                            break;

                        case Funcs.WalkDown:
                            if (!isExist(new Point(genten.X, genten.Y + 1)))
                            {
                                map.Add(Funcs.WalkDown);
                            }

                            break;
                    }
                }

                if (map.Count > 0)
                {
                    func(map[yukari.Next(map.Count)]);
                }
                else
                {
                    func(movableLocation[yukari.Next(movableLocation.Count)]);
                }
            }
        }

        private static void display()
        {
            foreach (Point p in map.Keys)
            {
                Console.WriteLine("(" + p.X + ", " + p.Y + ")" + "   " + map[p]);
            }
        }

        private static int[] func(Funcs data)
        {
            int[] result = new int[9];
            switch (data)
            {
                case Funcs.WalkRight:
                    result = c.WalkRight();
                    genten = new Point(genten.X + 1, genten.Y);
                    mapAdd(genten, Funcs.WalkRight, result);
                    break;
                case Funcs.WalkLeft:
                    result = c.WalkLeft();
                    genten = new Point(genten.X - 1, genten.Y);
                    mapAdd(genten, Funcs.WalkLeft, result);
                    break;
                case Funcs.WalkUp:
                    result = c.WalkUp();
                    genten = new Point(genten.X, genten.Y - 1);
                    mapAdd(genten, Funcs.WalkUp, result);
                    break;
                case Funcs.WalkDown:
                    result = c.WalkDown();
                    genten = new Point(genten.X, genten.Y + 1);
                    mapAdd(genten, Funcs.WalkDown, result);
                    break;
                case Funcs.LookRight:
                    result = c.LookRight();
                    mapAdd(genten, Funcs.LookRight, result);
                    break;
                case Funcs.LookLeft:
                    result = c.LookLeft();
                    mapAdd(genten, Funcs.LookLeft, result);
                    break;
                case Funcs.LookUp:
                    result = c.LookUp();
                    mapAdd(genten, Funcs.LookUp, result);
                    break;
                case Funcs.LookDown:
                    result = c.LookDown();
                    mapAdd(genten, Funcs.LookDown, result);
                    break;
                case Funcs.SearchRight:
                    result = c.SearchRight();
                    mapAdd(genten, Funcs.SearchRight, result);
                    break;
                case Funcs.SearchLeft:
                    result = c.SearchLeft();
                    mapAdd(genten, Funcs.SearchLeft, result);
                    break;
                case Funcs.SearchUp:
                    result = c.SearchUp();
                    mapAdd(genten, Funcs.SearchUp, result);
                    break;
                case Funcs.SearchDown:
                    result = c.SearchDown();
                    mapAdd(genten, Funcs.SearchDown, result);
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


            if (data != Funcs.None)
            {
                movableLocation.Clear();
            }


            return result;
        }

        /// <summary>
        /// マップを更新する
        /// </summary>
        /// <param name="pos">マップ情報</param>
        /// <param name="func">実行した命令</param>
        /// <param name="value"></param>
        private static void mapAdd(Point pos, Funcs func, int[] value)
        {
            Point[] offsetPoint;

            switch (func)
            {
                case Funcs.LookUp:
                    offsetPoint = new Point[9]
                    {
                        new Point(-1, -3), new Point(0, -3), new Point(1, -3),
                        new Point(-1, -2), new Point(0, -2), new Point(1, -2),
                        new Point(-1, -1), new Point(0, -1), new Point(1, -1),
                    };
                    break;
                case Funcs.LookDown:
                    offsetPoint = new Point[9]
                    {
                        new Point(-1, 1), new Point(0, 1), new Point(1, 1),
                        new Point(-1, 2), new Point(0, 2), new Point(1, 2),
                        new Point(-1, 3), new Point(0, 3), new Point(1, 3),
                    };
                    break;
                case Funcs.LookRight:
                    offsetPoint = new Point[9]
                    {
                        new Point(1, -1), new Point(2, -1), new Point(3, -1),
                        new Point(1, 0), new Point(2, 0), new Point(3, 0),
                        new Point(1, 1), new Point(2, 1), new Point(3, 1),
                    };
                    break;
                case Funcs.LookLeft:
                    offsetPoint = new Point[9]
                    {
                        new Point(-3, -1), new Point(-2, -1), new Point(-1, -1),
                        new Point(-3, 0), new Point(-2, 0), new Point(-1, 0),
                        new Point(-3, 1), new Point(-2, 1), new Point(-1, 1),
                    };
                    break;
                case Funcs.SearchUp:
                    offsetPoint = new Point[9]
                    {
                        new Point(0, -1), new Point(0, -2), new Point(0, -3),
                        new Point(0, -4), new Point(0, -5), new Point(0, -6),
                        new Point(0, -7), new Point(0, -8), new Point(0, -9),
                    };
                    break;
                case Funcs.SearchDown:
                    offsetPoint = new Point[9]
                    {
                        new Point(0, 1), new Point(0, 2), new Point(0, 3),
                        new Point(0, 4), new Point(0, 5), new Point(0, 6),
                        new Point(0, 7), new Point(0, 8), new Point(0, 9),
                    };
                    break;
                case Funcs.SearchRight:
                    offsetPoint = new Point[9]
                    {
                        new Point(1, 0), new Point(2, 0), new Point(3, 0),
                        new Point(4, 0), new Point(5, 0), new Point(6, 0),
                        new Point(7, 0), new Point(8, 0), new Point(9, 0),
                    };
                    break;
                case Funcs.SearchLeft:
                    offsetPoint = new Point[9]
                    {
                        new Point(-1, 0), new Point(-2, 0), new Point(-3, 0),
                        new Point(-4, 0), new Point(-5, 0), new Point(-6, 0),
                        new Point(-7, 0), new Point(-8, 0), new Point(-9, 0),
                    };

                    break;
                case Funcs.WalkUp:
                case Funcs.WalkDown:
                case Funcs.WalkRight:
                case Funcs.WalkLeft:
                case Funcs.PutUp:
                case Funcs.PutDown:
                case Funcs.PutRight:
                case Funcs.PutLeft:
                default:
                    offsetPoint = new Point[9]
                    {
                        new Point(-1, -1), new Point(0, -1), new Point(1, -1),
                        new Point(-1, 0), new Point(0, 0), new Point(1, 0),
                        new Point(-1, 1), new Point(0, 1), new Point(1, 1),
                    };
                    break;
            }

            // マップディクショナリの更新
            for (int i = 0; i < offsetPoint.Length; i++)
            {
                map[new Point(pos.X + offsetPoint[i].X, pos.Y + offsetPoint[i].Y)] = value[i];
            }
        }

        /// <summary>
        /// GetReady用マップ更新
        /// </summary>
        /// <param name="pos">マップ情報</param>
        /// <param name="value"></param>
        private static void mapAdd(Point pos, int[] value)
        {
            Point[] offsetPoint = new Point[9]
            {
                new Point(-1, -1), new Point(0, -1), new Point(1, -1),
                new Point(-1, 0), new Point(0, 0), new Point(1, 0),
                new Point(-1, 1), new Point(0, 1), new Point(1, 1)
            };

            for (int i = 0; i < offsetPoint.Length; i++)
            {
                map[new Point(pos.X + offsetPoint[i].X, pos.Y + offsetPoint[i].Y)] = value[i];
            }
        }

        private static bool isExist(Point po)
        {
            foreach (Point p in path)
            {
                if (p.Equals(po)) return true;
            }

            return false;
        }

        public static void save()
        {
            string csv = "test.csv";
            var encoding = Encoding.GetEncoding("shift_jis");
            StreamWriter sw = new StreamWriter(csv, false, encoding);

            foreach (Point p in map.Keys)
            {
                sw.WriteLine(p.X * -1 + "," + p.Y * -1 + "," + map[p]);
            }

            Console.WriteLine("ファイル書き込み完了");
            sw.Close();
        }

        //csvファイルを読み込む関数 
        public static void load()
        {
            var sr = new StreamReader("test.csv");
            map.Clear();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var values = line.Split(',');
                map.Add(new Point(int.Parse(values[0]), int.Parse(values[1])), int.Parse(values[2]));
            }
        }

        public static List<Point> GetItems()
        {
            List<Point> items = new List<Point>();
            foreach (Point point in map.Keys)
            {
                if (map[point] == Item) items.Add(point);
            }

            return items;
        }
        
        public static int GetValue(Point point)
        {
            foreach(Point po in map.Keys)
            {
                if(po.X == point.X && po.Y == point.Y)
                {
                    return map[po];
                }
            }
            return -1;
        }
    }

    class Astar
    {
        private static List<Node> openList;
        private static List<Point> items = Program.GetItems();
        private static Point position;
        private static Point nearestItem;
        private static List<Point> path;

        //初期化関数.
        public static void Init(Point current)
        {
            openList = new List<Node>();
            path = new List<Point>();
            position = current;
            //現在地のノードをオープンする.
            Node node = new Node(position, null);
            node.SetCost(0);
            node.SetHcost(0);
            node.SetScore(node.GetCost() + node.GetHcost());
            nearestItem = GetNearestItem();
            Calc(node);
        }

        //現在地からもっとも近いアイテムの座標を返す関数.
        private static Point GetNearestItem()
        {
            int min = Int32.MaxValue;
            Point nearestItem;
            foreach (var point in items)
            {
                int hcost = Math.Abs(point.X - position.X) + Math.Abs(point.Y - position.Y);
                if (hcost > min) continue;
                min = hcost;
                nearestItem = point;
            }

            return nearestItem;
        }

        //実際にAstarアルゴリズムを使って計算する.
        public static void Calc(Node parent)
        {
            if (parent.GetPoint().X == position.X && parent.GetPoint().Y == position.Y)
            {
                //移動経路を取得する関数を呼び出して、ループを抜ける.
                GetPath(parent);
                return;
            }

            //上下左右のノードをオープンする.
            Point up = new Point(parent.GetPoint().X, parent.GetPoint().Y - 1);
            Point down = new Point(parent.GetPoint().X, parent.GetPoint().Y + 1);
            Point rgt = new Point(parent.GetPoint().X + 1, parent.GetPoint().Y);
            Point lft = new Point(parent.GetPoint().X - 1, parent.GetPoint().Y);
            
            
            if(Program.GetValue(up) != 2 && Program.GetValue(up)  > -1)
            {
                Node top = new Node(new Point(parent.GetPoint().X, parent.GetPoint().Y - 1), parent);
                top.SetCost(parent.GetCost() + 1);
                top.SetHcost(Math.Abs(nearestItem.X - top.GetPoint().X) + Math.Abs(nearestItem.Y - top.GetPoint().Y));
                top.SetScore(top.GetCost() + top.GetHcost());
                openList.Add(top);
            }
            
            if(Program.GetValue(down) != 2 && Program.GetValue(down)  > -1)
            {
                Node bottom = new Node(new Point(parent.GetPoint().X, parent.GetPoint().Y + 1), parent);
                bottom.SetCost(parent.GetCost() + 1);
                bottom.SetHcost(Math.Abs(nearestItem.X - bottom.GetPoint().X) + Math.Abs(nearestItem.Y - bottom.GetPoint().Y));
                bottom.SetScore(bottom.GetCost() + bottom.GetHcost());
                openList.Add(bottom);
            }
            if(Program.GetValue(rgt) != 2 && Program.GetValue(rgt)  > -1)
            {
                Node right = new Node(new Point(parent.GetPoint().X + 1, parent.GetPoint().Y), parent);
                right.SetCost(parent.GetCost() + 1);
                right.SetHcost(Math.Abs(nearestItem.X - right.GetPoint().X) + Math.Abs(nearestItem.Y - right.GetPoint().Y));
                right.SetScore(right.GetCost() + right.GetHcost());
                openList.Add(right);
            }
            if(Program.GetValue(lft) != 2 && Program.GetValue(lft)  > -1)
            {
                Node left = new Node(new Point(parent.GetPoint().X - 1, parent.GetPoint().Y), parent);
                left.SetCost(parent.GetCost() + 1);
                left.SetHcost(Math.Abs(nearestItem.X - left.GetPoint().X) + Math.Abs(nearestItem.Y - left.GetPoint().Y));
                left.SetScore(left.GetCost() + left.GetHcost());
                openList.Add(left);
            }

            Calc(SearchMinScoreNodeFromOpenList());
        }

        /// 最小スコアのノードを取得する.
        public static Node SearchMinScoreNodeFromOpenList()
        {
            // 最小スコア
            int min = 9999;
            // 最小実コスト
            int minCost = 9999;
            Node minNode = null;
            foreach (Node node in openList)
            {
                int score = node.GetScore();
                if (score > min)
                {
                    // スコアが大きい
                    continue;
                }

                if (score == min && node.GetCost() >= minCost)
                {
                    // スコアが同じときは実コストも比較する
                    continue;
                }

                // 最小値更新.
                min = score;
                minCost = node.GetCost();
                minNode = node;
            }

            return minNode;
        }

        private static void GetPath(Node node)
        {
            path.Add(node.GetPoint());
            if (node.GetParent() != null) GetPath(node.GetParent());
            path.Reverse();
            path.RemoveAt(0);
        }
        
        public static List<Point> GetRoute()
        {
            return path;
        }
    }

    class Node
    {
        private Point point;
        private int cost;
        private int hcost;
        private int score;
        private Node parent;

        public enum Status
        {
            Open,
            Close,
            None
        }

        private Status status = Status.None;

        public int GetCost()
        {
            return cost;
        }

        public int GetHcost()
        {
            return hcost;
        }

        public int GetScore()
        {
            return score;
        }

        public Node GetParent()
        {
            return parent;
        }

        public Point GetPoint()
        {
            return point;
        }

        public Status GetStatus()
        {
            return status;
        }

        public void SetCost(int cost)
        {
            this.cost = cost;
        }

        public void SetHcost(int hcost)
        {
            this.hcost = hcost;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }

        public void SetStatus(Status state)
        {
            status = state;
        }

        public Node(Point point, Node parent)
        {
            this.point = point;
            this.parent = parent;
        }
    }
}