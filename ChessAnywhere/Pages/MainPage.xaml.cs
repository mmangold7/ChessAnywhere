using ChessAnywhere.Models;
using ChessAnywhere.PageModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace ChessAnywhere.Pages
{
    public partial class MainPage : ContentPage
    {
        private SKColor DarkColor { get; set; } = SKColors.Black;
        private SKColor LightColor { get; set; } = SKColors.White;

        private ChessTile[,] ChessBoard { get; set; }

        public class ChessPiece(ChessPieceName name, bool isDark)
        {
            public ChessPieceName Name { get; set; } = name;
            public bool IsDark { get; set; } = isDark;
        }

        public enum ChessPieceName
        {
            Pawn = 0,
            Rook = 1,
            Knight = 2,
            Bishop = 3,
            King = 4,
            Queen = 5
        }

        public static string GetAbbreviation(ChessPieceName piece)
        {
            var name = piece.ToString();
            return name.Length >= 2
                ? name.Substring(0, 2)
                : name;
        }

        public enum ColumnNames
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            E = 5,
            F = 6,
            G = 7,
            H = 8
        }

        public class ChessTile
        {
            public readonly bool IsDarkColor;
            public ChessPiece? OccupiedBy;

            public ChessTile(bool isDarkTile, ChessPieceName? initialPiece = null, bool? isDarkPiece = null)
            {
                IsDarkColor = isDarkTile;
                if (initialPiece.HasValue && isDarkPiece.HasValue)
                    OccupiedBy = new ChessPiece(initialPiece.Value, isDarkPiece.Value);
            }
        }

        public MainPage(MainPageModel model)
        {
            ChessBoard = BuildStandardChessBoard();

            InitializeComponent();
            BindingContext = model;
        }

        private ChessTile[,] BuildStandardChessBoard()
        {
            ChessTile[,] standardBoard = {
                {
                    new(true,  ChessPieceName.Rook, false),
                    new(false, ChessPieceName.Knight, false),
                    new(true,  ChessPieceName.Bishop, false),
                    new(false, ChessPieceName.King, false),
                    new(true,  ChessPieceName.Queen, false),
                    new(false, ChessPieceName.Bishop, false),
                    new(true,  ChessPieceName.Knight, false),
                    new(false, ChessPieceName.Rook, false)
                },
                {
                    new(false, ChessPieceName.Pawn, false),
                    new(true,  ChessPieceName.Pawn, false),
                    new(false, ChessPieceName.Pawn, false),
                    new(true,  ChessPieceName.Pawn, false),
                    new(false, ChessPieceName.Pawn, false),
                    new(true,  ChessPieceName.Pawn, false),
                    new(false, ChessPieceName.Pawn, false),
                    new(true,  ChessPieceName.Pawn, false)
                },
                {
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false)
                },
                {
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true)
                },
                {
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false)
                },
                {
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true),
                    new(false),
                    new(true)
                },
                {
                    new(true, ChessPieceName.Pawn, true),
                    new(false,  ChessPieceName.Pawn, true),
                    new(true, ChessPieceName.Pawn, true),
                    new(false,  ChessPieceName.Pawn, true),
                    new(true, ChessPieceName.Pawn, true),
                    new(false,  ChessPieceName.Pawn, true),
                    new(true, ChessPieceName.Pawn, true),
                    new(false,  ChessPieceName.Pawn, true)
                },
                {
                    new(false,  ChessPieceName.Rook, true),
                    new(true, ChessPieceName.Knight, true),
                    new(false,  ChessPieceName.Bishop, true),
                    new(true, ChessPieceName.King, true),
                    new(false,  ChessPieceName.Queen, true),
                    new(true, ChessPieceName.Bishop, true),
                    new(false,  ChessPieceName.Knight, true),
                    new(true, ChessPieceName.Rook, true)
                }
            };

            return standardBoard;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            var boardWidth = Math.Min(info.Width, info.Height);
            var tileWidth = boardWidth / 8;
            var xOffset = (info.Width - boardWidth) / 2;
            var yOffset = (info.Height - boardWidth) / 2;

            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            canvas.Translate(xOffset, yOffset);
            DrawChessBoard(canvas, tileWidth);
        }

        private void DrawChessBoard(SKCanvas canvas, int tileWidth)
        {
            for (var rowIndex = 0; rowIndex < 8; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < 8; columnIndex++)
                {
                    var currentTile = ChessBoard[rowIndex, columnIndex];
                    canvas.DrawRect(
                        rowIndex * tileWidth, 
                        columnIndex * tileWidth,
                        tileWidth, 
                        tileWidth,
                        new SKPaint() { Color = currentTile.IsDarkColor ? DarkColor : LightColor });

                    if (currentTile.OccupiedBy != null)
                    {
                        canvas.DrawRect(
                            
                            );

                        canvas.DrawText(
                            GetAbbreviation(currentTile.OccupiedBy.Name),
                            x: ,
                            y: ,
                            SKTextAlign.Center,
                            new SKFont(SKTypeface.Default),
                            paint: );
                    }
                }
            }
        }
    }
}