using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using HPGrid.Models;

namespace HPGrid
{
    class DrawGrid : Container
    {
        public List<GridPhase> Phases = null;
        public bool ShowDesc = false;
        private Texture2D _arrowImg;
        private List<ArrowNote> _arrowNotes = new List<ArrowNote>();
        private Point _resolution = new Point(0, 0);

            
        public DrawGrid()
        {
            _resolution = new Point(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            
            Disposed += OnDisposed; 
            this.Visible = true;
            _arrowImg = Module.ModuleInstance.ContentsManager.GetTexture("arrow.png");
            GameService.Input.Mouse.LeftMouseButtonPressed += OnMousePressed;
        }
        private void OnDisposed(object sender, EventArgs e)
        {
            GameService.Input.Mouse.LeftMouseButtonPressed -= OnMousePressed;
        }
        private void OnMousePressed(object o, MouseEventArgs e)
        {
            foreach (ArrowNote _note in _arrowNotes)
            {
                if (_note.InRadius(new Vector2(Input.Mouse.Position.X, Input.Mouse.Position.Y), 6))
                {
                    ScreenNotification.ShowNotification(_note.Note);
                }
            }
        }

        public void SetSize()
        {
            int w = 0;
            int h = 0;
            int x = 0;
            int y = 0;

            float UIScale = GameService.Graphics.UIScaleMultiplier;

            switch (GameService.Gw2Mumble.UI.UISize)
            {
                case Gw2Sharp.Mumble.Models.UiSize.Larger:
                    w = (int)(311 / UIScale);
                    h = (int)(18 / UIScale);
                    y = (int)(106 / UIScale);
                    x = (int)((_resolution.X / 2 - 218) / UIScale);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Large:
                    w = (int)(280 / UIScale);
                    h = (int)(15 / UIScale);
                    y = (int)(97 / UIScale);
                    x = (int)((_resolution.X / 2 - 198) / UIScale);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Normal:
                    w = (int)(253 / UIScale);
                    h = (int)(15 / UIScale);
                    y = (int)(87 / UIScale);
                    x = (int)((_resolution.X / 2 - 178) / UIScale);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Small:
                    w = (int)(228 / UIScale);
                    h = (int)(13 / UIScale);
                    y = (int)(78 / UIScale);
                    x = (int)((_resolution.X / 2 - 160) / UIScale);
                    break;
            }

            this.Size = new Point(w + 15, h + 15);
            this.Location = new Point(x, y);
        }
        public override void PaintAfterChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            //spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(0, 0, Size.X-15, Size.Y-15), Color.White);

            if (Phases != null)
            {
                _arrowNotes.Clear();
                foreach (GridPhase phase in Phases)
                {
                    Color color = FindColor(phase.Color);
                    float pct = (float)phase.Percent / 100f * ((float)this.Size.X-15);
                    spriteBatch.DrawOnCtrl(this,
                        ContentService.Textures.Pixel,
                        new Rectangle((int)pct, 0, 1, Size.Y - 15),
                        null,
                        color
                        );
                    if (this.ShowDesc)
                    {
                        spriteBatch.DrawOnCtrl(this,
                            _arrowImg,
                            new Rectangle((int)pct - 5, Size.Y - 15, 11, 11),
                            null,
                            Color.White
                            );
                        _arrowNotes.Add(
                            new ArrowNote()
                            {
                                Loc = new Vector2(this.Location.X + (int)pct, this.Location.Y + Size.Y - 8),
                                Note = phase.Percent + "%: " + phase.Description
                            });
                    }
                }
            }
        }
        private Color FindColor(string colorname)
        {
            if (colorname == null) colorname = "Black";
            System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorname);
            return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

    }
    public class ArrowNote
    {
        public Vector2 Loc { get; set; }

        public string Note { get; set; }
        public bool InRadius(Vector2 point, float radius)
        {
            float distance = (this.Loc - point).Length();
            if (distance <= radius)
                return true;
            else
                return false;
        }
    }
}
