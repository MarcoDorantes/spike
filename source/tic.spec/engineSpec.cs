using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tic.spec
{
    enum EventType { Control }
    enum EventState { Accepted, Rejected }
    class TicEventArgs : EventArgs
    {
        public ushort Sequence;
        public EventType EventType;
        public EventState Result;
        public char EventData;
    }
    class Tic
    {
        public const int Size = 3;
        public const char X = 'X';
        public const char O = 'O';
        public const char None = '.';

        private char[,] grid;
        private ushort sequence;

        public Tic()
        {
            grid = new char[Size, Size];
            sequence = 0;
            for (int k = 0; k < Size; ++k)
            {
                for (int j = 0; j < Size; ++j)
                {
                    grid[k, j] = None;
                }
            }
        }

        public EventHandler<TicEventArgs> onevent;
        public char this[int r, int c]
        {
            get
            {
                //TODO input validation
                return grid[r, c];
            }
            private set { throw new InvalidOperationException("set accesor is not allowed."); }
        }

        public void move(char move, int row, int col)
        {
            var state = EventState.Rejected;
            if (grid[row, col] == None)
            {
                state = EventState.Accepted;
                grid[row, col] = move;
            }
            onevent?.Invoke(this, new TicEventArgs { Sequence = ++sequence, EventType = EventType.Control, Result = state, EventData = move });
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
        public void start()
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
            Assert.AreEqual(1, result[0].Sequence);
            Assert.AreEqual(expected_move, actual_move);
            Assert.AreEqual(expected_view, actual_view);
        }

        [TestMethod]
        public void repeated()
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
            g.move(Tic.X, 0, 0);
            var actual_move = g[0, 0];
            var actual_view = $"{g}";

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(EventType.Control, result[0].EventType);
            Assert.AreEqual(EventState.Accepted, result[0].Result);
            Assert.AreEqual(Tic.X, result[0].EventData);
            Assert.AreEqual(1, result[0].Sequence);
            Assert.AreEqual(EventType.Control, result[1].EventType);
            Assert.AreEqual(EventState.Rejected, result[1].Result);
            Assert.AreEqual(Tic.X, result[1].EventData);
            Assert.AreEqual(2, result[1].Sequence);

            Assert.AreEqual(expected_move, actual_move);
            Assert.AreEqual(expected_view, actual_view);
        }
    }
}