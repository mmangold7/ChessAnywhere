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

        private SKColor DarkColor { get; } = SKColors.Black;
        private SKColor LightColor { get; } = SKColors.White;

        private readonly SKPaint _darkTilePaint = new() { Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint _lightTilePaint = new() { Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint _pieceFillPaint = new() { Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint _pieceBorderPaint = new() { Style = SKPaintStyle.Stroke, IsAntialias = true };
        private readonly SKPaint _textPaint = new()
        {
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            Typeface = SKTypeface.Default
        };

        // yellow border for selected square
        private readonly SKPaint _selectPaint = new()
            { Style = SKPaintStyle.Stroke, StrokeWidth = 3, Color = SKColors.Yellow, IsAntialias = true };

        // ───── game state ─────
        private readonly ChessTile[,] ChessBoard;
        private bool _whiteToMove = true;          // White starts
        private (int row, int col)? _selected;     // currently selected square (if any)

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

            // enable touch / click events on SKCanvasView
            CanvasView.EnableTouchEvents = true;
            CanvasView.Touch += OnCanvasTouch;
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

        // ───────────────────────────────── Touch handler
        private void OnCanvasTouch(object? sender, SKTouchEventArgs e)
        {
            if (e.ActionType != SKTouchAction.Released) return;

            // translate pixel→board coordinates
            var info = CanvasView.CanvasSize;
            int boardWidth = (int)Math.Min(info.Width, info.Height);
            int tileWidth = boardWidth / boardSize;
            int xOffset = (int)((info.Width - boardWidth) / 2);
            int yOffset = (int)((info.Height - boardWidth) / 2);

            int px = (int)e.Location.X - xOffset;
            int py = (int)e.Location.Y - yOffset;

            if (px < 0 || py < 0) return;
            int col = px / tileWidth;
            int row = py / tileWidth;
            if (col >= boardSize || row >= boardSize) return;

            var clickedTile = ChessBoard[row, col];
            var clickedPiece = clickedTile.OccupiedBy;

            // 1) nothing selected yet → try to select own piece
            if (_selected is null)
            {
                if (clickedPiece is not null && clickedPiece.IsDark == !_whiteToMove)
                    _selected = (row, col);  // select
            }
            else
            {
                var (selRow, selCol) = _selected.Value;
                var selPiece = ChessBoard[selRow, selCol].OccupiedBy!; // by def. exists

                // same square → deselect
                if (selRow == row && selCol == col)
                {
                    _selected = null;
                }
                // clicked own piece → change selection
                else if (clickedPiece is not null && clickedPiece.IsDark == !_whiteToMove)
                {
                    _selected = (row, col);
                }
                // otherwise attempt move
                else
                {
                    if (IsValidMove(selPiece, selRow, selCol, row, col))
                    {
                        ChessBoard[row, col].OccupiedBy = selPiece;   // capture or move
                        ChessBoard[selRow, selCol].OccupiedBy = null;
                        _selected = null;
                        _whiteToMove = !_whiteToMove; // switch turn
                    }
                }
            }

            CanvasView.InvalidateSurface();
            e.Handled = true;
        }

        // placeholder – replace with full chess rules later
        private static bool IsValidMove(ChessPiece piece, int fromRow, int fromCol, int toRow, int toCol)
        {
            // cannot capture own piece (already guaranteed outside)
            // allow any destination for now
            return true;
        }

        // ───────────────────────────────── Draw
        private void DrawChessBoard(SKCanvas canvas, int tileWidth)
        {
            _darkTilePaint.Color = DarkColor;
            _lightTilePaint.Color = LightColor;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    float x = col * tileWidth;
                    float y = row * tileWidth;

                    bool darkSquare = (row + col) % 2 == 1;
                    canvas.DrawRect(x, y, tileWidth, tileWidth,
                                    darkSquare ? _darkTilePaint : _lightTilePaint);

                    // yellow border if selected
                    if (_selected is { row: var sr, col: var sc } && sr == row && sc == col)
                        canvas.DrawRect(x, y, tileWidth, tileWidth, _selectPaint);

                    var tile = ChessBoard[row, col];
                    if (tile.OccupiedBy is null) continue;

                    bool isDarkPiece = tile.OccupiedBy.IsDark;
                    var pieceColor = isDarkPiece ? DarkColor : LightColor;
                    var borderColor = isDarkPiece ? LightColor : DarkColor;

                    _pieceFillPaint.Color = pieceColor;
                    _pieceBorderPaint.Color = borderColor;
                    _pieceBorderPaint.StrokeWidth = tileWidth * 0.05f;

                    float rectW = tileWidth * 0.5f;
                    float rectH = tileWidth * 0.8f;
                    float rectX = x + (tileWidth - rectW) / 2f;
                    float rectY = y + (tileWidth - rectH) / 2f;

                    canvas.DrawRect(rectX, rectY, rectW, rectH, _pieceFillPaint);
                    canvas.DrawRect(rectX, rectY, rectW, rectH, _pieceBorderPaint);

                    _textPaint.Color = borderColor;
                    _textPaint.TextSize = tileWidth * 0.4f;
                    string abbr = GetAbbreviation(tile.OccupiedBy.Name);

                    var fm = _textPaint.FontMetrics;
                    float tx = rectX + rectW / 2f;
                    float ty = rectY + rectH / 2f - (fm.Ascent + fm.Descent) / 2f;
                    canvas.DrawText(abbr, tx, ty, _textPaint);
                }
            }
        }
    }
}