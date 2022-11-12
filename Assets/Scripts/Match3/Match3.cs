using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3Gen
{
    public class Match3 : MonoBehaviour
    {
        public ArrayLayout boardLayout;
        public Sprite[] pieces;
        Node[,] board;
        int width = 9;
        int height = 14;
        System.Random random;

        private void Start()
        {

        }
        void StartGame()
        {
            board = new Node[width, height];
            string seed = GetRandomSeed();
            InitializeBoard();
        }

        private string GetRandomSeed()
        {
            string seed = "";
            string acceptables = "ABCDEFGHIJKLMNOPQRSTVUWxyzabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*";
            for (int i = 0; i < 20; i++)
            {
                seed += acceptables[UnityEngine.Random.Range(0, acceptables.Length)];
                random = new System.Random(seed.GetHashCode());

            }
            return seed;
        }
        void InitializeBoard()
        {
            board = new Node[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : FillPiece(), new Point(x, y));
                }

            }

        }

        void VerifyBoard()
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point p = new Point(x, y);
                    int val = GetValueAtPoint(p);
                    if (val <= 0) continue;

                }

            }
        }
        List<Point> isConnected(Point p, bool main)
        {
            List<Point> connected = new List<Point>();
            int val = GetValueAtPoint(p);
            Point[] directions = { Point.Up, Point.Down, Point.Right, Point.Left };
            foreach (Point dir in directions)//checking if there are two or more shapes of same
            {
                List<Point> line = new List<Point>();
                int same = 0;

                for (int i = 1; i < 3; i++)
                {
                    Point check = Point.Add(p, Point.Multi(dir, i));
                    if (GetValueAtPoint(check) == val)
                    {
                        line.Add(check);
                        same++;
                    }
                }
                if (same > 1)//if there are more than 1 of the same shape in the direction we know it is match
                    AddPoints(ref connected, line);//Add these points to the overarching connected list

            }
            for (int i = 0; i < 2; i++)//chekcing if we are in the middle of two same shapes
            {
                List<Point> line = new List<Point>();

                int same = 0;
                Point[] check = { Point.Add(p, directions[i]),
                    Point.Add(p, directions[i + 2]) };
                foreach (Point next in check)//check both sides of piece if there are same
                {
                    if (GetValueAtPoint(next) == val)
                    {
                        line.Add(next);
                        same++;
                    }

                }
                if (same > 1)
                    AddPoints(ref connected, line);

            }
            for (int i = 0; i < 4; i++)//checks for 2x2
            {
                List<Point> square = new List<Point>();

                int same = 0;
                int next = i + 1;
                if (next >= 4)
                    next -= 4;

                Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[next]), Point.Add(p, Point.Add(directions[i], directions[next])) };
                foreach (Point pnt in check)//check both sides of piece if there are same
                {
                    if (GetValueAtPoint(pnt) == val)
                    {
                        square.Add(pnt);
                        same++;
                    }

                }
                if (same > 2)
                    AddPoints(ref connected, square);

            }

            if (main)//check for other matches along current match
            {
                for (int i = 0; i < connected.Count; i++)
                {
                    AddPoints(ref connected, isConnected(connected[i], false));
                }
            }
            if (connected.Count > 0)
                connected.Add(p);

            return connected;
          
        }

        void AddPoints(ref List<Point> points, List<Point> add)
        {
            foreach ( Point p in add)
            {
                bool doAdd = true;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].Equals(p))
                    {
                        doAdd = false;
                        break;
                    }
                }
            }
        }
        int FillPiece()
        {
            int val = 1;
            val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
            return val;
        }

        int GetValueAtPoint(Point p)
        {
            return board[p.x, p.y].value;
        }
    }

    [System.Serializable]
    public class Node
    {
        public int value;//symbols from 0 to 4
        public Point index;

        public Node(int v, Point _index)
        {
            value = v;
            index = _index;
        }
    }
}
