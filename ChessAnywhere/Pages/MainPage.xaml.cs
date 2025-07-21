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
        private const int boardSize = 8;
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
            for (var rowIndex = 0; rowIndex < boardSize; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < boardSize; columnIndex++)
                {
                    var currentTile = ChessBoard[rowIndex, columnIndex];

                    float x = columnIndex * tileWidth;
                    float y = rowIndex * tileWidth;

                    bool darkSquare = (rowIndex + columnIndex) % 2 == 1;

                    using (var tilePaint = new SKPaint())
                    {
                        tilePaint.Style = SKPaintStyle.Fill;
                        tilePaint.Color = darkSquare ? DarkColor : LightColor;
                        tilePaint.IsAntialias = true;
                        canvas.DrawRect(x, y, tileWidth, tileWidth, tilePaint);
                    }

                    if (currentTile.OccupiedBy is not null)
                    {
                        var isDarkPiece = currentTile.OccupiedBy.IsDark;
                        var pieceColor = isDarkPiece ? DarkColor : LightColor;
                        var borderColor = isDarkPiece ? LightColor : DarkColor;

                        var rectWidth = tileWidth * 0.5f;
                        var rectHeight = tileWidth * 0.8f;

                        var rectX = x + (tileWidth - rectWidth) / 2f;
                        var rectY = y + (tileWidth - rectHeight) / 2f;

                        using (var fillPaint = new SKPaint())
                        {
                            fillPaint.Style = SKPaintStyle.Fill;
                            fillPaint.Color = pieceColor;
                            fillPaint.IsAntialias = true;
                            canvas.DrawRect(rectX, rectY, rectWidth, rectHeight, fillPaint);
                        }

                        using (var borderPaint = new SKPaint())
                        {
                            borderPaint.Style = SKPaintStyle.Stroke;
                            borderPaint.StrokeWidth = 1;
                            borderPaint.Color = borderColor;
                            borderPaint.IsAntialias = true;
                            canvas.DrawRect(rectX, rectY, rectWidth, rectHeight, borderPaint);
                        }

                        var abbr = GetAbbreviation(currentTile.OccupiedBy.Name);
                        using (var textPaint = new SKPaint())
                        {
                            textPaint.Color = borderColor;
                            textPaint.IsAntialias = true;
                            var textX = rectX + rectWidth / 2f;
                            var textY = rectY + rectHeight / 2f;

                            canvas.DrawText(abbr, textX, textY, SKTextAlign.Center, new SKFont(SKTypeface.Default), textPaint);
                        }
                    }
                }
            }
        }
    }
}