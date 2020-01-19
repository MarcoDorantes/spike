using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tic.spec
{
    enum EventType { Control }
    enum EventState { Accepted }
    class TicEventArgs : EventArgs
    {
        public EventType EventType;
        public EventState Result;
        public char EventData;
    }
    class Tic
    {
        public const int Size = 3;
        public const char X = 'X';
        public const char O = 'O';

        private char[,] grid;

        public Tic()
        {
            grid = new char[Size, Size];
            for (int k = 0; k < Size; ++k)
            {
                for (int j = 0; j < Size; ++j)
                {
                    grid[k, j] = '.';
                }
            }
        }

        public EventHandler<TicEventArgs> onevent;
        public char this[int r, int c]
        {
            get
            {
                return grid[r, c];
            }
            private set { throw new InvalidOperationException("set accesor is not allowed."); }
        }

        public void move(char move, int row, int col)
        {
            grid[row, col] = move;
            onevent?.Invoke(this, new TicEventArgs { EventType = EventType.Control, Result = EventState.Accepted, EventData = move });
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            for (int k = 0; k < Size; ++k)
            {
                for (int j = 0; j < Size; ++j)
                {
                    result.Append(grid[k, j]);
                    if (j + 1 < Size) result.Append('\t');
                }
                if (k + 1 < Size) result.Append(Environment.NewLine);
            }
            return $"{result}";
        }
    }
    [TestClass]
    public class engineSpec
    {
        [TestMethod]
        public void grid1()
        {
            //Arrange
            var expected_view = $"{Tic.X}\t.\t.{Environment.NewLine}.\t{Tic.X}\t.{Environment.NewLine}.\t.\t{Tic.X}";
            var g = new Tic();

            //Act
            g.move(Tic.X, 0, 0);
            g.move(Tic.X, 1, 1);
            g.move(Tic.X, 2, 2);

            //Assert
            Assert.AreEqual(expected_view, $"{g}");
        }

        [TestMethod]
        public void begin()
        {
            //Arrange
            var result = new List<TicEventArgs>();
            void onevent(object from, TicEventArgs e)
            {
                result.Add(e);
            }
            var g = new Tic();
            g.onevent += onevent;
            var expected_move = 'X';
            var expected_view = $"{Tic.X}\t.\t.{Environment.NewLine}.\t.\t.{Environment.NewLine}.\t.\t.";

            //Act
            g.move(Tic.X, 0, 0);
            var actual_move = g[0, 0];
            var actual_view = $"{g}";

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(EventType.Control, result[0].EventType);
            Assert.AreEqual(EventState.Accepted, result[0].Result);
            Assert.AreEqual(Tic.X, result[0].EventData);
            Assert.AreEqual(expected_move, actual_move);
            Assert.AreEqual(expected_view, actual_view);
        }
    }
}