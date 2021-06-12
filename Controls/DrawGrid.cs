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

    class DrawGrid : Container
    {
        public List<GridLine> Lines = null;
        Texture2D arrow;
        List<ArrowNote> arrowNotes = new List<ArrowNote>();

        public DrawGrid()
        {
            Disposed += OnDisposed; 
            this.Visible = true;
            arrow = Module.ModuleInstance.ContentsManager.GetTexture("arrow.png");
            GameService.Input.Mouse.LeftMouseButtonPressed += OnMousePressed;
        }
        private void OnDisposed(object sender, EventArgs e)
        {
            GameService.Input.Mouse.LeftMouseButtonPressed -= OnMousePressed;
        }
        private void OnMousePressed(object o, MouseEventArgs e)
        {
            foreach (ArrowNote _note in arrowNotes)
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
            //float ratio_w = 0;
            //float ratio_h = 0;
            float ratio_x = 0;
            //float ratio_y = 0;

            switch (GameService.Gw2Mumble.UI.UISize)
            {
                case Gw2Sharp.Mumble.Models.UiSize.Larger:
                    //larger = 1.103
                    //741,106 - 1051,122 @ 1920x1080
                    //ratio_w = ((1051f - 741f) / 1920f);
                    //ratio_h = ((122f - 106f) / 1080f);
                    //ratio_y = (107f / 1080f);
                    w = (int)((1053 - 742) / UIScale);
                    h = (int)((124 - 106) / UIScale);
                    y = (int)(106 / UIScale);
                    ratio_x = (742f / 1920f);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Large:
                    //large = 1
                    //762,96 - 1043,111 @ 1920x1080
                    //ratio_w = ((1043f - 762f) / 1920f);
                    //ratio_h = ((111f - 96f) / 1080f);
                    //ratio_y = (97f / 1080f);
                    w = (int)((1043 - 763) / UIScale);
                    h = (int)((112 - 97) / UIScale);
                    y = (int)(97 / UIScale);
                    ratio_x = (763f / 1920f);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Normal:
                    //Normal = .897
                    //782,86 - 1034,100 @ 1920x1080
                    //ratio_w = ((1034f - 782f) / 1920f);
                    //ratio_h = ((100f - 86f) / 1080f);
                    //ratio_y = (87f / 1080f);
                    w = (int)((1036 - 783) / UIScale);
                    h = (int)((102 - 87) / UIScale);
                    y = (int)(87 / UIScale);
                    ratio_x = (783f / 1920f);
                    break;
                case Gw2Sharp.Mumble.Models.UiSize.Small:
                    //Small = .81
                    //799,78 - 1027, 90 @ 1920x1080
                    //ratio_w = ((1027f - 799f) / 1920f);
                    //ratio_h = ((90f - 78f) / 1080f);
                    //ratio_y = (79f / 1080f);
                    w = (int)((1028 - 800) / UIScale);
                    h = (int)((91 - 78) / UIScale);
                    y = (int)(78 / UIScale);
                    ratio_x = (800f / 1920f);
                    break;
            }
            //w = (int)((ratio_w * GameService.Graphics.WindowWidth / UIScale));
            //h = (int)((ratio_h * GameService.Graphics.WindowHeight / UIScale));
            x = (int)((ratio_x * GameService.Graphics.WindowWidth / UIScale));
            //y = (int)((ratio_y * GameService.Graphics.WindowHeight / UIScale));

            this.Size = new Point(w + 15, h + 15);
            this.Location = new Point(x, y);
        }
        public override void PaintAfterChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            //spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(0, 0, Size.X-15, Size.Y-15), Color.White);

            if (Lines != null)
            {
                arrowNotes.Clear();
                foreach (GridLine _line in Lines)
                {
                    Color color = FindColor(_line.color);
                    float pct = (float)_line.pct / 100f * ((float)this.Size.X-15);
                    spriteBatch.DrawOnCtrl(this,
                        ContentService.Textures.Pixel,
                        new Rectangle((int)pct, 0, 1, Size.Y - 15),
                        null,
                        color
                        );
                    spriteBatch.DrawOnCtrl(this,
                        arrow,
                        new Rectangle((int)pct-5, Size.Y - 15, 11, 11),
                        null,
                        Color.White
                        );
                    arrowNotes.Add(
                        new ArrowNote()
                        {
                            Loc = new Vector2(this.Location.X + (int)pct, this.Location.Y + Size.Y - 8),  
                            Note = _line.pct + "%: " + _line.description
                        });
                }
            }
        }
        private Color FindColor(string colorname)
        {
            System.Drawing.Color systemColor = System.Drawing.Color.FromName(colorname);
            return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
        }

    }
}
